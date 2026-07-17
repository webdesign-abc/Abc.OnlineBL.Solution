using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Orders.Workflow.Model;
using Abc.OnlineBL.VirtualFileSystem;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using System.Net.Mail;
using System.Text;
using Abc.OnlineBL.Entities.Enums;
using System.Xml.Linq;
using Dom = Abc.OnlinePublication.Common.DOM;
using Abc.OnlinePublication.Common.DOM;
namespace Abc.OnlineBL.Orders.Workflow
{
    public class OrderProcessor
    {
        #region CreateClientPersonalAccount
        public static void CreateClientPersonalAccount(int clientID)
        {
            if (clientID < 0)
            {
                throw new ArgumentNullException("clientID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.CreatePersonalAccount(clientID);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CreateClientPersonalAccount'. clientID{0}", clientID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetPropertyById
        public static Abc.OnlineBL.Entities.Property GetPropertyById(int propertyID, List<EntityRelations> loadOptions)
        {
            if (propertyID <= 0)
            {
                throw new ArgumentNullException("propertyID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Abc.OnlineBL.Entities.Property pro = ctx.Properties.SingleOrDefault(p => p.PropertyId == propertyID);

                    return pro;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetPropertyById'. propertyID:{0}", propertyID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetClientContactById
        public static Abc.OnlineBL.Entities.ClientContact GetClientContactById(int contactID, List<EntityRelations> loadOptions)
        {
            if (contactID <= 0)
            {
                throw new ArgumentNullException("contactID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    ClientContact clientContact = (from cc in ctx.ClientContacts
                                                   where cc.ContactId == contactID
                                                   select cc).FirstOrDefault();

                    return clientContact;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetClientContactById'. contactID:{0}", contactID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetClientById
        public static Client GetClientById(int clientId, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    Client client = (from c in ctx.Clients
                                     where c.ClientID == clientId
                                     select c).FirstOrDefault();

                    return client;
                }
            }
            catch (Exception ex)
            {
                string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
                string message = string.Format("Error occured in 'GetClientByIdWithLoadOptions'. ClientId:{0} LoadOptions:{1}", clientId, options);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region CreateNewOrder
        public static int CreateNewOrder(NewOrder nOrder, bool isWorkshop)
        {
            if (nOrder == null)
            {
                throw new ArgumentNullException("nOrder");
            }
            try
            {
                int? orderId = 0;

                //Try to move installation file to server first
                bool moveInsFileSuccess = false;
                string destFile = string.Empty;
                string finalDestFile = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(nOrder.InstallFile))
                    {
                        if (System.IO.File.Exists(nOrder.InstallFile))
                        {
                            string fileName = System.IO.Path.GetFileName(nOrder.InstallFile);
                            destFile = System.IO.Path.Combine(OnlineBLConfig.INSTALLATION_FILE_PATH, fileName);
                            System.IO.File.Copy(nOrder.InstallFile, destFile, true);
                            System.IO.File.Delete(nOrder.InstallFile);
                            moveInsFileSuccess = true;
                            Logger.Warn("Installation file:" + destFile);
                        }
                        else
                        {
                            Logger.Warn("Installation file not found:" + nOrder.InstallFile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Moving Installation File failed");
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.CommandTimeout = ctx.CommandTimeout * 2;
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();

                        if (moveInsFileSuccess)
                        {
                            if (string.IsNullOrEmpty(nOrder.ErectionNotes))
                                nOrder.ErectionNotes = DateTime.Now.ToString() + " Installation File has been uploaded";
                            else
                                nOrder.ErectionNotes = nOrder.ErectionNotes + " -- " + DateTime.Now.ToString() + " Installation File has been uploaded";
                        }

                        //Create new order
                        ctx.AIS_NewOrder(nOrder.ClientId, nOrder.LocId, nOrder.Property.Replace("  ", " "), nOrder.Caption,
                                        nOrder.Notes, nOrder.NoBoards,
                                        nOrder.ErectionNotes, nOrder.SendBy, nOrder.SendTo,
                                        ref orderId, nOrder.RefNo,
                                        nOrder.TransformListing, nOrder.SendSms,
                                        nOrder.SmsText, nOrder.SmsAgentMobileNo,
                                        nOrder.SmsNotifyAgent, nOrder.SmsSendEmail,
                                        nOrder.SmsAgentEmailAdd, nOrder.InddTemplatesAvail,
                                        nOrder.MMS_Allowed,
                                        nOrder.PreferredErectionDate, nOrder.PreferredErectionType,
                                        nOrder.PreferredRemovalDate, nOrder.PreferredRemovalType, moveInsFileSuccess ? destFile : null, nOrder.AgentContactId > 0 ? nOrder.AgentContactId : null, nOrder.IsExpressOrder);


                        if (!orderId.HasValue)
                            return 0;

                        //rename the file with order id
                        if (moveInsFileSuccess)
                        {
                            try
                            {
                                string fileExt = Path.GetExtension(nOrder.InstallFile);
                                string fName = orderId.Value.ToString() + "_InstallFile" + fileExt;
                                string finalMove = System.IO.Path.Combine(OnlineBLConfig.INSTALLATION_FILE_PATH, fName);
                                File.Move(destFile, finalMove);
                                finalDestFile = finalMove;

                                OrderOtherDetail od = (from o in ctx.OrderOtherDetails
                                                       where o.OrderId == orderId.Value
                                                       select o).FirstOrDefault();
                                if (od != null)
                                {
                                    od.InstallFile = finalMove;
                                }
                                else
                                {
                                    od = new OrderOtherDetail();
                                    od.OrderId = orderId.Value;
                                    od.InstallFile = finalMove;
                                    ctx.OrderOtherDetails.InsertOnSubmit(od);
                                }
                                ctx.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Rename Installation File failed");
                            }
                        }

                        //Site Inspection
                        if (nOrder.IsSiteInpectionRequired)
                        {

                            Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderId.Value);

                            //ROnlineBLe an Event to send email notification to Admin
                            int eventID = EventSettings.SiteInspectionRequired;
                            string sub = "Abc Notification: Site Inspection for Job No " + od.OrderID;
                            string xmlData = string.Empty;
                            if (!string.IsNullOrEmpty(nOrder.SiteInspectionNotes))
                            {
                                xmlData = @"<EVENT>
									<OrderID>" + od.OrderID + @"</OrderID>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
									<Notes>" + nOrder.SiteInspectionNotes.Replace("&", "&amp;") + @"</Notes>
									</EVENT>";
                            }
                            else
                            {
                                xmlData = @"<EVENT>
									<OrderID>" + od.OrderID + @"</OrderID>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
									<Notes>" + @"</Notes>
									</EVENT>";
                            }
                            string textData = "Site Inspection Notes for Job No " + od.OrderID;
                            string source = "OnlineBL_Workflow_CreateNewOrder";

                            ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, null, null, null, source, moveInsFileSuccess ? finalDestFile : String.Empty);
                        }


                        //link property id with order id
                        ctx.PropertyOrderInsert(nOrder.PropertyId, orderId.Value);

                        if (nOrder.IsStockBoardOrder)
                        {
                            InsertStockboardProduct(nOrder.PropertyOrder, orderId.Value, ctx);
                        }
                        else
                        {
                            InsertProducts(nOrder.PropertyOrder, nOrder.ManagerId, orderId.Value, ctx, isWorkshop);

                            try
                            {
                                DespatchDetail despatch = (from des in ctx.DespatchDetails
                                                           where des.OrderID == orderId
                                                           select des).FirstOrDefault();

                                if (despatch != null && nOrder.PropertyOrder.OrderHasLargeBoard())
                                {
                                    despatch.RequireTwoMen = true;
                                    Logger.Warn("Order has Large Board: " + orderId.ToString());
                                }

                                if (despatch != null && (nOrder.PropertyOrder.OrderHasHighInstallation() || nOrder.PropertyOrder.BoardInstallationType == BoardInstallationType.High || nOrder.PropertyOrder.BoardInstallationType == BoardInstallationType.HigherThanFirstLevel))
                                {
                                    despatch.HighInstallRunOnly = true;
                                    Logger.Warn("Order has High Install: " + orderId.ToString());
                                }

                                ctx.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "Error occured in 'Saving DespatchDetails'.");
                            }


                            if (nOrder.HasTextReceived && !nOrder.IsDIYOrder)
                            {
                                MaterialDetail md = (from m in ctx.MaterialDetails
                                                     where m.OrderID == orderId
                                                     select m).FirstOrDefault();
                                if (md != null)
                                {
                                    md.TextReceived = DateTime.Now;
                                }
                                ctx.SubmitChanges();
                            }
                            if (nOrder.IsDIYOrder && nOrder.Caption == "DIY")
                            {
                                MaterialDetail md = (from m in ctx.MaterialDetails
                                                     where m.OrderID == orderId
                                                     select m).FirstOrDefault();
                                if (md != null)
                                {
                                    md.TextReceived = DateTime.Now;
                                }
                                ctx.SubmitChanges();
                            }

                        }

                        //Add flag holder logic For Workshop Manager and Newcastle Regional Manager
                        if (isWorkshop || nOrder.ManagerId == ManagerSettings.GarryPrince)
                        {
                            ClientsPref cp = (from c in ctx.ClientsPrefs
                                              where c.ClientId == nOrder.ClientId && c.PrefID == ClientsPref.FlagHolderOnAllBoard
                                              select c).FirstOrDefault();

                            if (cp != null && cp.BitValue.HasValue && cp.BitValue == true)
                            {
                                if (!nOrder.IsStockBoardOrder)
                                {
                                    //Get Order Details

                                    var odBoard = (from od in ctx.OrderDetails
                                                   where od.OrderID == orderId && ((od.Product.TypeID == ProductTypes.BillBoard && !od.Product.Name.Contains("Community") && od.Product.FrameType != "Window Vinyl" && od.Product.FrameType != "Vinyl Sold Sticker/s")
                                                   || (od.Product.TypeID == ProductTypes.Stockboard && od.Product.ContentType != "SB For Lease"))
                                                   select od).ToList();

                                    var odFlagHolder = (from od in ctx.OrderDetails
                                                        where od.OrderID == orderId && od.ProductID == ProductSettings.FlagHolder
                                                        select od).ToList();

                                    if (odBoard != null && odBoard.Count > 0)
                                    {
                                        int totalBoard = 0;
                                        int totalFlagHolder = 0;
                                        int diff = 0;

                                        totalBoard = odBoard.Count(od => od.Product.TypeID == ProductTypes.BillBoard || od.Product.TypeID == ProductTypes.Stockboard);

                                        if (odFlagHolder != null && odFlagHolder.Count > 0)
                                        {
                                            totalFlagHolder = odFlagHolder.Count(od => od.ProductID == ProductSettings.FlagHolder);
                                        }

                                        if (totalBoard > totalFlagHolder)
                                        {
                                            diff = totalBoard - totalFlagHolder;

                                            if (odFlagHolder != null && odFlagHolder.Count > 0)
                                            {
                                                odFlagHolder[0].Qty = odFlagHolder[0].Qty + diff;
                                                odFlagHolder[0].Total = (odFlagHolder[0].Price + odFlagHolder[0].GST) * diff;
                                                ctx.SubmitChanges();
                                            }
                                            else
                                            {
                                                InsertProductIntoOrderDetails(ctx, nOrder.ClientId, ProductSettings.FlagHolder, diff,
                                                    null, false, orderId.Value, null, null, false);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //check if it is lease board
                                    var odBoard = (from od in ctx.OrderDetails
                                                   where od.OrderID == orderId && od.Product.TypeID == ProductTypes.Stockboard && od.Product.ContentType != "SB For Lease"
                                                   select od).ToList();

                                    if (nOrder.ClientId == 20771)
                                    {
                                        odBoard = (from od in ctx.OrderDetails
                                                   where od.OrderID == orderId && od.Product.TypeID == ProductTypes.Stockboard
                                                   select od).ToList();
                                    }

                                    var odFlagHolder = (from od in ctx.OrderDetails
                                                        where od.OrderID == orderId && od.ProductID == ProductSettings.FlagHolder
                                                        select od).ToList();

                                    if (odBoard != null && odBoard.Count > 0)
                                    {
                                        int totalBoard = 0;
                                        int totalFlagHolder = 0;
                                        int diff = 0;

                                        totalBoard = odBoard.Count(od => od.Product.TypeID == ProductTypes.Stockboard);

                                        if (odFlagHolder != null && odFlagHolder.Count > 0)
                                        {
                                            totalFlagHolder = odFlagHolder.Count(od => od.ProductID == ProductSettings.FlagHolder);
                                        }

                                        if (totalBoard > totalFlagHolder)
                                        {
                                            diff = totalBoard - totalFlagHolder;

                                            if (odFlagHolder != null && odFlagHolder.Count > 0)
                                            {
                                                odFlagHolder[0].Qty = diff;
                                                odFlagHolder[0].Total = (odFlagHolder[0].Price + odFlagHolder[0].GST) * diff;
                                                ctx.SubmitChanges();
                                            }
                                            else
                                            {
                                                InsertProductIntoOrderDetails(ctx, nOrder.ClientId, ProductSettings.FlagHolder, diff,
                                                    null, false, orderId.Value, null, null, false);
                                            }
                                        }
                                    }
                                }
                            }

                        }

                        //Add flag holder logic For Con Costi Regional Manager
                        if (nOrder.ManagerId == ManagerSettings.ConCostiWollongong)
                        {
                            ClientsPref cp = (from c in ctx.ClientsPrefs
                                              where c.ClientId == nOrder.ClientId && c.PrefID == ClientsPref.FlagHolderOnAllBoard
                                              select c).FirstOrDefault();

                            if (cp != null && cp.BitValue.HasValue && cp.BitValue == true)
                            {
                                if (!nOrder.IsStockBoardOrder)
                                {
                                    //Get Order Details

                                    var odBoard = (from od in ctx.OrderDetails
                                                   where od.OrderID == orderId && ((od.Product.TypeID == ProductTypes.BillBoard && !od.Product.Name.Contains("Community") && od.Product.FrameType != "Window Vinyl" && od.Product.FrameType != "Vinyl Sold Sticker/s")
                                                   || (od.Product.TypeID == ProductTypes.Stockboard && od.Product.ContentType != "SB For Lease"))
                                                   select od).ToList();

                                    //As long as order has board then just add the notes for Con Costi
                                    if (odBoard != null && odBoard.Count > 0)
                                    {

                                        var order = (from o in ctx.Orders
                                                     where o.OrderID == orderId
                                                     select o).FirstOrDefault();
                                        if (order != null)
                                        {
                                            order.Notes += "\r\n----------------------\r\nBoards Accessory: Please include flag holder";
                                            order.ErectionNotes += " -- Boards Accessory: Please include flag holder";
                                            ctx.SubmitChanges();
                                        }
                                    }

                                }
                                else
                                {
                                    //check if it is lease board
                                    var odBoard = (from od in ctx.OrderDetails
                                                   where od.OrderID == orderId && od.Product.TypeID == ProductTypes.Stockboard && od.Product.ContentType != "SB For Lease"
                                                   select od).ToList();

                                    if (odBoard != null && odBoard.Count > 0)
                                    {
                                        var order = (from o in ctx.Orders
                                                     where o.OrderID == orderId
                                                     select o).FirstOrDefault();
                                        if (order != null)
                                        {
                                            order.Notes += "\r\n----------------------\r\nBoards Accessory : Please include flag holder";
                                            order.ErectionNotes += " -- Boards Accessory : Please include flag holder";
                                            ctx.SubmitChanges();
                                        }
                                    }
                                }
                            }

                        }

                        //if client supply alternate delivery address
                        if (isWorkshop)
                        {
                            if (nOrder.HasDeliveryDetails)
                            {
                                try
                                {
                                    if (nOrder.DeliveryLocationId <= 0 || nOrder.DeliveryPreference == "Alternate" || nOrder.DeliveryPreference == "2")
                                    {

                                        if (!string.IsNullOrEmpty(nOrder.DeliverySuburb) && !string.IsNullOrEmpty(nOrder.DeliveryState) && !string.IsNullOrEmpty(nOrder.DeliveryPostCode))
                                        {
                                            Location location = (from l in ctx.Locations
                                                                 where l.Location1.ToUpper() == nOrder.DeliverySuburb.ToUpper() && l.State.ToUpper() == nOrder.DeliveryState.ToUpper()
                                                                 select l).FirstOrDefault();

                                            if (location == null)
                                            {
                                                location = (from l in ctx.Locations
                                                            where l.PostCode == nOrder.DeliveryPostCode && l.State.ToUpper() == nOrder.DeliveryState.ToUpper()
                                                            select l).FirstOrDefault();
                                            }
                                            if (location != null)
                                                nOrder.DeliveryLocationId = location.LocationID;
                                            else
                                                nOrder.DeliveryLocationId = 10751;
                                        }
                                        else
                                        {
                                            nOrder.DeliveryLocationId = 10751;
                                        }

                                    }

                                    OrderOtherDetail od = (from o in ctx.OrderOtherDetails
                                                           where o.OrderId == orderId.Value
                                                           select o).FirstOrDefault();
                                    if (od != null)
                                    {
                                        od.DeliveryName = nOrder.DeliveryName;
                                        od.DeliveryEmail = nOrder.DeliveryEmail;
                                        od.DeliveryStreetAddress = nOrder.DeliveryAddress;
                                        od.DeliveryLocationId = nOrder.DeliveryLocationId;
                                    }
                                    else
                                    {
                                        od = new OrderOtherDetail();
                                        od.OrderId = orderId.Value;
                                        od.DeliveryName = nOrder.DeliveryName;
                                        od.DeliveryEmail = nOrder.DeliveryEmail;
                                        od.DeliveryStreetAddress = nOrder.DeliveryAddress;
                                        od.DeliveryLocationId = nOrder.DeliveryLocationId;
                                        ctx.OrderOtherDetails.InsertOnSubmit(od);
                                    }

                                    Logger.Warn(orderId.Value + " - " + od.DeliveryName + " - " + nOrder.DeliverySuburb + " - " + nOrder.DeliveryState  + " - " + nOrder.DeliveryPostCode + " - " + nOrder.DeliveryLocationId.ToString());

                                    ctx.SubmitChanges();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Client Supply Alternate delivery Address");
                                }
                            }

                            try
                            {
                                ClientsPref cp = (from c in ctx.ClientsPrefs
                                                  where c.ClientId == nOrder.ClientId && c.PrefID == ClientsPref.SupplyAlternativeDeliveryAddress
                                                  select c).FirstOrDefault();
                                bool hasSupplyAlternativeDeliveryAddressPref = false;
                                if (cp != null && cp.BitValue.HasValue && cp.BitValue.Value)
                                {
                                    hasSupplyAlternativeDeliveryAddressPref = true;
                                }

                                if (hasSupplyAlternativeDeliveryAddressPref)
                                {
                                    if (!string.IsNullOrEmpty(nOrder.ManagerState) && !string.IsNullOrEmpty(nOrder.DeliveryState) && nOrder.ManagerState.ToUpper() != nOrder.DeliveryState.ToUpper())
                                    {
                                        string managerID = nOrder.ManagerId;
                                        if (nOrder.PropertyOrder.OrderHasBoardNotIncludingFlatPack())
                                        {
                                            if (nOrder.DeliveryState.ToUpper() == "QLD")
                                                managerID = ManagerSettings.WorkshopQueensland;
                                            else if (nOrder.DeliveryState.ToUpper() == "NSW")
                                                managerID = ManagerSettings.WorkshopNewSouthWales;
                                            else if (nOrder.DeliveryState.ToUpper() == "VIC")
                                                managerID = ManagerSettings.WorkshopVictoria;
                                            else if (nOrder.DeliveryState.ToUpper() == "WA")
                                                managerID = ManagerSettings.WorkshopWesternAustralia;
                                            else if (nOrder.DeliveryState.ToUpper() == "SA")
                                                managerID = ManagerSettings.WorkshopSouthAustralia;
                                            else
                                                managerID = ManagerSettings.InHouse;
                                        }
                                        else
                                        {
                                            if (nOrder.PropertyOrder.OrderOnlyHasNonBoardExcludeFlatPackItems())
                                            {
                                                managerID = ManagerSettings.InHouse;
                                            }
                                        }

                                        Order od = (from o in ctx.Orders
                                                    where o.OrderID == orderId.Value
                                                    select o).FirstOrDefault();
                                        if (od != null)
                                        {
                                            od.ManagerID = managerID;
                                        }
                                        ctx.SubmitChanges();
                                    }
                                }
                                else
                                {
                                    try
                                    {

                                        if (nOrder.DeliveryPreference != "Office" && nOrder.DeliveryPreference != "3" && !string.IsNullOrEmpty(nOrder.DeliverySuburb))
                                        {
                                            bool deliveryInZone1 = false;
                                            var dl = ctx.ClientNationalDeliveryLocations.Where(loc => loc.Suburb.ToUpper() == nOrder.DeliverySuburb.Trim().ToUpper() && loc.State.ToUpper() == nOrder.DeliveryState.Trim().ToUpper()).FirstOrDefault();
                                            if (dl != null)
                                            {

                                                if (dl.DeliveryRegion.ToUpper().Contains("ZONE 1"))
                                                    deliveryInZone1 = true;
                                            }

                                            Logger.Warn("Pref 1: " + nOrder.DeliveryPreference + " - " + orderId.Value);

                                            if (deliveryInZone1)
                                            {
                                                try
                                                {
                                                    if (nOrder.ClientId != 21159)
                                                    {
                                                        decimal delZone1Price = (decimal)18.1818;

                                                        try
                                                        {
                                                            var prlid = (from c in ctx.Clients
                                                                         join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                                         where c.ClientID == nOrder.ClientId
                                                                         && i.ProductID == WorkflowConfig.DELIVERY_ZONE_1_PRODUCT_ID
                                                                         select i).FirstOrDefault();
                                                            if (prlid != null && prlid.ProductPrice >= 0)
                                                            {
                                                                delZone1Price = prlid.ProductPrice;
                                                            }
                                                            else
                                                            {
                                                                //look through product pricing
                                                                var priceList = (from c in ctx.Clients
                                                                                 join i in ctx.PriceLists on c.PriceListID equals i.PriceListID
                                                                                 where c.ClientID == nOrder.ClientId
                                                                                 select i).FirstOrDefault();

                                                                if (priceList != null)
                                                                {
                                                                    var productp = (from pp in ctx.ProductPricings
                                                                                    where pp.PricingID == priceList.PricingID
                                                                                    && pp.ProductID == WorkflowConfig.DELIVERY_ZONE_1_PRODUCT_ID
                                                                                    select pp).FirstOrDefault();

                                                                    if (productp != null)
                                                                    {
                                                                        delZone1Price = productp.Price;
                                                                    }

                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Logger.Exception(ex, string.Format("Can not find price for product {0} {1}", orderId, WorkflowConfig.DELIVERY_ZONE_1_PRODUCT_ID));
                                                        }

                                                        InsertUpgradeProductIntoOrderDetails(ctx, nOrder.ClientId, WorkflowConfig.DELIVERY_ZONE_1_PRODUCT_ID, 1,
                                                        string.Empty, false, orderId.Value, null, null, false, (decimal)delZone1Price);
                                                    }
                                                    else
                                                    {
                                                        InsertUpgradeProductIntoOrderDetails(ctx, nOrder.ClientId, WorkflowConfig.DELIVERY_ZONE_1_PRODUCT_ID, 1,
                                                        string.Empty, false, orderId.Value, null, null, false, (decimal)0);
                                                    }
                                                }
                                                catch (System.Exception ex)
                                                {
                                                    Logger.Exception(ex, string.Format("Can not insert product {0} {1}", orderId, WorkflowConfig.DELIVERY_ZONE_1_PRODUCT_ID));
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    InsertUpgradeProductIntoOrderDetails(ctx, nOrder.ClientId, WorkflowConfig.DELIVERY_ZONE_2_PRODUCT_ID, 1,
                                                        string.Empty, false, orderId.Value, null, null, false, (decimal)30);
                                                }
                                                catch (System.Exception ex)
                                                {
                                                    Logger.Exception(ex, string.Format("Can not insert product {0} {1}", orderId, WorkflowConfig.DELIVERY_ZONE_2_PRODUCT_ID));
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Logger.Exception(exc, "Error occured in 'CreateNewOrder'. Client Supply Alternate delivery Address " + nOrder.DeliverySuburb + " -- " + nOrder.DeliveryState);
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Client Supply Alternate delivery Address");
                            }

                        }

                        //do the delivery address same as workshop manager for Con Costi regional manager
                        if (nOrder.ManagerId == ManagerSettings.ConCostiWollongong)
                        {
                            if (nOrder.HasDeliveryDetails)
                            {
                                try
                                {
                                    if (nOrder.DeliveryLocationId <= 0)
                                    {

                                        if (!string.IsNullOrEmpty(nOrder.DeliverySuburb) && !string.IsNullOrEmpty(nOrder.DeliveryState) && !string.IsNullOrEmpty(nOrder.DeliveryPostCode))
                                        {
                                            Location location = (from l in ctx.Locations
                                                                 where l.Location1.ToUpper() == nOrder.DeliverySuburb.ToUpper() && l.State.ToUpper() == nOrder.DeliveryState.ToUpper()
                                                                 select l).FirstOrDefault();

                                            if (location == null)
                                            {
                                                location = (from l in ctx.Locations
                                                            where l.PostCode == nOrder.DeliveryPostCode && l.State.ToUpper() == nOrder.DeliveryState.ToUpper()
                                                            select l).FirstOrDefault();
                                            }
                                            if (location != null)
                                                nOrder.DeliveryLocationId = location.LocationID;
                                            else
                                                nOrder.DeliveryLocationId = 10751;
                                        }
                                        else
                                        {
                                            nOrder.DeliveryLocationId = 10751;
                                        }

                                    }

                                    OrderOtherDetail od = (from o in ctx.OrderOtherDetails
                                                           where o.OrderId == orderId.Value
                                                           select o).FirstOrDefault();
                                    if (od != null)
                                    {
                                        od.DeliveryName = nOrder.DeliveryName;
                                        od.DeliveryEmail = nOrder.DeliveryEmail;
                                        od.DeliveryStreetAddress = nOrder.DeliveryAddress;
                                        od.DeliveryLocationId = nOrder.DeliveryLocationId;
                                    }
                                    else
                                    {
                                        od = new OrderOtherDetail();
                                        od.OrderId = orderId.Value;
                                        od.DeliveryName = nOrder.DeliveryName;
                                        od.DeliveryEmail = nOrder.DeliveryEmail;
                                        od.DeliveryStreetAddress = nOrder.DeliveryAddress;
                                        od.DeliveryLocationId = nOrder.DeliveryLocationId;
                                        ctx.OrderOtherDetails.InsertOnSubmit(od);
                                    }
                                    Logger.Warn(od.DeliveryName + " -Manager- " + nOrder.DeliveryLocationId.ToString());
                                    ctx.SubmitChanges();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Client Manager Supply Alternate delivery Address");
                                }
                            }

                        }

                        ClientsPref photoApprovalFirstPref = (from c in ctx.ClientsPrefs
                                                              where c.ClientId == nOrder.ClientId && c.PrefID == ClientsPref.PhotographyApprovalFirstRequired
                                          select c).FirstOrDefault();

                        bool photoApprovalFirst = false;
                        if (photoApprovalFirstPref != null && photoApprovalFirstPref.BitValue.HasValue && photoApprovalFirstPref.BitValue.Value)
                        {
                            photoApprovalFirst = true;
                        }

                        if (!nOrder.IsStockBoardOrder && nOrder.PropertyOrder.ClientHasStockboardDIYPreference && nOrder.PropertyOrder.IsDIYOrder
                            && nOrder.PropertyOrder.OrderHasPhotographyorFloorPlan() && nOrder.PropertyOrder.OrderHasStockboardIncludePackageCheck() && photoApprovalFirst)
                        {
                            Order od = (from o in ctx.Orders
                                        where o.OrderID == orderId.Value
                                        select o).FirstOrDefault();
                            if (od != null)
                            {
                                od.OnHold = DateTime.Now;
                                od.Notes = "On Hold – awaiting photography to be completed" + od.Notes;
                            }
                            ctx.SubmitChanges();
                        }

                        if(nOrder.DeliveryPreference == "Pickup")
                        {
                            Order od = (from o in ctx.Orders
                                        where o.OrderID == orderId.Value
                                        select o).FirstOrDefault();
                            if (od != null)
                            {
                                od.ManagerID = ManagerSettings.InHouse;
                            }
                            ctx.SubmitChanges();
                        }

                        ctx.Transaction.Commit();

                        return orderId.HasValue ? orderId.Value : 0;
                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        Logger.Exception(ex, "Error occured in 'CreateNewOrder'.");
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CreateNewOrder'.");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GenerateNewOrderEvent
        public static void GenerateNewOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    //Create new order event
                    ctx.CDAS_GenNewOrderEvent(nOrderEvent.OrderId, nOrderEvent.ClientId,
                                            nOrderEvent.HtmlBody, nOrderEvent.HtmlBodyUS,
                                            nOrderEvent.FileName, nOrderEvent.Prop);

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewOrderEvent'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region Generate3DLetteringBoardOrderToManagerEvent
        public static void Generate3DLetteringBoardOrderToManagerEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var client = (from c in ctx.Clients
                                  where c.ClientID == nOrderEvent.ClientId
                                  select c).FirstOrDefault();
                    if (client != null)
                    {
                        string sub = "3D Lettering Order for " + nOrderEvent.Prop + " - Order No: " + nOrderEvent.OrderId;
                        ctx.SP_EventQueueAdd(EventSettings.OrderHas3DLetteringBoard, sub, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, client.ManagerID, null, "Generate3DLetteringBoardOrderToManagerEvent", nOrderEvent.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'Generate3DLetteringBoardOrderToManagerEvent'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GenerateNewOrderNoNotesEvent
        public static void GenerateNewOrderNoNotesEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string subject = "Online Order No Notes For: " + nOrderEvent.Prop + " - Job No: " + nOrderEvent.OrderId.ToString();

                    if (nOrderEvent.HtmlBodyUS != null)
                        ctx.SP_EventQueueAdd(146, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, null, null, "GenerateNewOrderNoNotesEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewOrderNoNotesEvent'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GenerateNewOrderNoTemplateEvent
        public static void GenerateNewOrderNoTemplateEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string subject = "Online Order No Template For: " + nOrderEvent.Prop + " - Job No: " + nOrderEvent.OrderId.ToString();

                    if (nOrderEvent.HtmlBodyUS != null)
                        ctx.SP_EventQueueAdd(146, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, null, null, "GenerateNewOrderNoTemplateEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewOrderNoTemplateEvent'.");
                Logger.Exception(ex, message);
                //throw;
            }
        }

        #endregion

        #region GetTemplateProductById
        public static AOP_TemplateProduct GetTemplateProductById(int templateProductId, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    AOP_TemplateProduct tp = (from t in ctx.AOP_TemplateProducts
                                              where t.TemplateProductId == templateProductId && t.Active
                                              select t).FirstOrDefault();

                    return tp;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetTemplateProductById'. templateProductId:{0}", templateProductId);
                throw;
            }
        }
        #endregion

        #region GetJobDocumentPath
        /// <summary>
        /// Gets the job document path. E.g. RootPath\ClientId\JobId\JobDocumentId\
        /// </summary>
        /// <param name="documentRootPath">The document root path.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="jobId">The job id.</param>
        /// <param name="jobDocumentId">The job document id.</param>
        /// <returns></returns>
        private static string GetJobDocumentPath(string documentRootPath, int clientId, int jobId, int jobDocumentId)
        {
            string templateFilePath = string.Format("{0}\\{1}\\{2}\\{3}", documentRootPath.TrimEnd('\\'), clientId, jobId, jobDocumentId);
            return templateFilePath;
        }
        #endregion

        #region GetJobDocumentTemplatePath
        /// <summary>
        /// Gets the job document template path. E.g. RootPath\ClientId\JobId\JobDocumentId\JobDocumentId.indt
        /// </summary>
        /// <param name="documentRootPath">The document root path.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="jobId">The job id.</param>
        /// <param name="jobDocumentId">The job document id.</param>
        /// <returns></returns>
        private static string GetJobDocumentTemplatePath(string documentRootPath, int clientId, int jobId, int jobDocumentId)
        {
            string templateFile = Path.Combine(GetJobDocumentPath(documentRootPath, clientId, jobId, jobDocumentId), jobDocumentId + ".indt");
            return templateFile;
        }
        #endregion

        #region CreateAssetsFolders
        /// <summary>
        /// Create Assets Folders path. E.g. RootPath\ClientId\JobId\Assets\Original
        /// </summary>
        /// <param name="documentRootPath">The document root path.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="jobId">The job id.</param>
        /// <returns></returns>
        public static void CreateAssetsFolders(string documentRootPath, int clientId, int jobId)
        {
            string assetsOriginal = string.Format("{0}\\{1}\\{2}\\{3}\\{4}", documentRootPath.TrimEnd('\\'), clientId, jobId, "Assets", "Original");
            string assetsMediumRes = string.Format("{0}\\{1}\\{2}\\{3}\\{4}", documentRootPath.TrimEnd('\\'), clientId, jobId, "Assets", "MediumRes");
            string assetsLowRes = string.Format("{0}\\{1}\\{2}\\{3}\\{4}", documentRootPath.TrimEnd('\\'), clientId, jobId, "Assets", "LowRes");
            string transformedAssetsOriginal = string.Format("{0}\\{1}\\{2}\\{3}\\{4}", documentRootPath.TrimEnd('\\'), clientId, jobId, "TransformedAssets", "Original");
            string transformedAssetsMediumRes = string.Format("{0}\\{1}\\{2}\\{3}\\{4}", documentRootPath.TrimEnd('\\'), clientId, jobId, "TransformedAssets", "MediumRes");
            string transformedAssetsLowRes = string.Format("{0}\\{1}\\{2}\\{3}\\{4}", documentRootPath.TrimEnd('\\'), clientId, jobId, "TransformedAssets", "LowRes");
           
            try
            {

                IFile file = VirtualFileSystemFactory.GetFile();

                file.CreateDir(assetsOriginal);
                file.CreateDir(assetsMediumRes);
                file.CreateDir(assetsLowRes);
                file.CreateDir(transformedAssetsOriginal);
                file.CreateDir(transformedAssetsMediumRes);
                file.CreateDir(transformedAssetsLowRes);

            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "CreateAssetsFolders");
                throw;
            }
        }
        #endregion

        #region CreateNewPhotoOrder
        public static int CreateNewPhotoOrder(NewPhotoOrder nPhotoOrder)
        {
            if (nPhotoOrder == null)
            {
                throw new ArgumentNullException("nPhotoOrder");
            }
            try
            {
                int? photoOrderId = 0;

                //Try to move photography uploaded file to server first
                bool movePhotographyFileSuccess = false;
                bool moveInstructionFileSuccess = false;
                string destPhotographyFile = string.Empty;
                string destInstructionFile = string.Empty;
                string finalDestPhotographyFile = string.Empty;
                string finalDestInstructionFile = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(nPhotoOrder.PhotographyFile))
                    {
                        if (System.IO.File.Exists(nPhotoOrder.PhotographyFile))
                        {
                            string fileName = System.IO.Path.GetFileName(nPhotoOrder.PhotographyFile);
                            destPhotographyFile = System.IO.Path.Combine(WorkflowConfig.PHOTOGRAPHY_UPLOAD_FILE_PATH, fileName);
                            System.IO.File.Copy(nPhotoOrder.PhotographyFile, destPhotographyFile, true);
                            System.IO.File.Delete(nPhotoOrder.PhotographyFile);
                            movePhotographyFileSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Moving Photography File failed");
                }

                try
                {
                    if (!string.IsNullOrEmpty(nPhotoOrder.SitePlanInstructionFile))
                    {
                        if (System.IO.File.Exists(nPhotoOrder.SitePlanInstructionFile))
                        {
                            string fileName = System.IO.Path.GetFileName(nPhotoOrder.SitePlanInstructionFile);
                            destInstructionFile = System.IO.Path.Combine(WorkflowConfig.INSTRUCTION_UPLOAD_FILE_PATH, fileName);
                            System.IO.File.Copy(nPhotoOrder.SitePlanInstructionFile, destInstructionFile, true);
                            System.IO.File.Delete(nPhotoOrder.SitePlanInstructionFile);
                            moveInstructionFileSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Moving Instruction File failed");
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();

                        Logger.Warn(nPhotoOrder.ContactName + " " + nPhotoOrder.ContactNumber);

                        //Create new order event
                        ctx.BL_NewPhotoOrder(nPhotoOrder.ClientId, nPhotoOrder.LocId,
                                                nPhotoOrder.Property, nPhotoOrder.Caption,
                                                nPhotoOrder.Notes, nPhotoOrder.ErectionNotes,
                                                nPhotoOrder.OrderData, nPhotoOrder.RefNo,
                                                ref photoOrderId, nPhotoOrder.Instructions,
                                                nPhotoOrder.VendorName, nPhotoOrder.VendorPhone,
                                                nPhotoOrder.IsKeySafe, nPhotoOrder.IsPickupKeys,
                                                nPhotoOrder.PhotoContact, nPhotoOrder.HouseFaces,
                                                nPhotoOrder.Melway, nPhotoOrder.SendBy,
                                                nPhotoOrder.SendTo, nPhotoOrder.ContactName, nPhotoOrder.ContactNumber);


                        if (!photoOrderId.HasValue)
                            return 0;

                        //rename the photography uploaded file with order id
                        if (movePhotographyFileSuccess)
                        {
                            try
                            {
                                string fileExt = Path.GetExtension(nPhotoOrder.PhotographyFile);
                                string fName = photoOrderId.Value.ToString() + "_PhotographyFile" + fileExt;
                                string finalMove = System.IO.Path.Combine(WorkflowConfig.PHOTOGRAPHY_UPLOAD_FILE_PATH, fName);
                                File.Move(destPhotographyFile, finalMove);
                                finalDestPhotographyFile = finalMove;

                                OrderOtherDetail od = (from o in ctx.OrderOtherDetails
                                                       where o.OrderId == photoOrderId.Value
                                                       select o).FirstOrDefault();
                                if (od != null)
                                {
                                    od.PhotographyFile = finalDestPhotographyFile;
                                }
                                else
                                {
                                    od = new OrderOtherDetail();
                                    od.OrderId = photoOrderId.Value;
                                    od.PhotographyFile = finalDestPhotographyFile;
                                    ctx.OrderOtherDetails.InsertOnSubmit(od);
                                }
                                ctx.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Rename Photography File failed");
                            }
                        }

                        if (moveInstructionFileSuccess)
                        {
                            try
                            {
                                string fileExt = Path.GetExtension(nPhotoOrder.SitePlanInstructionFile);
                                string fName = photoOrderId.Value.ToString() + "_InstructionFile" + fileExt;
                                string finalMove = System.IO.Path.Combine(WorkflowConfig.INSTRUCTION_UPLOAD_FILE_PATH, fName);
                                File.Move(destInstructionFile, finalMove);
                                finalDestInstructionFile = finalMove;

                                OrderOtherDetail od = (from o in ctx.OrderOtherDetails
                                                       where o.OrderId == photoOrderId.Value
                                                       select o).FirstOrDefault();
                                if (od != null)
                                {
                                    od.SitePlanInstructionFile = finalDestInstructionFile;
                                }
                                else
                                {
                                    od = new OrderOtherDetail();
                                    od.OrderId = photoOrderId.Value;
                                    od.SitePlanInstructionFile = finalDestInstructionFile;
                                    ctx.OrderOtherDetails.InsertOnSubmit(od);
                                }
                                ctx.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "Error occured in 'CreateNewOrder'. Rename Instruction File failed");
                            }
                        }

                        //link property id with photo order id
                        ctx.PropertyOrderInsert(nPhotoOrder.PropertyId, photoOrderId.Value);

                        // Insert Photography Product into Order Details Table
                        InsertPhotographyProduct(nPhotoOrder.PropertyOrder, photoOrderId.Value, ctx);

                        bool isMudMap = false;
                        if (nPhotoOrder.PropertyOrder.Cart.Count == 1 && nPhotoOrder.PropertyOrder.Cart[0].ProductId == ProductSettings.MudMap)
                        {
                            isMudMap = true;
                        }

                        if (isMudMap || nPhotoOrder.PropertyOrder.OrderHasMudMapOrReDrawFloorplan())
                        {
                            var photoOd = (from o in ctx.PhotoOrders
                                            where o.OrderID == photoOrderId.Value
                                            select o).FirstOrDefault();
                            var PHOTO_ADMIN_ID = 1; // TODO: move to WorkflowConfig
                            photoOd.PgId = PHOTO_ADMIN_ID;
                            ctx.SubmitChanges();
                        }

                        if (nPhotoOrder.PropertyOrder.OrderHasDronePhotography())
                        {
                            var photoLocation = (from l in ctx.Locations
                                           where l.LocationID == nPhotoOrder.LocId
                                           select l).FirstOrDefault();

                            if(photoLocation != null && photoLocation.State != "VIC")
                            {
                                var photoOd = (from o in ctx.PhotoOrders
                                               where o.OrderID == photoOrderId.Value
                                               select o).FirstOrDefault();

                                if (photoOd != null)
                                {
                                    //insert new row into DronePhotography table
                                    DroneAuthorisation da = new DroneAuthorisation();
                                    da.OrderID = photoOrderId.Value;
                                    da.Status = (int)Abc.OnlineBL.Entities.Enums.ProductAvailability.MayBe;
                                    da.FlightAddress = nPhotoOrder.Property;
                                    da.RemotePilot = photoOd.PgId;
                                    da.ChiefRemotePilot = photoOd.PgId;
                                    da.LastUpdatedBy = photoOd.PgId;
                                    da.LastUpdatedOn = DateTime.Now;
                                    ctx.DroneAuthorisations.InsertOnSubmit(da);

                                    ctx.SubmitChanges();
                                }
                            }
                            else
                            {
                                var photoOd = (from o in ctx.PhotoOrders
                                               where o.OrderID == photoOrderId.Value
                                               select o).FirstOrDefault();

                                if (photoOd != null)
                                {
                                    //insert new row into DronePhotography table with PhotoAdmin as photographer
                                    DroneAuthorisation da = new DroneAuthorisation();
                                    da.OrderID = photoOrderId.Value;
                                    da.Status = (int)Abc.OnlineBL.Entities.Enums.ProductAvailability.MayBe;
                                    da.FlightAddress = nPhotoOrder.Property;
                                    da.RemotePilot = 1;
                                    da.ChiefRemotePilot = 1;
                                    da.LastUpdatedBy = 1;
                                    da.LastUpdatedOn = DateTime.Now;
                                    ctx.DroneAuthorisations.InsertOnSubmit(da);

                                    //photoOd.PgId = 1;

                                    ctx.SubmitChanges();
                                }
                            }
                        }


                        if (nPhotoOrder.PropertyOrder.OrderHasVirtualWalkThrough())
                        {
                            var photoOd = (from o in ctx.PhotoOrders
                                           where o.OrderID == photoOrderId.Value
                                           select o).FirstOrDefault();
                            var PHOTO_ADMIN_ID = 1; // TODO: move to WorkflowConfig
                            photoOd.PgId = PHOTO_ADMIN_ID;
                            ctx.SubmitChanges();
                        }

                        try
                        {
                            bool isRedrawFloorplanProduct = false;
                            if (nPhotoOrder.PropertyOrder.Cart.Count == 1 && nPhotoOrder.PropertyOrder.Cart[0].ProductId == ProductSettings.RedrawFloorPlan)
                            {
                                isRedrawFloorplanProduct = true;
                            }

                            if (isRedrawFloorplanProduct)
                            {
                                //insert new row into FloorPlanOrder table
                                FloorPlanOrder fpo = new FloorPlanOrder();
                                fpo.OrderID = photoOrderId.Value;
                                fpo.ProductID = ProductSettings.RedrawFloorPlan;
                                ctx.FloorPlanOrders.InsertOnSubmit(fpo);

                                ctx.SubmitChanges();
                            }
                        }
                        catch (Exception exc)
                        {
                            Logger.Exception(exc, "Error occured in 'CreateNewPhotoOrder': Try to create FloorPlan Order Details.");
                        }

                        if (nPhotoOrder.OrderId > 0)
                        {
                            PhotoOrderDetail pod = new PhotoOrderDetail();
                            pod.OrderID = photoOrderId.Value;
                            pod.LinkSBOrderID = nPhotoOrder.OrderId;
                            ctx.PhotoOrderDetails.InsertOnSubmit(pod);

                            ctx.SubmitChanges();
                        }

                        ctx.Transaction.Commit();

                        return photoOrderId.HasValue ? photoOrderId.Value : 0;
                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        Logger.Exception(ex, "Error occured in 'CreateNewPhotoOrder'.");
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CreateNewPhotoOrder'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GenerateNewPhotoOrderEvent
        public static void GenerateNewPhotoOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nPhotoOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (nOrderEvent.SendPhotoEmailToAdmin)
                    {
                        //Create new order event
                        ctx.CDAS_GenPhotoEvent(nOrderEvent.OrderId, nOrderEvent.ClientId,
                                                nOrderEvent.HtmlBody, nOrderEvent.HtmlBodyUS,
                                                nOrderEvent.FileName, nOrderEvent.Prop);
                    }

                    var pho = (from p in ctx.Photographers
                               join po in ctx.PhotoOrders on p.PgId equals po.PgId
                               join o in ctx.Orders on po.OrderID equals o.OrderID
                               where po.OrderID == nOrderEvent.OrderId
                               select p).FirstOrDefault();

                    var od = (from o in ctx.Orders
                              where o.OrderID == nOrderEvent.OrderId
                              select o).FirstOrDefault();

                    if (pho != null && !string.IsNullOrEmpty(pho.Email))
                    {
                        try
                        {
                            ctx.OnlineServiceQueue_AddEmailNewPhotoOrderToQueue(nOrderEvent.OrderId, pho.Email);
                        }
                        catch (Exception exx)
                        {
                            string message = string.Format("Error occured in 'GenerateNewPhotoOrderEvent' when send email to Photographer.");
                            Logger.Exception(exx, message);
                        }
                    }

                    if (nOrderEvent.OrderHasMudMapOrReDrawFloorplan)
                    {
                        //Online an Event to send email notification to Admin
                        int eventID = EventSettings.OrderHasMudMapReDraw;
                        string sub = "Abc Notification: Has Mud Map / Re-Draw Product on Order " + nOrderEvent.OrderId.ToString();
                        string xmlData = string.Empty;
                        xmlData = @"<EVENT>
									    <OrderID>" + nOrderEvent.OrderId.ToString() + @"</OrderID>
									    <ClientID>" + nOrderEvent.ClientId.ToString() + @"</ClientID>
									    <ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
									    </EVENT>";

                        string textData = "Abc Notification: Has Mud Map / Re-Draw Product on Order " + nOrderEvent.OrderId.ToString();
                        string source = "Workflow_GenerateNewPhotoOrderEvent";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, nOrderEvent.OrderId, null, null, null, source, String.Empty);
                    }

                    if (nOrderEvent.OrderHasProfessionalSlideshowVideo)
                    {
                        try
                        {
                            var cl = (from c in ctx.Clients
                                      where c.ClientID == nOrderEvent.ClientId
                                      select c).FirstOrDefault();

                            if (cl != null)
                            {
                                //Raise an Event to send email notification to Imaging
                                int eventID = EventSettings.ProfessionalSlideshowVideo;
                                string sub = "Abc Notification: Has Professional Slideshow Video Product on Order " + nOrderEvent.OrderId.ToString();
                                string xmlData = string.Empty;
                                xmlData = @"<EVENT>
									    <OrderID>" + nOrderEvent.OrderId.ToString() + @"</OrderID>
									    <AgentName>" + cl.ClientName.Replace("&", "&amp;") + @"</AgentName>
									    <Office>" + cl.Office.ToString() + @"</Office>
									    <PAddress>" + nOrderEvent.Prop.ToString().Replace("&", "&amp;") + @"</PAddress>
									    <ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
									    </EVENT>";

                                string textData = "Abc Notification: Has Professional Slideshow Video Product on Order " + nOrderEvent.OrderId.ToString();
                                string source = "Workflow_GenerateNewPhotoOrderEvent";

                                ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, nOrderEvent.OrderId, null, null, null, source, String.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, "Error on sending email to imaging");
                        }

                    }

                    if (nOrderEvent.OrderHasVirtualWalkThrough)
                    {
                        ctx.OnlineServiceQueue_AddEmailVirtualWalkThroughToQueue(nOrderEvent.OrderId, nOrderEvent.SendProofToEmail);
                    }

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewPhotoOrderEvent'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region CreateNewSBOrder
        public static int CreateNewSBOrder(NewStockBoardOrder nSBOrder)
        {
            if (nSBOrder == null)
            {
                throw new ArgumentNullException("nSBOrder");
            }
            try
            {
                int? sbOrderId = 0;
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {

                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();

                        //Create new order event
                        ctx.AIS_NewSBOrder(nSBOrder.ClientId, nSBOrder.State,
                                                nSBOrder.Loc, nSBOrder.Property,
                                                nSBOrder.Caption, nSBOrder.Notes,
                                                ref sbOrderId, nSBOrder.PreferredErectionDate,
                                                nSBOrder.PreferredErectionType, nSBOrder.AgentContactId > 0 ? nSBOrder.AgentContactId : null);

                        nSBOrder.SBOrderId = sbOrderId.HasValue ? sbOrderId.Value : 0;
                        if (nSBOrder.PropertyId > 0 && nSBOrder.SBOrderId > 0)
                        {
                            ctx.PropertySBOrderInsert(nSBOrder.PropertyId, nSBOrder.SBOrderId);
                        }

                        bool clientHasStockboardDIY = false;
                        ClientsPref cp = (from c in ctx.ClientsPrefs
                                          where c.ClientId == nSBOrder.ClientId && c.PrefID == Abc.OnlineBL.Entities.Utility.Preferences.StockboardDIY
                                            select c).FirstOrDefault();

                        if (cp != null && cp.BitValue.HasValue && cp.BitValue == true)
                            clientHasStockboardDIY = true;

                        bool hasFlagHolder = false;
                        //Insert SB_orderDetails
                        foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item in nSBOrder.PropertyOrder.Cart)
                        {
                            if (clientHasStockboardDIY)
                            {
                                if (item.TypeId == ProductTypes.Stockboard ||
                                    (item.TypeId == ProductTypes.BoardAccessory && !nSBOrder.PropertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard)) ||
                                    (item.TypeId == ProductTypes.Overlay && !nSBOrder.PropertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard)) ||
                                    (item.TypeId == ProductTypes.StockboardOverlay && !nSBOrder.PropertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard)))
                                {
                                    if (item.ProductId > 0)
                                    {
                                        if (nSBOrder.ManagerID == ManagerSettings.ConCostiWollongong)
                                        {
                                            if (item.ProductId == 429)
                                            {
                                                //dont add to order
                                                hasFlagHolder = true;
                                                Logger.Warn("By pass the product when create SB Order, OrderId {0}, ProductId {1}, Qty {2}", sbOrderId.Value, item.ProductId, item.ItemQty);
                                            }
                                            else
                                            {
                                                SB_OrderDetail sbOrderDetail = new SB_OrderDetail();
                                                sbOrderDetail.OrderID = sbOrderId.Value;
                                                sbOrderDetail.ProductID = item.ProductId;
                                                sbOrderDetail.Qty = item.ItemQty;
                                                sbOrderDetail.Details = string.Empty;
                                                sbOrderDetail.Price = 0;
                                                sbOrderDetail.GST = 0;
                                                sbOrderDetail.Total = 0;

                                                Logger.Warn("Trying to Create SB Order Line, OrderId {0}, ProductId {1}, Qty {2}", sbOrderId.Value, item.ProductId, item.ItemQty);

                                                ctx.SB_OrderDetails.InsertOnSubmit(sbOrderDetail);
                                                ctx.SubmitChanges();
                                            }
                                        }
                                        else
                                        {
                                            SB_OrderDetail sbOrderDetail = new SB_OrderDetail();
                                            sbOrderDetail.OrderID = sbOrderId.Value;
                                            sbOrderDetail.ProductID = item.ProductId;
                                            sbOrderDetail.Qty = item.ItemQty;
                                            sbOrderDetail.Details = string.Empty;
                                            sbOrderDetail.Price = 0;
                                            sbOrderDetail.GST = 0;
                                            sbOrderDetail.Total = 0;

                                            Logger.Warn("Trying to Create SB Order Line, OrderId {0}, ProductId {1}, Qty {2}", sbOrderId.Value, item.ProductId, item.ItemQty);

                                            ctx.SB_OrderDetails.InsertOnSubmit(sbOrderDetail);
                                            ctx.SubmitChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (item.TypeId == ProductTypes.Stockboard ||
                                    (item.TypeId == ProductTypes.BoardAccessory && !nSBOrder.PropertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard)) ||
                                    (item.TypeId == ProductTypes.Overlay && !nSBOrder.PropertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard)) ||
                                    (item.TypeId == ProductTypes.StockboardOverlay && !nSBOrder.PropertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard)))
                                {
                                    if (item.ProductId > 0)
                                    {
                                        if (nSBOrder.ManagerID == ManagerSettings.ConCostiWollongong)
                                        {
                                            if (item.ProductId == 429)
                                            {
                                                //dont add to order
                                                hasFlagHolder = true;
                                                Logger.Warn("By pass the product when create SB Order, OrderId {0}, ProductId {1}, Qty {2}", sbOrderId.Value, item.ProductId, item.ItemQty);
                                            }
                                            else
                                            {
                                                SB_OrderDetail sbOrderDetail = new SB_OrderDetail();
                                                sbOrderDetail.OrderID = sbOrderId.Value;
                                                sbOrderDetail.ProductID = item.ProductId;
                                                sbOrderDetail.Qty = item.ItemQty;
                                                sbOrderDetail.Details = string.Empty;
                                                sbOrderDetail.Price = 0;
                                                sbOrderDetail.GST = 0;
                                                sbOrderDetail.Total = 0;

                                                Logger.Warn("Trying to Create SB Order Line, OrderId {0}, ProductId {1}, Qty {2}", sbOrderId.Value, item.ProductId, item.ItemQty);

                                                ctx.SB_OrderDetails.InsertOnSubmit(sbOrderDetail);
                                                ctx.SubmitChanges();
                                            }
                                        }
                                        else
                                        {

                                            SB_OrderDetail sbOrderDetail = new SB_OrderDetail();
                                            sbOrderDetail.OrderID = sbOrderId.Value;
                                            sbOrderDetail.ProductID = item.ProductId;
                                            sbOrderDetail.Qty = item.ItemQty;
                                            sbOrderDetail.Details = string.Empty;
                                            sbOrderDetail.Price = 0;
                                            sbOrderDetail.GST = 0;
                                            sbOrderDetail.Total = 0;

                                            Logger.Warn("Trying to Create SB Order Line, OrderId {0}, ProductId {1}, Qty {2}", sbOrderId.Value, item.ProductId, item.ItemQty);

                                            ctx.SB_OrderDetails.InsertOnSubmit(sbOrderDetail);
                                            ctx.SubmitChanges();
                                        }
                                    }
                                }
                            }

                        }

                        //Add flag holder logic for Newcastle Regional Manager
                        if (nSBOrder.ManagerID == ManagerSettings.GarryPrince)
                        {
                            ClientsPref cpflag = (from c in ctx.ClientsPrefs
                                                  where c.ClientId == nSBOrder.ClientId && c.PrefID == ClientsPref.FlagHolderOnAllBoard
                                                  select c).FirstOrDefault();

                            if (cpflag != null && cpflag.BitValue.HasValue && cpflag.BitValue == true)
                            {
                                //check if it is lease board
                                var odBoard = (from od in ctx.SB_OrderDetails
                                               where od.OrderID == nSBOrder.SBOrderId && od.Product.TypeID == ProductTypes.Stockboard && od.Product.ContentType != "SB For Lease"
                                               select od).ToList();

                                var odFlagHolder = (from od in ctx.SB_OrderDetails
                                                    where od.OrderID == nSBOrder.SBOrderId && od.ProductID == ProductSettings.FlagHolder
                                                    select od).ToList();

                                if (odBoard != null && odBoard.Count > 0)
                                {
                                    int totalBoard = 0;
                                    int totalFlagHolder = 0;
                                    int diff = 0;

                                    totalBoard = odBoard.Count(od => od.Product.TypeID == ProductTypes.Stockboard);

                                    if (odFlagHolder != null && odFlagHolder.Count > 0)
                                    {
                                        totalFlagHolder = odFlagHolder.Count(od => od.ProductID == ProductSettings.FlagHolder);
                                    }

                                    if (totalBoard > totalFlagHolder)
                                    {
                                        diff = totalBoard - totalFlagHolder;

                                        if (odFlagHolder != null && odFlagHolder.Count > 0)
                                        {
                                            odFlagHolder[0].Qty = diff;
                                            odFlagHolder[0].Total = (odFlagHolder[0].Price + odFlagHolder[0].GST) * diff;
                                            ctx.SubmitChanges();
                                        }
                                        else
                                        {
                                            SB_OrderDetail sbOrderDetail = new SB_OrderDetail();
                                            sbOrderDetail.OrderID = nSBOrder.SBOrderId;
                                            sbOrderDetail.ProductID = ProductSettings.FlagHolder;
                                            sbOrderDetail.Qty = diff;
                                            sbOrderDetail.Details = string.Empty;
                                            sbOrderDetail.Price = 0;
                                            sbOrderDetail.GST = 0;
                                            sbOrderDetail.Total = 0;
                                        }
                                    }
                                }
                            }

                            //Add wings 1200 logic
                            if (nSBOrder.ClientId == ClientSettings.DowlingMedowie)
                            {
                                //check if it is lease board
                                var odBoard = (from od in ctx.SB_OrderDetails
                                               where od.OrderID == nSBOrder.SBOrderId && od.Product.TypeID == ProductTypes.Stockboard && od.Product.ContentType != "SB For Lease"
                                               select od).ToList();

                                var odWings1200 = (from od in ctx.SB_OrderDetails
                                                   where od.OrderID == nSBOrder.SBOrderId && od.ProductID == ProductSettings.Wings1200
                                                    select od).ToList();

                                if (odBoard != null && odBoard.Count > 0)
                                {
                                    int totalBoard = 0;
                                    int totalWings1200 = 0;
                                    int diff = 0;

                                    totalBoard = odBoard.Count(od => od.Product.TypeID == ProductTypes.Stockboard);

                                    if (odWings1200 != null && odWings1200.Count > 0)
                                    {
                                        totalWings1200 = odWings1200.Count(od => od.ProductID == ProductSettings.Wings1200);
                                    }

                                    if (totalBoard > totalWings1200)
                                    {
                                        diff = totalBoard - totalWings1200;

                                        if (odWings1200 != null && odWings1200.Count > 0)
                                        {
                                            odWings1200[0].Qty = diff;
                                            odWings1200[0].Total = (odWings1200[0].Price + odWings1200[0].GST) * diff;
                                            ctx.SubmitChanges();
                                        }
                                        else
                                        {
                                            SB_OrderDetail sbOrderDetail = new SB_OrderDetail();
                                            sbOrderDetail.OrderID = nSBOrder.SBOrderId;
                                            sbOrderDetail.ProductID = ProductSettings.Wings1200;
                                            sbOrderDetail.Qty = diff;
                                            sbOrderDetail.Details = string.Empty;
                                            sbOrderDetail.Price = 0;
                                            sbOrderDetail.GST = 0;
                                            sbOrderDetail.Total = 0;
                                        }
                                    }
                                }
                            }
                        }

                        //Add flag holder logic For Con Costi Regional Manager
                        if (nSBOrder.ManagerID == ManagerSettings.ConCostiWollongong)
                        {
                            if(hasFlagHolder)
                            {
                                var sborder = (from o in ctx.SB_Orders
                                               where o.OrderID == nSBOrder.SBOrderId
                                               select o).FirstOrDefault();
                                if (sborder != null)
                                {
                                    sborder.Notes += "\r\n----------------------\r\nBoards Accessory: Please include flag holder";
                                    //order.ErectionNotes += " -- Boards Accessory: Please include flag holder";
                                    ctx.SubmitChanges();
                                }
                            }
                            else
                            {
                                ClientsPref cpflag = (from c in ctx.ClientsPrefs
                                                      where c.ClientId == nSBOrder.ClientId && c.PrefID == ClientsPref.FlagHolderOnAllBoard
                                                      select c).FirstOrDefault();

                                if (cpflag != null && cpflag.BitValue.HasValue && cpflag.BitValue == true)
                                {
                                    //check if it is lease board
                                    var odBoard = (from od in ctx.SB_OrderDetails
                                                   where od.OrderID == nSBOrder.SBOrderId && od.Product.TypeID == ProductTypes.Stockboard
                                                   select od).ToList();

                                    if (odBoard != null && odBoard.Count > 0)
                                    {
                                        var sborder = (from o in ctx.SB_Orders
                                                       where o.OrderID == nSBOrder.SBOrderId
                                                       select o).FirstOrDefault();
                                        if (sborder != null)
                                        {
                                            sborder.Notes += "\r\n----------------------\r\nBoards Accessory: Please include flag holder";
                                            //order.ErectionNotes += " -- Boards Accessory: Please include flag holder";
                                            ctx.SubmitChanges();
                                        }
                                    }
                                }
                            }
                        }

                        ctx.Transaction.Commit();

                        return nSBOrder.SBOrderId;
                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        throw ex;
                    }
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CreateNewSBOrder'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GenerateNewSBOrderEvent
        public static void GenerateNewSBOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nSBOrderEvent");
            }
            try
            {
                bool isAutomateSB = false;
                if (nOrderEvent.ManagerID == ManagerSettings.WorkshopVictoria)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        //TimeSpan start = new TimeSpan(1, 0, 0); //7 o'clock
                        //TimeSpan end = new TimeSpan(23, 59, 0); //16:30 o'clock
                        TimeSpan now = DateTime.Now.TimeOfDay;

                        //if ((nOrderEvent.OrderType != OrderType.B2B) && !IsWeekend(DateTime.Now) && (now > start) && (now < end))
                        if ((nOrderEvent.OrderType != OrderType.B2B) && !IsWeekend(DateTime.Now) && nOrderEvent.GroupId != ClientGroupSettings.OxbridgePropertyGroup)
                        {
                            if (nOrderEvent.OrderHasStockboard && nOrderEvent.OrderId < 99000000 && !nOrderEvent.OrderHasOverlayExcludeUnitStickerAndNamePlates && nOrderEvent.OrderId > 0)
                            {
                                try
                                {
                                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                                    loadOptions.Add(EntityRelations.Order_To_Client);
                                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                                    ctx.DeferredLoadingEnabled = false;
                                    ctx.SetDataLoadOptions(loadOptions);

                                    ClientsPref cp = (from c in ctx.ClientsPrefs
                                                      where c.ClientId == nOrderEvent.ClientId && c.PrefID == ClientsPref.AllowStockBoardInventory
                                                      select c).FirstOrDefault();

                                    if (cp != null && cp.BitValue.HasValue && cp.BitValue == true)
                                    {
                                        Logger.Warn("VIC Stockboard Order Check Inventory: " + nOrderEvent.OrderId);

                                        Order order = ctx.Orders.SingleOrDefault(c => c.OrderID == nOrderEvent.OrderId);

                                        if (string.IsNullOrEmpty(nOrderEvent.Notes) && order.OrderStatus <= 0)
                                        {
                                            isAutomateSB = true;

                                            string captionText = order.Caption + "";

                                            order.Caption = order.Caption + "#";
                                            var result = ctx.ACC_GetStockOrderDetails(nOrderEvent.OrderId);

                                            MaterialDetail md = ctx.MaterialDetails.SingleOrDefault(c => c.OrderID == nOrderEvent.OrderId);

                                            md.TextReceived = DateTime.Now;

                                            if (result != null)
                                            {
                                                bool usePrint = false;
                                                string xmlDataItems = string.Empty;
                                                foreach (var item in result)
                                                {
                                                    if (item.QtyRes > item.QtyHand)
                                                    {
                                                        ///call SP to IncreaseStock
                                                        ctx.ACC_InventoryIncrease(item.OrderDetailsID);
                                                        usePrint = true;
                                                    }
                                                    xmlDataItems = @"<ITEM>
                                           <ITEMNAME>" + item.Name.Replace("&", "&amp;") + @"</ITEMNAME>
                                           <QTY>" + item.Qty + @"</QTY>
                                           <QTYINHAND>" + item.QtyHand + @"</QTYINHAND>
                                           <QTYRES>" + item.QtyRes + @"</QTYRES>";

                                                    if (item.QtyRes > item.QtyHand)
                                                        xmlDataItems += @"<DECISION>1</DECISION>";
                                                    else
                                                        xmlDataItems += @"<DECISION>0</DECISION>";
                                                    xmlDataItems += @"</ITEM>";
                                                }

                                                //send email notification to manager
                                                int eventID = EventSettings.StockboardProgressToManagers;
                                                string sub = "Stockboard Order Progress";
                                                string xmlData = string.Empty;

                                                xmlData = @"<EVENT>
                                                <ORDERID>" + nOrderEvent.OrderId + @"</ORDERID>
                                                <AGENT>" + order.Client.ClientName.Replace("&", "&amp;") + "/" + order.Client.Office + @"</AGENT>
                                                <PADDRESS>" + nOrderEvent.Prop.Replace("&", "&amp;") + @"</PADDRESS>
                                                <CAPTION>" + captionText.Replace("&", "&amp;") + @"</CAPTION>
                                                <DATERECEIVED>" + DateTime.Now.ToString() + @"</DATERECEIVED>
                                                <ITEMS>" + xmlDataItems + @"</ITEMS>
                                                </EVENT>";

                                                string textData = xmlData;
                                                string source = "ProcessOrder";

                                                ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, nOrderEvent.OrderId, null, order.ManagerID, null, source, String.Empty);
                                                ctx.SubmitChanges();

                                                if (usePrint)
                                                {
                                                    ctx.ACC_ApproveOnly(nOrderEvent.OrderId);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        foreach (OrderDetail detail in order.OrderDetails)
                                                        {
                                                            if (detail.Product.TypeID == ProductTypes.Stockboard)
                                                            {
                                                                detail.ItemNote += "DONT PRINT STOCKBOARD - USE FROM STOCK";
                                                            }
                                                        }
                                                        ctx.SubmitChanges();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Logger.Exception(ex, "Can not update item note");
                                                    }
                                                    //Approve only
                                                    if (order.ManagerID == ManagerSettings.WorkshopVictoria)
                                                    {

                                                        ctx.ABCWRKFLOW_ApproveOrder(nOrderEvent.OrderId, DateTime.Now, null, null, null, false);

                                                        if (nOrderEvent.SendJobsheetToPrintTeam)
                                                        {
                                                            ctx.FrontOffice_ServiceQueueAdd("PrintJobsheet", "OrderID=" + nOrderEvent.OrderId, 0, 2, null, @"\\adbb\stockboarddepot", null, null, "Online Order Processor", 1);
                                                        }
                                                        else
                                                        {
                                                            Logger.Warn("Stockboard Order: " + nOrderEvent.OrderId);
                                                            try
                                                            {
                                                                BoardApproval ba = new BoardApproval();
                                                                ba.OrderID = nOrderEvent.OrderId;
                                                                ba.Done = false;
                                                                ba.BoardApprovedDate = DateTime.Now;
                                                                ba.SBOrder = true;

                                                                ctx.BoardApprovals.InsertOnSubmit(ba);
                                                                ctx.SubmitChanges();
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                Logger.Exception(ex, "Can not add to board approval");
                                                            }
                                                        }
                                                        ctx.SubmitChanges();
                                                    }
                                                    else
                                                    {
                                                        ctx.ACC_ApproveAndDespatch(nOrderEvent.OrderId);
                                                    }
                                                }

                                                ctx.SubmitChanges();

                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = string.Format("Error occured in 'validating SB Inventory'. orderID:{0}", nOrderEvent.OrderId);
                                    Logger.Exception(ex, message);
                                }
                            }
                        }

                        if (isAutomateSB)
                        {
                            //Create SB event with new subject line 

                            string subject = "Automate Stockboard Order Received: " + nOrderEvent.Prop + " - Job No: " + nOrderEvent.OrderId.ToString();

                            ctx.SP_EventQueueAdd(EventSettings.AutomateStockboardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateNewSBOrderEvent", nOrderEvent.FileName);
                        }
                        else
                        {
                            //Create new order event
                            ctx.CDAS_GenNewStockOrderEvent(nOrderEvent.OrderId, nOrderEvent.ClientId,
                                                    nOrderEvent.HtmlBody, nOrderEvent.HtmlBodyUS,
                                                    nOrderEvent.FileName, nOrderEvent.Prop);
                        }

                    }
                }
                else
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        TimeSpan start = new TimeSpan(7, 0, 0); //7 o'clock
                        TimeSpan end = new TimeSpan(16, 30, 0); //16:30 o'clock
                        TimeSpan now = DateTime.Now.TimeOfDay;

                        if (nOrderEvent.ManagerID != ManagerSettings.WorkshopWesternAustralia && (nOrderEvent.OrderType != OrderType.B2B) && nOrderEvent.GroupId != ClientGroupSettings.OxbridgePropertyGroup)
                        {
                            if (nOrderEvent.OrderHasStockboard && nOrderEvent.OrderId < 99000000 && !nOrderEvent.InterstateOrderHasOverlayExcludeUnitStickerAndNamePlates && nOrderEvent.OrderId > 0)
                            {
                                try
                                {

                                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                                    loadOptions.Add(EntityRelations.Order_To_Client);
                                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                                    ctx.DeferredLoadingEnabled = false;
                                    ctx.SetDataLoadOptions(loadOptions);

                                    ClientsPref cp = (from c in ctx.ClientsPrefs
                                                      where c.ClientId == nOrderEvent.ClientId && c.PrefID == ClientsPref.AllowStockBoardInventory
                                                      select c).FirstOrDefault();

                                    if (cp != null && cp.BitValue.HasValue && cp.BitValue == true)
                                    {
                                        Logger.Warn("Interstate Stockboard Order Check Inventory: " + nOrderEvent.OrderId);

                                        Order order = ctx.Orders.SingleOrDefault(c => c.OrderID == nOrderEvent.OrderId);

                                        if (string.IsNullOrEmpty(nOrderEvent.Notes) && order.OrderStatus <= 0)
                                        {
                                            isAutomateSB = true;

                                            string captionText = order.Caption + "";

                                            if(!string.IsNullOrWhiteSpace(order.Caption))
                                            {
                                                if (order.Caption.Length > 150)
                                                {
                                                    order.Caption = order.Caption.Substring(0, 150) + "...";
                                                }
                                            }

                                            order.Caption = order.Caption + "#";
                                            var result = ctx.ACC_GetStockOrderDetails(nOrderEvent.OrderId);

                                            MaterialDetail md = ctx.MaterialDetails.SingleOrDefault(c => c.OrderID == nOrderEvent.OrderId);

                                            md.TextReceived = DateTime.Now;

                                            if (result != null)
                                            {
                                                bool usePrint = false;
                                                string xmlDataItems = string.Empty;
                                                foreach (var item in result)
                                                {
                                                    if (item.QtyRes > item.QtyHand)
                                                    {
                                                        ///call SP to IncreaseStock
                                                        ctx.ACC_InventoryIncrease(item.OrderDetailsID);
                                                        usePrint = true;
                                                    }
                                                    xmlDataItems = @"<ITEM>
                                           <ITEMNAME>" + item.Name.Replace("&", "&amp;") + @"</ITEMNAME>
                                           <QTY>" + item.Qty + @"</QTY>
                                           <QTYINHAND>" + item.QtyHand + @"</QTYINHAND>
                                           <QTYRES>" + item.QtyRes + @"</QTYRES>";

                                                    if (item.QtyRes > item.QtyHand)
                                                        xmlDataItems += @"<DECISION>1</DECISION>";
                                                    else
                                                        xmlDataItems += @"<DECISION>0</DECISION>";
                                                    xmlDataItems += @"</ITEM>";
                                                }

                                                //send email notification to manager
                                                int eventID = EventSettings.StockboardProgressToManagers;
                                                string sub = "Stockboard Order Progress";
                                                string xmlData = string.Empty;

                                                xmlData = @"<EVENT>
                                                <ORDERID>" + nOrderEvent.OrderId + @"</ORDERID>
                                                <AGENT>" + order.Client.ClientName.Replace("&", "&amp;") + "/" + order.Client.Office.Replace("&", "&amp;") + @"</AGENT>
                                                <PADDRESS>" + nOrderEvent.Prop.Replace("&", "&amp;") + @"</PADDRESS>
                                                <CAPTION>" + captionText.Replace("&", "&amp;") + @"</CAPTION>
                                                <DATERECEIVED>" + DateTime.Now.ToString() + @"</DATERECEIVED>
                                                <ITEMS>" + xmlDataItems + @"</ITEMS>
                                                </EVENT>";

                                                string textData = xmlData;
                                                string source = "ProcessOrder";

                                                ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, nOrderEvent.OrderId, null, order.ManagerID, null, source, String.Empty);
                                                ctx.SubmitChanges();

                                                if (usePrint)
                                                {
                                                    ctx.ACC_ApproveOnly(nOrderEvent.OrderId);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        foreach (OrderDetail detail in order.OrderDetails)
                                                        {
                                                            if (detail.Product.TypeID == ProductTypes.Stockboard)
                                                            {
                                                                detail.ItemNote += "DONT PRINT STOCKBOARD - USE FROM STOCK";
                                                            }
                                                        }
                                                        ctx.SubmitChanges();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Logger.Exception(ex, "Can not update item note");
                                                    }
                                                    //Approve only
                                                    if (order.ManagerID == ManagerSettings.WorkshopVictoria)
                                                    {
                                                        //ctx.ACC_ApproveOnly(nOrderEvent.OrderId);

                                                        ctx.ABCWRKFLOW_ApproveOrder(nOrderEvent.OrderId, DateTime.Now, null, null, null, false);

                                                        ctx.FrontOffice_ServiceQueueAdd("PrintJobsheet", "OrderID=" + nOrderEvent.OrderId, 0, 2, null, @"\\adbb\stockboarddepot", null, null, "Online Order Processor", 1);
                                                        ctx.SubmitChanges();
                                                    }
                                                    else
                                                    {
                                                        //check order has any other product other than just stockboard
                                                        if (nOrderEvent.InterstateOrderHasOverlayOrUnitStickerOrNamePlates)
                                                        {
                                                            try
                                                            {
                                                                ctx.FrontOffice_ServiceQueueAdd("PrintJobsheet", "OrderID=" + nOrderEvent.OrderId, 0, 2, null, @"\\adbb\HP Boards 02", null, null, "Online Order Processor", 1);
                                                                Logger.Warn("Sending interstate jobsheet to despatch: " + nOrderEvent.OrderId);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                string message = string.Format("Error occured in 'sending interstate jobsheet to despatch '. orderID:{0}", nOrderEvent.OrderId);
                                                                Logger.Exception(ex, message);
                                                            }
                                                        }
                                                        ctx.ACC_ApproveAndDespatch(nOrderEvent.OrderId);
                                                    }
                                                }

                                                ctx.SubmitChanges();

                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = string.Format("Error occured in 'validating SB Inventory'. orderID:{0}", nOrderEvent.OrderId);
                                    Logger.Exception(ex, message);
                                }
                            }
                        }

                        if (isAutomateSB)
                        {
                            //Create SB event with new subject line 

                            string subject = "Automate Stockboard Order Received: " + nOrderEvent.Prop + " - Job No: " + nOrderEvent.OrderId.ToString();

                            ctx.SP_EventQueueAdd(EventSettings.AutomateStockboardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateNewSBOrderEvent", nOrderEvent.FileName);
                        }
                        else
                        {
                            try
                            {
                                if (nOrderEvent.OrderId > 99000000 && System.IO.File.Exists(nOrderEvent.InstallFile))
                                {
                                    string fileExt = Path.GetExtension(nOrderEvent.InstallFile);
                                    string fName = nOrderEvent.OrderId.ToString() + "_InstallFile" + fileExt;
                                    string finalMove = System.IO.Path.Combine(OnlineBLConfig.INSTALLATION_FILE_PATH, fName);
                                    File.Move(nOrderEvent.InstallFile, finalMove);
                                    nOrderEvent.FileName += ";" + finalMove;

                                    SB_Order or = (from o in ctx.SB_Orders
                                                   where o.OrderID == nOrderEvent.OrderId
                                                   select o).FirstOrDefault();

                                    or.InstallFile = finalMove;

                                    ctx.SubmitChanges();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "Can not move the file");
                            }
                            
                            //Create new order event
                            ctx.CDAS_GenNewStockOrderEvent(nOrderEvent.OrderId, nOrderEvent.ClientId,
                                                    nOrderEvent.HtmlBody, nOrderEvent.HtmlBodyUS,
                                                    nOrderEvent.FileName, nOrderEvent.Prop);
                            
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewSBOrderEvent'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        public static bool IsWeekend(DateTime date)
        {
            return ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday));
        }

        public static bool IsHoliday()
        {
            DateTime startHoliday = new DateTime(2018, 12, 20, 16, 30, 0);

            return (startHoliday < DateTime.Now);
        }
        #endregion

        #region private methods
        private static void InsertProducts(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, string managerId, int orderId, AbcDataContext ctx, bool isWorkshop)
        {
            //if client has sb diy preferences
            bool clientHasStockboardDIY = false;
            ClientsPref cpHasSBDIY = (from c in ctx.ClientsPrefs
                                      where c.ClientId == propertyOrder.ClientId && c.PrefID == Abc.OnlineBL.Entities.Utility.Preferences.StockboardDIY
                              select c).FirstOrDefault();

            if (cpHasSBDIY != null && cpHasSBDIY.BitValue.HasValue && cpHasSBDIY.BitValue == true)
                clientHasStockboardDIY = true;

            foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item in propertyOrder.Cart)
            {
                var pro = ctx.Products.FirstOrDefault(p => p.ProductID == item.ProductId);
                bool usePackageContentPrice = false;
                if (pro != null)
                {
                    usePackageContentPrice = pro.UsePackageContentPrice;
                }

                decimal solarPanelPrice = 200;
                var prlid = (from c in ctx.Clients
                             join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                             where c.ClientID == propertyOrder.ClientId
                             && i.ProductID == OnlineBLConfig.SOLAR_PANEL_PRODUCT_ID
                             select i).FirstOrDefault();
                if (prlid != null && prlid.ProductPrice >= 0)
                {
                    solarPanelPrice = prlid.ProductPrice * new decimal(1.1);
                }

                #region ModularPackage
                if (item.TypeId == ProductTypes.Packages || item.TypeId == ProductTypes.BoardPackages || item.TypeId == ProductTypes.OtherPackages)
                {
                    List<UpgradeProduct> upgradeProductList = new List<UpgradeProduct>();
                    List<PackageContentProduct> packLightBoards = new List<PackageContentProduct>();

                    //just add product as normal
                    int orderDetailID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                            item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, null, null, usePackageContentPrice);


                    //then retreive the orderdetailID and pass it on as parent id
                    foreach (PackageGroup itemGroup in item.PackageGroups)
                    {
                        if (itemGroup.IsUpgradeProductApplicable)
                        {
                            upgradeProductList.Add(itemGroup.UpgradedProduct);
                            itemGroup.UpgradedProduct.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, itemGroup.UpgradedProduct.UpgradeProductID, 1,
                                            itemGroup.UpgradedProduct.FindFormat(propertyOrder.IsDIYOrder), itemGroup.UpgradedProduct.ProductHasDIY(), orderId, itemGroup.UpgradedProduct.ItemNotes + " Upgraded Product", orderDetailID, usePackageContentPrice);
                        }
                        else
                        {
                            foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                            {
                                if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                                {
                                    bool userDesignOnline = false;
                                    if (contentProductItem.ProductHasDIY() && propertyOrder.IsDIYOrder)
                                    {
                                        userDesignOnline = true;
                                    }
                                    contentProductItem.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, contentProductItem.ProductId, contentProductItem.PkgQty,
                                            contentProductItem.FindFormat(propertyOrder.IsDIYOrder), userDesignOnline, orderId, contentProductItem.ItemNotes, orderDetailID, usePackageContentPrice);

                                    if ((contentProductItem.TypeId == ProductTypes.BillBoard) &&
                                        !string.IsNullOrEmpty(contentProductItem.FrameType) && contentProductItem.FrameType.ToLower() == "light board")
                                    {
                                        packLightBoards.Add(contentProductItem);
                                    }
                                }
                                if (contentProductItem.TypeId == ProductTypes.Stockboard && contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                                {
                                    //Send an email out
                                    StringBuilder sb = new StringBuilder();

                                    sb.Append("-------------------Product Content-------------------------<br/>");

                                    if (contentProductItem.ProductId > 0)
                                    {
                                        sb.Append("***** Product Id: " + contentProductItem.ProductId + "<br/>");
                                    }

                                    if (!string.IsNullOrEmpty(contentProductItem.ProductName))
                                    {
                                        sb.Append("***** Product Name: " + contentProductItem.ProductName + "<br/>");
                                    }

                                    if (contentProductItem.ProductConfig != null)
                                    {
                                        foreach (var field in contentProductItem.ProductConfig.Fields.Field)
                                        {
                                            sb.Append("***** " + field.Caption + " - " + field.Value + "<br/>");
                                        }
                                    }

                                    if (clientHasStockboardDIY)
                                    {
                                        if (!propertyOrder.IsDIYOrder)
                                            SendStockBoardInPackEmail(ctx, "StockBoard Order within Package.  BREAK UP ORDER - " + orderId.ToString(), sb.ToString());
                                    }
                                    else
                                        SendStockBoardInPackEmail(ctx, "StockBoard Order within Package.  BREAK UP ORDER - " + orderId.ToString(), sb.ToString());

                                }
                            }
                        }
                    }

                    //Check to see if there is any upgrade product, then add additional item to OrderDetails
                    if (upgradeProductList.Count > 0)
                    {
                        foreach (var ugItem in upgradeProductList)
                        {
                            //live product 7398
                            //test product 20015
                            InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, WorkflowConfig.UPGRADE_PRODUCT_ID, 1,
                                    null, propertyOrder.IsDIYOrder, orderId, "Upgraded Product", null, false, (decimal)(ugItem.UpgradePrice / (decimal)1.1));

                            //Check if item light board then save new row 
                            if (ugItem.FrameType.Contains("Light") && ugItem.ProductConfig != null)
                            {
                                if (ugItem.ProductConfig.Fields != null && ugItem.ProductConfig.Fields.Field != null && ugItem.ProductConfig.Fields.Field.Count > 0)
                                {
                                    int solarPanel = 0;
                                    var solarPanelConfigField = ugItem.ProductConfig.Fields.Field.Where(f => f.FieldName.ToLower().Contains("solarpanel")).FirstOrDefault();
                                    if (solarPanelConfigField != null && !string.IsNullOrEmpty(solarPanelConfigField.Value) && int.TryParse(solarPanelConfigField.Value, out solarPanel))
                                    {
                                        //solarPanel product test 19743 
                                        //solarPanel produce live 9065
                                        if (solarPanel == 1)
                                        {
                                            InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, OnlineBLConfig.SOLAR_PANEL_PRODUCT_ID, item.ItemQty,
                                            item.FindFormat(propertyOrder.IsDIYOrder), false, orderId, "Solar Panel with Light Board", null, false, (decimal)solarPanelPrice / (decimal)1.1);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Check to see if there is any light board product, then add additional item to OrderDetails
                    if (packLightBoards.Count > 0)
                    {
                        foreach (var lightBoardItem in packLightBoards)
                        {
                            //Check if item light board then save new row 
                            if (lightBoardItem.FrameType.Contains("Light") && lightBoardItem.ProductConfig != null)
                            {
                                if (lightBoardItem.ProductConfig.Fields != null && lightBoardItem.ProductConfig.Fields.Field != null && lightBoardItem.ProductConfig.Fields.Field.Count > 0)
                                {
                                    int solarPanel = 0;
                                    var solarPanelConfigField = lightBoardItem.ProductConfig.Fields.Field.Where(f => f.FieldName.ToLower().Contains("solarpanel")).FirstOrDefault();
                                    if (solarPanelConfigField != null && !string.IsNullOrEmpty(solarPanelConfigField.Value) && int.TryParse(solarPanelConfigField.Value, out solarPanel))
                                    {
                                        //solarPanel product test 19743 
                                        //solarPanel produce live 9065
                                        if (solarPanel == 1)
                                        {
                                            InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, OnlineBLConfig.SOLAR_PANEL_PRODUCT_ID, item.ItemQty,
                                            item.FindFormat(propertyOrder.IsDIYOrder), false, orderId, "Solar Panel with Light Board", null, false, (decimal)solarPanelPrice / (decimal)1.1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Board
                else if (item.TypeId == ProductTypes.BillBoard)
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            item.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, null, null, false);
                        }
                        if (!string.IsNullOrEmpty(item.FrameType) && item.FrameType.ToLower() == "light board")
                        {
                            if (item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
                            {
                                int solarPanel = 0;
                                if (!string.IsNullOrEmpty(item.ProductConfig.Fields.Field[2].Value) && int.TryParse(item.ProductConfig.Fields.Field[2].Value, out solarPanel))
                                {
                                    //solarPanel product test 19743 
                                    //solarPanel produce live 9065
                                    if (solarPanel == 1)
                                    {
                                        InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, OnlineBLConfig.SOLAR_PANEL_PRODUCT_ID, item.ItemQty,
                                        item.FindFormat(propertyOrder.IsDIYOrder), false, orderId, "Solar Panel with Light Board", null, false, (decimal)solarPanelPrice / (decimal)1.1);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("OrderID{0}\r\n TypeID{1}\r\n ProductID{2}", orderId, item.TypeId, item.ProductId));
                    }
                }
                #endregion

                #region Stockboard
                else if (item.TypeId == ProductTypes.Stockboard && propertyOrder.IsDIYOrder)
                {
                    if (clientHasStockboardDIY)
                    {
                        try
                        {
                            if (item.ProductId > 0)
                            {
                                item.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                        item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, null, null, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, string.Format("OrderID{0}\r\n TypeID{1}\r\n ProductID{2}", orderId, item.TypeId, item.ProductId));
                        }
                    }
                }
                #endregion

                #region Stockboard Overlay
                else if (item.TypeId == ProductTypes.StockboardOverlay)
                {
                    if (clientHasStockboardDIY)
                    {
                        try
                        {
                            if (item.ProductId > 0)
                            {
                                item.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                        item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, null, null, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, string.Format("OrderID{0}\r\n TypeID{1}\r\n ProductID{2}", orderId, item.TypeId, item.ProductId));
                        }
                    }
                    else
                    {
                        if (propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) ||
                               (!propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) && !propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.Stockboard)))
                        {
                            try
                            {
                                if (item.ProductId > 0)
                                {
                                    item.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                            item.FindFormat(propertyOrder.IsDIYOrder), false, orderId, null, null, false);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, string.Format("OrderID{0}\r\n TypeID{1}\r\n ProductID{2}", orderId, item.TypeId, item.ProductId));
                            }
                        }
                    }
                }
                #endregion

                #region Brochure WindowCard
                else if (item.TypeId == ProductTypes.Brochure || item.TypeId == ProductTypes.WindowCard || item.TypeId == ProductTypes.DIYStickers)
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            item.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, null, null, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("OrderID{0}\r\n TypeID{1}\r\n ProductID{2}", orderId, item.TypeId, item.ProductId));
                    }
                }
                #endregion

                #region Corflute
                else if (item.TypeId == ProductTypes.Corflute)
                {
                    StringBuilder sb = new StringBuilder();
                    if (item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
                    {
                        foreach (var field in item.ProductConfig.Fields.Field)
                        {
                            if (!string.IsNullOrEmpty(field.Value))
                                sb.Append(field.Caption + ": " + field.Value.Replace("\n", " ").Replace("\r\n", " ") + " -- ");
                        }
                    }

                    try
                    {
                        if (item.ProductId > 0)
                        {
                            if (propertyOrder.ClientId == ClientSettings.BuyMyPlaceSouthMelbourne)
                            {
                                item.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, sb.ToString(), null, false);
                            }
                            else
                            {
                                try
                                {
                                    SyncProduct(item);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, string.Format("OrderID {0}\r\n CategoryName {1}\r\n ProductID {2}", orderId, item.CategoryName, item.ProductId));
                                }
                                Logger.Warn("Product: " + item.ProductId + " -- " + item.ProductPrice);
                                item.OrderDetailsID = InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                        item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, sb.ToString(), null, false, (decimal)(item.ProductPrice / (decimal)1.1));
                            }
                        }
                        if (item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
                        {
                            int eyelet = 0;
                            if (!string.IsNullOrEmpty(item.ProductConfig.Fields.Field[2].Value) && int.TryParse(item.ProductConfig.Fields.Field[2].Value, out eyelet))
                            {
                                if (eyelet == 4)
                                {
                                    item.OrderDetailsID = InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, 1521, item.ItemQty * 4,
                                    item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, "An eyelet in each corner", null, false, (decimal)0.25 / (decimal)1.1);
                                }
                                else if (eyelet == 6)
                                {
                                    item.OrderDetailsID = InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, 1521, item.ItemQty * 6,
                                    item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, "3 eyelets down each side", null, false, (decimal)0.25 / (decimal)1.1);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("OrderID{0}\r\n TypeID{1}\r\n ProductID{2}", orderId, item.TypeId, item.ProductId));
                    }
                }
                #endregion

                #region ErectionFee
                else if (item.TypeId == ProductTypes.BoardAccessory && item.ProductName.Contains("High Installation"))
                {
                    if (item.ProductId > 0)
                    {
                        try
                        {
                            if (propertyOrder.BoardInstallationType == BoardInstallationType.High)
                            {
                                bool isVicSign = false;
                                bool isVicClient = false;
                                try
                                {
                                    var pl = (from c in ctx.Clients
                                              join i in ctx.PriceLists on c.PriceListID equals i.PriceListID
                                              where c.ClientID == propertyOrder.ClientId
                                              select i).FirstOrDefault();
                                    if (pl != null && pl.PriceListID == 84)
                                    {
                                        isVicSign = true;
                                    }
                                }
                                catch (Exception)
                                {

                                }
                                if (isVicSign)
                                {
                                    InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, (decimal)150.00, orderId, managerId, propertyOrder);
                                }
                                else
                                {

                                    var client = (from c in ctx.Clients
                                                  where c.ClientID == propertyOrder.ClientId
                                                  select c).FirstOrDefault();

                                    if (client != null && !string.IsNullOrEmpty(client.State) && client.State.ToUpper() == "VIC")
                                    {
                                        isVicClient = true;
                                    }

                                    if (isVicClient)
                                    {
                                        var pld = (from c in ctx.Clients
                                                   join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                   where c.ClientID == propertyOrder.ClientId
                                                   && i.ProductID == item.ProductId
                                                   select i).FirstOrDefault();

                                        //Product exist on Price List 
                                        if (pld != null && pld.ProductPrice > 0)
                                        {
                                            //Need to see how many board in order
                                            int total = 0;
                                            if (propertyOrder.OrderHasBoard())
                                            {
                                                total = 1;
                                            }
                                            else if(propertyOrder.OrderHasStockboardIncludePackageCheck())
                                            {
                                                total = 1;
                                            }

                                            decimal totalPrice = total * pld.ProductPrice;
                                            InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, managerId, propertyOrder);
                                        }
                                        else //Product not exist on price list - fetch from product Pricing Type
                                        {
                                            var priceList = (from c in ctx.Clients
                                                       join i in ctx.PriceLists on c.PriceListID equals i.PriceListID
                                                       where c.ClientID == propertyOrder.ClientId
                                                       select i).FirstOrDefault();

                                            if(priceList != null)
                                            {
                                                var productp = (from pp in ctx.ProductPricings 
                                                           where pp.PricingID == priceList.PricingID
                                                           && pp.ProductID == item.ProductId
                                                           select pp).FirstOrDefault();

                                                if(productp != null)
                                                {
                                                    int total = 0;
                                                    if (propertyOrder.OrderHasBoard())
                                                    {
                                                        total = 1;
                                                    }
                                                    else if (propertyOrder.OrderHasStockboardIncludePackageCheck())
                                                    {
                                                        total = 1;
                                                    }

                                                    decimal totalPrice = total * productp.Price;
                                                    InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, managerId, propertyOrder);
                                                }
                                                else
                                                {
                                                    int total = 0;
                                                    if (propertyOrder.OrderHasBoard())
                                                    {
                                                        total = 1;
                                                    }
                                                    else if (propertyOrder.OrderHasStockboardIncludePackageCheck())
                                                    {
                                                        total = 1;
                                                    }

                                                    decimal totalPrice = total * 150;
                                                    InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, managerId, propertyOrder);
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {

                                        var pld = (from c in ctx.Clients
                                                   join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                   where c.ClientID == propertyOrder.ClientId
                                                   && i.ProductID == item.ProductId
                                                   select i).FirstOrDefault();
                                        if (pld != null && pld.ProductPrice > 0)
                                        {
                                            //Need to see how many board in order
                                            int total = 0;
                                            if (propertyOrder.OrderHasBoard())
                                            {
                                                total = 1;
                                            }
                                            else if (propertyOrder.OrderHasStockboardIncludePackageCheck())
                                            {
                                                total = 1;
                                            }

                                            decimal totalPrice = total * pld.ProductPrice;
                                            InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, managerId, propertyOrder);
                                        }
                                        else
                                        {
                                            var priceList = (from c in ctx.Clients
                                                             join i in ctx.PriceLists on c.PriceListID equals i.PriceListID
                                                             where c.ClientID == propertyOrder.ClientId
                                                             select i).FirstOrDefault();

                                            if (priceList != null)
                                            {
                                                var productp = (from pp in ctx.ProductPricings
                                                                where pp.PricingID == priceList.PricingID
                                                                && pp.ProductID == item.ProductId
                                                                select pp).FirstOrDefault();

                                                if (productp != null)
                                                {
                                                    int total = 0;
                                                    if (propertyOrder.OrderHasBoard())
                                                    {
                                                        total = 1;
                                                    }
                                                    else if (propertyOrder.OrderHasStockboardIncludePackageCheck())
                                                    {
                                                        total = 1;
                                                    }

                                                    decimal totalPrice = total * productp.Price;
                                                    InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, managerId, propertyOrder);
                                                }
                                                else
                                                {
                                                    int total = 0;
                                                    if (propertyOrder.OrderHasBoard())
                                                    {
                                                        total = 1;
                                                    }
                                                    else if (propertyOrder.OrderHasStockboardIncludePackageCheck())
                                                    {
                                                        total = 1;
                                                    }

                                                    decimal totalPrice = total * 150;
                                                    InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, managerId, propertyOrder);
                                                }
                                            }

                                        }
                                    }
                                }

                                //send an email

                                int eventID = EventSettings.HighInstallationProductNeedSetup;
                                string sub = "Abc Notification: Please Check High Installation To Make Sure - Order: " + orderId;
                                string xmlData = string.Empty;
                                xmlData = @"<EVENT>
									        <OrderID>" + orderId.ToString() + @"</OrderID>
									        <ClientID>" + propertyOrder.ClientId.ToString() + @"</ClientID>
									        <ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
									        </EVENT>";

                                string textData = "Abc Notification: Please Check High Installation To Make Sure - Order: " + orderId;
                                string source = "OnlineBL_Workflow_CreateNewOrder";

                                ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, orderId, null, null, null, source, String.Empty);
                            }
                            else if (propertyOrder.BoardInstallationType == BoardInstallationType.HigherThanFirstLevel)
                            {

                                InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, 0, orderId, managerId, propertyOrder);

                            }
                            else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed500mmOfTheGround)
                            {
                                InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 225, orderId, managerId, propertyOrder);
                            }
                            else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1000mmOfTheGround)
                            {
                                InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 325, orderId, managerId, propertyOrder);
                            }
                            else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1250mmOfTheGround)
                            {
                                InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 525, orderId, managerId, propertyOrder);
                            }
                            else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed2000mmOfTheGround)
                            {
                                InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 0, orderId, managerId, propertyOrder);
                            }

                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, "Erection Fee", item.ProductId });
                        }
                    }
                }
                #endregion

                #region TravelFee
                else if (item.TypeId == ProductTypes.BoardAccessory && item.ProductId == ProductSettings.Travel)
                {
                    if(propertyOrder.IsDIYOrder)
                    {
                        try
                        {
                            InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                string.Empty, false, orderId, null, null, false, item.ProductPrice);
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, ProductTypes.Other, item.ProductId });
                        }
                    }
                    else
                    {
                        if (propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) ||
                                (!propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) && !propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.Stockboard)))
                        {
                            try
                            {
                                InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    string.Empty, false, orderId, null, null, false, item.ProductPrice);
                            }
                            catch (System.Exception ex)
                            {
                                Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, ProductTypes.Other, item.ProductId });
                            }
                        }
                    }
                }
                #endregion

                #region BoardAccessory
                else if (item.TypeId == ProductTypes.BoardAccessory && !item.ProductName.Contains("High Installation") && item.ProductId != ProductSettings.Travel)
                {
                    if (clientHasStockboardDIY)
                    {
                        if (item.ProductId > 0)
                        {
                            try
                            {
                                //ticket 595
                                //for regional managers put board accessory to notes
                                if (!isWorkshop)
                                {
                                    var order = (from o in ctx.Orders
                                                 where o.OrderID == orderId
                                                 select o).FirstOrDefault();
                                    if (order != null)
                                    {
                                        order.Notes += "\r\n----------------------\r\nBoards Accessory: " + item.GetText();
                                        order.ErectionNotes += " -- Boards Accessory: " + item.GetText();
                                        ctx.SubmitChanges();
                                    }
                                }
                                else
                                {
                                    Logger.Info("ProductAccessory: " + item.ProductId + " -- " + orderId);

                                    if (item.ProductId > 0)
                                        InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                            item.FindFormat(false), false, orderId, null, null, false);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Logger.Exception(ex, string.Format("{0} {1}", orderId, item.ProductId ));
                            }
                        }
                    }
                    else
                    {
                        if (propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) ||
                                    (!propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) && !propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.Stockboard)))
                        {
                            if (item.ProductId > 0)
                            {
                                try
                                {
                                    //ticket 595
                                    //for regional managers put board accessory to notes
                                    if (!isWorkshop)
                                    {
                                        var order = (from o in ctx.Orders
                                                     where o.OrderID == orderId
                                                     select o).FirstOrDefault();
                                        if (order != null)
                                        {
                                            order.Notes += "\r\n----------------------\r\nBoards Accessory: " + item.GetText();
                                            order.ErectionNotes += " -- Boards Accessory: " + item.GetText();
                                            ctx.SubmitChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (item.ProductId > 0)
                                            InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                                item.FindFormat(false), false, orderId, null, null, false);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    Logger.Exception(ex, string.Format("{0} {1}", orderId, item.ProductId ));
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Overlay
                else if (item.TypeId == ProductTypes.Overlay)
                {
                    if (clientHasStockboardDIY)
                    {
                        if (item.ProductId > 0)
                        {
                            try
                            {
                                if (item.ProductId > 0)
                                    InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                        item.FindFormat(false), false, orderId, null, null, false);
                            }
                            catch (System.Exception ex)
                            {
                                Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, ProductTypes.Other, item.ProductId });
                            }
                        }
                    }
                    else
                    {
                        if (propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) ||
                                    (!propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard) && !propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.Stockboard)))
                        {
                            if (item.ProductId > 0)
                            {
                                try
                                {
                                    if (item.ProductId > 0)
                                        InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                            item.FindFormat(false), false, orderId, null, null, false);
                                }
                                catch (System.Exception ex)
                                {
                                    Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, ProductTypes.Other, item.ProductId });
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Spotlight
                else if (item.TypeId == ProductTypes.Other && item.ProductName.Contains("Spotlight"))
                {
                    try
                    {
                        if (item.ProductId > 0)
                            InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                item.FindFormat(false), false, orderId, null, null, false);
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, ProductTypes.Other, item.ProductId });
                    }
                }
                #endregion

                #region OtherBoardExtension
                else if (item.TypeId == ProductTypes.Other && (item.ProductId == WorkflowConfig.INSTALLATION_EXTENSION_STOCK_BOARD_PRODUCT_ID || item.ProductId == WorkflowConfig.INSTALLATION_EXTENSION_TEXT_PHOTO_BOARD_PRODUCT_ID))
                {
                    try
                    {
                        if (item.ProductId > 0)
                            item.OrderDetailsID = InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                item.FindFormat(false), false, orderId, null, null, false, (decimal)(item.ProductPrice / (decimal)1.1));
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, ProductTypes.Other, item.ProductId });
                    }
                }
                #endregion

                #region OtherFrame
                else if (item.ProductId == 20353) //item.TypeId == ProductTypes.Other
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            string color = string.Empty;

                            if (item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(item.ProductConfig.Fields.Field[0].Value))
                                {
                                    color = item.ProductConfig.Fields.Field[0].Value;
                                }
                            }

                            item.OrderDetailsID = InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                item.FindFormat(false), false, orderId, color, null, false, (decimal)(item.ProductPrice / (decimal)1.1));

                        }
                                
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, ProductTypes.Other, item.ProductId });
                    }
                }
                #endregion

                #region All Optional Item
                else
                {
                    if (item.TypeId != ProductTypes.Photography && item.TypeId != ProductTypes.FloorPlans && item.TypeId != ProductTypes.Stockboard
                        && item.TypeId != ProductTypes.BoardAccessory && item.TypeId != ProductTypes.Overlay)
                    {
                        try
                        {
                            if (item.ProductId > 0)
                            {
                                Logger.Info("Productid: " + item.ProductId);
                                InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                        item.FindFormat(propertyOrder.IsDIYOrder), false, orderId, null, null, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, string.Format("OrderID{0}\r\n TypeID{1}\r\n ProductID{2}", orderId, item.TypeId, item.ProductId));
                        }
                    }
                }
                #endregion
            }

            #region  Add artwork fee
            //if client has proof artwork preferences
            ClientsPref cp = (from c in ctx.ClientsPrefs
                              where c.ClientId == propertyOrder.ClientId && c.PrefID == ClientsPref.ClientNeedToProofArtwork
                              select c).FirstOrDefault();

            if (cp != null && cp.BitValue.HasValue && cp.BitValue == true)
            {
                if (propertyOrder.OrderHasBoardNotIncludingCommunityBoard() || propertyOrder.OrderHasBrochure())
                {
                    decimal proofArtworkPrice = 35;
                    var prlid = (from c in ctx.Clients
                                 join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                 where c.ClientID == propertyOrder.ClientId
                                 && i.ProductID == ProductSettings.Proof
                                 select i).FirstOrDefault();
                    if (prlid != null && prlid.ProductPrice >= 0)
                    {
                        proofArtworkPrice = prlid.ProductPrice;
                    }

                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderID = orderId;
                    orderDetail.ProductID = ProductSettings.Proof;
                    orderDetail.Qty = 1;
                    orderDetail.Price = proofArtworkPrice;
                    orderDetail.GST = proofArtworkPrice * new decimal(0.1);
                    orderDetail.Total = proofArtworkPrice * new decimal(1.1);

                    Logger.Warn("Trying to add artwork fee");

                    ctx.OrderDetails.InsertOnSubmit(orderDetail);
                    ctx.SubmitChanges();
                }
            }
            #endregion

            if (propertyOrder.OrderHasEnduroReSkinProduct())
            {

                try
                {
                    if (propertyOrder.MarketingDeliveryType == MarketingDeliveryType.PickupDropoffAframe)
                    {

                        try
                        {
                            decimal EnduroFrameDeliveryFee = (decimal)66.15;
                            
                            var prlid = (from c in ctx.Clients
                                         join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                         where c.ClientID == propertyOrder.ClientId
                                         && i.ProductID == ProductSettings.AFramereskinDeliveryFeePickupDropoff
                                         select i).FirstOrDefault();
                            if (prlid != null && prlid.ProductPrice >= 0)
                            {
                                EnduroFrameDeliveryFee = prlid.ProductPrice;
                            }

                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderID = orderId;
                            orderDetail.ProductID = ProductSettings.AFramereskinDeliveryFeePickupDropoff;
                            orderDetail.Qty = 1;
                            orderDetail.Price = EnduroFrameDeliveryFee;
                            orderDetail.GST = EnduroFrameDeliveryFee * new decimal(0.1);
                            orderDetail.Total = EnduroFrameDeliveryFee * new decimal(1.1);

                            Logger.Warn("Trying to add Enduro pick up and drop off fee");

                            ctx.OrderDetails.InsertOnSubmit(orderDetail);
                            ctx.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format("Error occured in 'add Enduro pick up and drop off fee'.");
                            Logger.Exception(ex, message);
                        }

                    }

                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'add Enduro pick up and drop off fee'.");
                    Logger.Exception(ex, message);
                }

            }
        }

        private static void InsertPhotographyProduct(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, int photoOrderId, AbcDataContext ctx)
        {
            int colourisedPlan = 0;
            int threeDFloorPlan = 0;
            int threeDPlusStandard = 0;
            int threeDPlusColourised = 0;
            int keyCollection = 0;

            #region GetProductPrice
            decimal threeDFloorPlanPrice = (decimal)54.54;
            try
            {
                var priceDetail = (from c in ctx.Clients
                                   join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                   where c.ClientID == propertyOrder.ClientId
                                   && i.ProductID == ProductSettings.ThreeDFloorPlan
                                   select i).FirstOrDefault();
                if (priceDetail != null && priceDetail.ProductPrice >= 0)
                {
                    threeDFloorPlanPrice = priceDetail.ProductPrice;
                }
            }
            catch (Exception ex)
            {

                Logger.Exception(ex, "InsertPhotographyProduct");
            }

            decimal colouriseFloorPlanPrice = (decimal)18.18;
            try
            {
                var priceDetail = (from c in ctx.Clients
                                   join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                   where c.ClientID == propertyOrder.ClientId
                                   && i.ProductID == ProductSettings.ColourisationOfFloorPlanOrSitePlanStandard
                                   select i).FirstOrDefault();
                if (priceDetail != null && priceDetail.ProductPrice >= 0)
                {
                    colouriseFloorPlanPrice = priceDetail.ProductPrice;
                }
            }
            catch (Exception ex)
            {

                Logger.Exception(ex, "InsertPhotographyProduct");
            } 
            #endregion

            #region SaveProductFromCart
            foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item in propertyOrder.Cart)
            {
                #region PhotographyAndFloorplanProduct
                if (item.TypeId == ProductTypes.Photography || item.TypeId == ProductTypes.FloorPlans)
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    null, false, photoOrderId, null, null, false);

                            if (item.ProductId == ProductSettings.ColourisationOfFloorPlanOrSitePlanStandard)
                            {
                                colourisedPlan = colourisedPlan + 1;
                            }
                        }

                        #region Photography
                        //If Dusk Overlay for Platinium Day Photography is added with Platinum Dusk Photography then update the cart accordingly
                        if (item.TypeId == ProductTypes.Photography && item.ProductConfig.Fields.Field.Any(f => f.FieldName == "Duskoverlay"))
                        {
                            #region FOR Dusk Overlay
                            decimal duskOverLayPrice = WorkflowConfig.DUSK_OVERLAY_PRODUCT_PRICE;
                            var prlid = (from c in ctx.Clients
                                         join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                         where c.ClientID == propertyOrder.ClientId
                                         && i.ProductID == WorkflowConfig.DUSK_OVERLAY_PRODUCT_ID
                                         select i).FirstOrDefault();
                            if (prlid != null && prlid.ProductPrice > 0)
                            {
                                duskOverLayPrice = prlid.ProductPrice * new decimal(1.1);
                            }
                            //int duskOverlay = 0;

                            if (item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
                            {

                                var duskOverLayConfig = item.ProductConfig.Fields.Field.Where(f => f.FieldName == "Duskoverlay").FirstOrDefault();
                                if (duskOverLayConfig != null)
                                {

                                    //duskOverlay = duskOverLayConfig.Value.ToString() == "True" || duskOverLayConfig.Value.ToString() == "1" ? 1 : 0;
                                    if (duskOverLayConfig.Value == "True" || duskOverLayConfig.Value == "true")
                                    {
                                        InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, WorkflowConfig.DUSK_OVERLAY_PRODUCT_ID, item.ItemQty,
                                        null, false, photoOrderId, "Dusk Overlay for Platinium Day Photography (Upgrade)", null, false, (decimal)duskOverLayPrice / (decimal)1.1);
                                    }
                                }
                            }
                            #endregion
                        }
                        if (item.TypeId == ProductTypes.Photography && item.ProductConfig.Fields.Field.Any(f => f.FieldName == "LandDimensionsOptions"))
                        {
                            #region FOR OverlayOption for DronePhotoGraphy
                            var overlayConfig = item.ProductConfig.Fields.Field.Where(f => f.FieldName == "LandDimensionsOptions").FirstOrDefault();
                            //Get Overlay Config Selected value
                            if (overlayConfig != null && !string.IsNullOrEmpty(overlayConfig.Value))
                            {
                                int productId = 0;
                                decimal overlayPrice = 0;
                                string productNote = string.Empty;
                                if (overlayConfig.Value == WorkflowConfig.DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_NAME)
                                {
                                    productId = WorkflowConfig.DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_ID;
                                    overlayPrice = WorkflowConfig.DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_PRICE;
                                    productNote = "Drone Photography - 1 Overlays";
                                }
                                else if (overlayConfig.Value == WorkflowConfig.DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_NAME)
                                {
                                    productId = WorkflowConfig.DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_ID;
                                    overlayPrice = WorkflowConfig.DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_PRICE;
                                    productNote = "Drone Photography - 2 Overlays";
                                }
                                else if (overlayConfig.Value == WorkflowConfig.DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_NAME)
                                {
                                    productId = WorkflowConfig.DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_ID;
                                    overlayPrice = WorkflowConfig.DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_PRICE;
                                    productNote = "Drone Photography - 3 Overlays";
                                }
                                else if (overlayConfig.Value == WorkflowConfig.DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_NAME)
                                {
                                    productId = WorkflowConfig.DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_ID;
                                    overlayPrice = WorkflowConfig.DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_PRICE;
                                    productNote = "Drone Photography - 4 Overlays";
                                }
                                if (productId > 0)
                                {
                                    var prlid = (from c in ctx.Clients
                                                 join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                 where c.ClientID == propertyOrder.ClientId
                                                 && i.ProductID == productId
                                                 select i).FirstOrDefault();
                                    if (prlid != null && prlid.ProductPrice >= 0)
                                    {
                                        overlayPrice = prlid.ProductPrice * new decimal(1.1);
                                    }
                                    InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, productId, item.ItemQty,
                                           null, false, photoOrderId, productNote, null, false, overlayPrice / (decimal)1.1);
                                }
                            }
                            var sitePlanConfig = item.ProductConfig.Fields.Field.Where(f => f.FieldName == "DimensionInputOption").FirstOrDefault();
                            if (sitePlanConfig != null && !string.IsNullOrEmpty(sitePlanConfig.Value) && sitePlanConfig.Value == "Add Site Plan")
                            {
                                int prodId = WorkflowConfig.DRONE_PHOTOGRAPHY_SITEPLAN_PRODUCT_ID; ;
                                decimal sitePlanOptionPrice = WorkflowConfig.DRONE_PHOTOGRAPHY_SITEPLAN_PRODUCT_PRICE;
                                if (prodId > 0)
                                {
                                    var prlid = (from c in ctx.Clients
                                                 join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                 where c.ClientID == propertyOrder.ClientId
                                                 && i.ProductID == prodId
                                                 select i).FirstOrDefault();
                                    if (prlid != null && prlid.ProductPrice > 0)
                                    {
                                        sitePlanOptionPrice = prlid.ProductPrice * new decimal(1.1);
                                    }
                                    InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, prodId, item.ItemQty,
                                         null, false, photoOrderId, "Drone Dimension Input Option: SitePlan", null, false, sitePlanOptionPrice / (decimal)1.1);
                                }
                            }
                            #endregion
                        }
                        //If Photography and key collection ticked then update the cart accordingly
                        if (item.TypeId == ProductTypes.Photography && item.ProductConfig.Fields.Field.Any(f => f.FieldName == "PickupKeys"))
                        {
                            #region FOR Pick up Key
                            decimal pickUpKeyPrice = (decimal)27.27;
                            var prlid = (from c in ctx.Clients
                                         join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                         where c.ClientID == propertyOrder.ClientId
                                         && i.ProductID == ProductSettings.KeyCollectionForPhotographyJobs
                                         select i).FirstOrDefault();
                            if (prlid != null && prlid.ProductPrice >= 0)
                            {
                                pickUpKeyPrice = prlid.ProductPrice;
                            }

                            if (item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
                            {

                                var pickUpKeyConfig = item.ProductConfig.Fields.Field.Where(f => f.FieldName == "PickupKeys").FirstOrDefault();
                                if (pickUpKeyConfig != null)
                                {

                                    if ((pickUpKeyConfig.Value == "True" || pickUpKeyConfig.Value == "true") && keyCollection == 0)
                                    {

                                        //insert key collection
                                        OrderDetail orderDetail = new OrderDetail();
                                        orderDetail.OrderID = photoOrderId;
                                        orderDetail.ProductID = ProductSettings.KeyCollectionForPhotographyJobs;
                                        orderDetail.Qty = 1;
                                        orderDetail.Price = pickUpKeyPrice;
                                        orderDetail.GST = pickUpKeyPrice * new decimal(0.1);
                                        orderDetail.Total = pickUpKeyPrice * new decimal(1.1);

                                        Logger.Warn("Trying to add Pickup Keys (Fee Applies $30) fee");

                                        ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                        ctx.SubmitChanges();
                                        keyCollection = keyCollection + 1;
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("photoOrderId: {0} - TypeId: {1} - ProductId: {2}", photoOrderId.ToString(), item.TypeId.ToString(), item.ProductId.ToString()));
                    }

                }
                #endregion

                #region ModularPackage
                if (item.TypeId == ProductTypes.Packages || item.TypeId == ProductTypes.BoardPackages || item.TypeId == ProductTypes.OtherPackages)
                {
                    ////just add product as normal
                    //int orderDetailID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                    //                        item.FindFormat(propertyOrder.IsDIYOrder), propertyOrder.IsDIYOrder, orderId, null, null, usePackageContentPrice);

                    foreach (PackageGroup itemGroup in item.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                        {
                            if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                            {
                                if (contentProductItem.TypeId == ProductTypes.Photography || contentProductItem.TypeId == ProductTypes.FloorPlans)
                                {
                                    InsertPhotoProductIntoOrderDetails(ctx, propertyOrder.ClientId, contentProductItem.ProductId, contentProductItem.PkgQty,
                                            null, false, photoOrderId, contentProductItem.ItemNotes, null, true, true);

                                    if (contentProductItem.ProductId == ProductSettings.ColourisationOfFloorPlanOrSitePlanStandard)
                                    {
                                        colourisedPlan = colourisedPlan + 1;
                                    }

                                    #region Photography
                                    //If Dusk Overlay for Platinium Day Photography is added with Platinum Dusk Photography then update the cart accordingly
                                    if (contentProductItem.TypeId == ProductTypes.Photography && contentProductItem.ProductConfig.Fields.Field.Any(f => f.FieldName == "Duskoverlay"))
                                    {
                                        #region FOR Dusk Overlay
                                        decimal duskOverLayPrice = WorkflowConfig.DUSK_OVERLAY_PRODUCT_PRICE;
                                        var prlid = (from c in ctx.Clients
                                                     join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                     where c.ClientID == propertyOrder.ClientId
                                                     && i.ProductID == WorkflowConfig.DUSK_OVERLAY_PRODUCT_ID
                                                     select i).FirstOrDefault();
                                        if (prlid != null && prlid.ProductPrice > 0)
                                        {
                                            duskOverLayPrice = prlid.ProductPrice * new decimal(1.1);
                                        }
                                        //int duskOverlay = 0;

                                        if (contentProductItem.ProductConfig != null && contentProductItem.ProductConfig.Fields.Field != null && contentProductItem.ProductConfig.Fields.Field.Count > 0)
                                        {

                                            var duskOverLayConfig = contentProductItem.ProductConfig.Fields.Field.Where(f => f.FieldName == "Duskoverlay").FirstOrDefault();
                                            if (duskOverLayConfig != null)
                                            {

                                                //duskOverlay = duskOverLayConfig.Value.ToString() == "True" || duskOverLayConfig.Value.ToString() == "1" ? 1 : 0;
                                                if (duskOverLayConfig.Value == "True" || duskOverLayConfig.Value == "true")
                                                {
                                                    InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, WorkflowConfig.DUSK_OVERLAY_PRODUCT_ID, contentProductItem.PkgQty,
                                                    null, false, photoOrderId, "Dusk Overlay for Platinum Day Photography (Upgrade)", null, false, (decimal)duskOverLayPrice / (decimal)1.1);
                                                }
                                            }
                                        }
                                        #endregion
                                    }

                                    //If Photography and key collection ticked then update the cart accordingly
                                    if (item.TypeId == ProductTypes.Photography && item.ProductConfig.Fields.Field.Any(f => f.FieldName == "PickupKeys"))
                                    {
                                        #region FOR Pick up Key
                                        decimal pickUpKeyPrice = (decimal)27.27;
                                        var prlid = (from c in ctx.Clients
                                                     join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                     where c.ClientID == propertyOrder.ClientId
                                                     && i.ProductID == ProductSettings.KeyCollectionForPhotographyJobs
                                                     select i).FirstOrDefault();
                                        if (prlid != null && prlid.ProductPrice >= 0)
                                        {
                                            pickUpKeyPrice = prlid.ProductPrice;
                                        }

                                        if (item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
                                        {

                                            var pickUpKeyConfig = item.ProductConfig.Fields.Field.Where(f => f.FieldName == "PickupKeys").FirstOrDefault();
                                            if (pickUpKeyConfig != null)
                                            {
                                                if ((pickUpKeyConfig.Value == "True" || pickUpKeyConfig.Value == "true") && keyCollection == 0)
                                                {
                                                    //insert key collection
                                                    OrderDetail orderDetail = new OrderDetail();
                                                    orderDetail.OrderID = photoOrderId;
                                                    orderDetail.ProductID = ProductSettings.KeyCollectionForPhotographyJobs;
                                                    orderDetail.Qty = 1;
                                                    orderDetail.Price = pickUpKeyPrice;
                                                    orderDetail.GST = pickUpKeyPrice * new decimal(0.1);
                                                    orderDetail.Total = pickUpKeyPrice * new decimal(1.1);

                                                    Logger.Warn("Trying to add Pickup Keys (Fee Applies $30) fee");

                                                    ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                    ctx.SubmitChanges();
                                                    keyCollection = keyCollection + 1;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion

                                }
                            }
                        }
                    }
                }
                #endregion

            } 
            #endregion

            #region SaveProductFromProductDataAttribute
            foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item in propertyOrder.Cart)
            {
                #region FloorplanProduct
                if (item.TypeId == ProductTypes.FloorPlans)
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            #region Floorplan
                            if (item.TypeId == ProductTypes.FloorPlans)
                            {
                                if (item.ProductConfig != null)
                                {
                                    foreach (var field in item.ProductConfig.Fields.Field)
                                    {
                                        if (!string.IsNullOrEmpty(field.Value) && field.Caption == "Floor Plan Options")
                                        {
                                            if (field.Value == "3D Colourised FloorPlan (Add $60)" && threeDFloorPlan == 0)
                                            {
                                                //insert 3d product 60

                                                OrderDetail orderDetail = new OrderDetail();
                                                orderDetail.OrderID = photoOrderId;
                                                orderDetail.ProductID = ProductSettings.ThreeDFloorPlan;
                                                //orderDetail.ProductID = 20041;
                                                orderDetail.Qty = 1;
                                                orderDetail.Price = threeDFloorPlanPrice;
                                                orderDetail.GST = threeDFloorPlan * new decimal(0.1);
                                                orderDetail.Total = threeDFloorPlanPrice * new decimal(1.1);

                                                Logger.Warn("Trying to add 3D Colourised FloorPlan fee");

                                                ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                ctx.SubmitChanges();
                                                threeDFloorPlan = threeDFloorPlan + 1;
                                            }
                                            else if (field.Value == "Colourised Standard Floorplan (Add $20)" && colourisedPlan == 0)
                                            {
                                                try
                                                {
                                                    InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, ProductSettings.ColourisationOfFloorPlanOrSitePlanStandard, 1,
                                                                                    null, false, photoOrderId, null, null, false);
                                                    colourisedPlan = colourisedPlan + 1;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Exception(ex, "Price List does not have this product - Clientid:" + propertyOrder.ClientId);
                                                    //insert colourise product 20
                                                    OrderDetail orderDetail = new OrderDetail();
                                                    orderDetail.OrderID = photoOrderId;
                                                    orderDetail.ProductID = ProductSettings.ColourisationOfFloorPlanOrSitePlanStandard;
                                                    //orderDetail.ProductID = 20040;
                                                    orderDetail.Qty = 1;
                                                    orderDetail.Price = colouriseFloorPlanPrice;
                                                    orderDetail.GST = colouriseFloorPlanPrice * new decimal(0.1);
                                                    orderDetail.Total = colouriseFloorPlanPrice * new decimal(1.1);

                                                    Logger.Warn("Trying to add Colourised Standard Floorplan fee");

                                                    ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                    ctx.SubmitChanges();
                                                    colourisedPlan = colourisedPlan + 1;
                                                }

                                            }
                                            else if (field.Value == "3D Colourised FloorPlan + Black and white Standard Floorplan (Add $80)" && threeDPlusStandard == 0)
                                            {
                                                //insert colourise product 20
                                                OrderDetail orderDetail = new OrderDetail();
                                                orderDetail.OrderID = photoOrderId;
                                                orderDetail.ProductID = ProductSettings.ThreeDFloorPlanBlackWhiteStandardFloorplan;
                                                //orderDetail.ProductID = 20042;
                                                orderDetail.Qty = 1;
                                                orderDetail.Price = (decimal)72.73;
                                                orderDetail.GST = (decimal)7.27;
                                                orderDetail.Total = (decimal)80.00;

                                                Logger.Warn("Trying to add 3D Colourised FloorPlan + Black and white Standard Floorplan fee");

                                                ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                ctx.SubmitChanges();
                                                threeDPlusStandard = threeDPlusStandard + 1;
                                            }
                                            else if (field.Value == "3D Colourised FloorPlan + Colourised Standard Floorplan (Add $90)" && threeDPlusColourised == 0)
                                            {
                                                //insert colourise product 20
                                                OrderDetail orderDetail = new OrderDetail();
                                                orderDetail.OrderID = photoOrderId;
                                                orderDetail.ProductID = ProductSettings.ThreeDFloorPlanColourStandardFloorplan;
                                                //orderDetail.ProductID = 20043;
                                                orderDetail.Qty = 1;
                                                orderDetail.Price = (decimal)81.82;
                                                orderDetail.GST = (decimal)8.18;
                                                orderDetail.Total = (decimal)90.00;

                                                Logger.Warn("Trying to add 3D Colourised FloorPlan + Colourised Standard Floorplan fee");

                                                ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                ctx.SubmitChanges();
                                                threeDPlusColourised = threeDPlusColourised + 1;
                                            }

                                        }
                                        if (!string.IsNullOrEmpty(field.Value) && field.Caption == "Pickup Keys (Fee Applies $30)")
                                        {
                                            if (field.Value == "True" && keyCollection == 0)
                                            {
                                                //insert key collection

                                                OrderDetail orderDetail = new OrderDetail();
                                                orderDetail.OrderID = photoOrderId;
                                                orderDetail.ProductID = ProductSettings.KeyCollectionForPhotographyJobs;
                                                orderDetail.Qty = 1;
                                                orderDetail.Price = (decimal)27.27;
                                                orderDetail.GST = (decimal)2.73;
                                                orderDetail.Total = (decimal)30.00;

                                                Logger.Warn("Trying to add Pickup Keys (Fee Applies $30) fee");

                                                ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                ctx.SubmitChanges();
                                                keyCollection = keyCollection + 1;
                                            }

                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("photoOrderId: {0} - TypeId: {1} - ProductId: {2}", photoOrderId.ToString(), item.TypeId.ToString(), item.ProductId.ToString()));
                    }

                }
                #endregion

                #region ModularPackage
                if (item.TypeId == ProductTypes.Packages || item.TypeId == ProductTypes.BoardPackages || item.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup itemGroup in item.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                        {
                            if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                            {
                                if (contentProductItem.TypeId == ProductTypes.FloorPlans)
                                {
                                    #region Floorplan
                                    if (contentProductItem.TypeId == ProductTypes.FloorPlans)
                                    {
                                        if (contentProductItem.ProductConfig != null)
                                        {
                                            foreach (var field in contentProductItem.ProductConfig.Fields.Field)
                                            {
                                                if (!string.IsNullOrEmpty(field.Value) && field.Caption == "Floor Plan Options")
                                                {
                                                    if (field.Value == "3D Colourised FloorPlan (Add $60)" && threeDFloorPlan == 0)
                                                    {
                                                        //insert 3d product 60

                                                        OrderDetail orderDetail = new OrderDetail();
                                                        orderDetail.OrderID = photoOrderId;
                                                        orderDetail.ProductID = ProductSettings.ThreeDFloorPlan;
                                                        //orderDetail.ProductID = 20041;
                                                        orderDetail.Qty = 1;
                                                        orderDetail.Price = threeDFloorPlanPrice;
                                                        orderDetail.GST = threeDFloorPlan * new decimal(0.1);
                                                        orderDetail.Total = threeDFloorPlanPrice * new decimal(1.1);

                                                        Logger.Warn("Trying to add 3D Colourised FloorPlan fee");

                                                        ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                        ctx.SubmitChanges();
                                                        threeDFloorPlan = threeDFloorPlan + 1;
                                                    }
                                                    else if (field.Value == "Colourised Standard Floorplan (Add $20)" && colourisedPlan == 0)
                                                    {
                                                        try
                                                        {
                                                            InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, ProductSettings.ColourisationOfFloorPlanOrSitePlanStandard, 1,
                                                                                            null, false, photoOrderId, null, null, false);
                                                            colourisedPlan = colourisedPlan + 1;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Logger.Exception(ex, "Price List does not have this product - Clientid:" + propertyOrder.ClientId);
                                                            //insert colourise product 20
                                                            OrderDetail orderDetail = new OrderDetail();
                                                            orderDetail.OrderID = photoOrderId;
                                                            orderDetail.ProductID = ProductSettings.ColourisationOfFloorPlanOrSitePlanStandard;
                                                            //orderDetail.ProductID = 20040;
                                                            orderDetail.Qty = 1;
                                                            orderDetail.Price = colouriseFloorPlanPrice;
                                                            orderDetail.GST = colouriseFloorPlanPrice * new decimal(0.1);
                                                            orderDetail.Total = colouriseFloorPlanPrice * new decimal(1.1);

                                                            Logger.Warn("Trying to add Colourised Standard Floorplan fee");

                                                            ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                            ctx.SubmitChanges();
                                                            colourisedPlan = colourisedPlan + 1;
                                                        }
                                                    }
                                                    else if (field.Value == "3D Colourised FloorPlan + Black and white Standard Floorplan (Add $80)" && threeDPlusStandard == 0)
                                                    {
                                                        //insert colourise product 20
                                                        OrderDetail orderDetail = new OrderDetail();
                                                        orderDetail.OrderID = photoOrderId;
                                                        orderDetail.ProductID = ProductSettings.ThreeDFloorPlanBlackWhiteStandardFloorplan;
                                                        //orderDetail.ProductID = 20042;
                                                        orderDetail.Qty = 1;
                                                        orderDetail.Price = (decimal)72.73;
                                                        orderDetail.GST = (decimal)7.27;
                                                        orderDetail.Total = (decimal)80.00;

                                                        Logger.Warn("Trying to add 3D Colourised FloorPlan + Black and white Standard Floorplan fee");

                                                        ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                        ctx.SubmitChanges();
                                                        threeDPlusStandard = threeDPlusStandard + 1;
                                                    }
                                                    else if (field.Value == "3D Colourised FloorPlan + Colourised Standard Floorplan (Add $90)" && threeDPlusColourised == 0)
                                                    {
                                                        //insert colourise product 20
                                                        OrderDetail orderDetail = new OrderDetail();
                                                        orderDetail.OrderID = photoOrderId;
                                                        orderDetail.ProductID = ProductSettings.ThreeDFloorPlanColourStandardFloorplan;
                                                        //orderDetail.ProductID = 20043;
                                                        orderDetail.Qty = 1;
                                                        orderDetail.Price = (decimal)81.82;
                                                        orderDetail.GST = (decimal)8.18;
                                                        orderDetail.Total = (decimal)90.00;

                                                        Logger.Warn("Trying to add 3D Colourised FloorPlan + Colourised Standard Floorplan fee");

                                                        ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                        ctx.SubmitChanges();
                                                        threeDPlusColourised = threeDPlusColourised + 1;
                                                    }
                                                }
                                                if (!string.IsNullOrEmpty(field.Value) && field.Caption == "Pickup Keys (Fee Applies $30)")
                                                {
                                                    if (field.Value == "True" && keyCollection == 0)
                                                    {
                                                        //insert key collection

                                                        OrderDetail orderDetail = new OrderDetail();
                                                        orderDetail.OrderID = photoOrderId;
                                                        orderDetail.ProductID = ProductSettings.KeyCollectionForPhotographyJobs;
                                                        orderDetail.Qty = 1;
                                                        orderDetail.Price = (decimal)27.27;
                                                        orderDetail.GST = (decimal)2.73;
                                                        orderDetail.Total = (decimal)30.00;

                                                        Logger.Warn("Trying to add Pickup Keys (Fee Applies $30) fee");

                                                        ctx.OrderDetails.InsertOnSubmit(orderDetail);
                                                        ctx.SubmitChanges();
                                                        keyCollection = keyCollection + 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                #endregion

            } 
            #endregion
        }


        private static void InsertStockboardProduct(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, int orderId, AbcDataContext ctx)
        {
            foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item in propertyOrder.Cart)
            {
                #region Stockboard
                if (item.TypeId == ProductTypes.Stockboard)
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    item.FindFormat(false), false, orderId, null, null, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("{0} {1}", orderId, item.ProductId));
                        throw ex;
                    }
                }

                if (item.TypeId == ProductTypes.BoardAccessory && !propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard))
                {
                    try
                    {
                        if (item.ProductId == ProductSettings.Travel)
                        {
                            InsertUpgradeProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                string.Empty, false, orderId, null, null, false, item.ProductPrice);
                        }
                        else if (item.ProductName.Contains("High Installation"))
                        {
                            if (item.ProductId > 0)
                            {
                                try
                                {
                                    var client = (from c in ctx.Clients
                                                  where c.ClientID == propertyOrder.ClientId
                                                  select c).FirstOrDefault();

                                    if (propertyOrder.BoardInstallationType == BoardInstallationType.High)
                                    {
                                        bool isVicClient = false;

                                        if (client != null && !string.IsNullOrEmpty(client.State) && client.State.ToUpper() == "VIC")
                                        {
                                            isVicClient = true;
                                        }

                                        if (isVicClient)
                                        {
                                            var pld = (from c in ctx.Clients
                                                        join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                        where c.ClientID == propertyOrder.ClientId
                                                        && i.ProductID == item.ProductId
                                                        select i).FirstOrDefault();
                                            if (pld != null && pld.ProductPrice > 0)
                                            {
                                                //Need to see how many board in order
                                                int total = 1;

                                                decimal totalPrice = total * pld.ProductPrice;
                                                InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, client.ManagerID, propertyOrder);
                                            }
                                            else //Product not exist on price list - fetch from product Pricing Type
                                            {
                                                var priceList = (from c in ctx.Clients
                                                                 join i in ctx.PriceLists on c.PriceListID equals i.PriceListID
                                                                 where c.ClientID == propertyOrder.ClientId
                                                                 select i).FirstOrDefault();

                                                if (priceList != null)
                                                {
                                                    var productp = (from pp in ctx.ProductPricings
                                                                    where pp.PricingID == priceList.PricingID
                                                                    && pp.ProductID == item.ProductId
                                                                    select pp).FirstOrDefault();

                                                    if (productp != null)
                                                    {
                                                        int total = 1;

                                                        decimal totalPrice = total * productp.Price;
                                                        InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, client.ManagerID, propertyOrder);
                                                    }
                                                    else
                                                    {
                                                        int total = 1;

                                                        decimal totalPrice = total * 150;
                                                        InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, client.ManagerID, propertyOrder);
                                                    }
                                                }

                                            }

                                        }
                                        else
                                        {

                                            var pld = (from c in ctx.Clients
                                                        join i in ctx.PriceListDetails on c.PriceListID equals i.PriceListID
                                                        where c.ClientID == propertyOrder.ClientId
                                                        && i.ProductID == item.ProductId
                                                        select i).FirstOrDefault();
                                            if (pld != null && pld.ProductPrice > 0)
                                            {
                                                //Need to see how many board in order
                                                int total = 1;

                                                decimal totalPrice = total * pld.ProductPrice;
                                                InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, client.ManagerID, propertyOrder);
                                            }
                                            else
                                            {
                                                var priceList = (from c in ctx.Clients
                                                                 join i in ctx.PriceLists on c.PriceListID equals i.PriceListID
                                                                 where c.ClientID == propertyOrder.ClientId
                                                                 select i).FirstOrDefault();

                                                if (priceList != null)
                                                {
                                                    var productp = (from pp in ctx.ProductPricings
                                                                    where pp.PricingID == priceList.PricingID
                                                                    && pp.ProductID == item.ProductId
                                                                    select pp).FirstOrDefault();

                                                    if (productp != null)
                                                    {
                                                        int total = 1;

                                                        decimal totalPrice = total * productp.Price;
                                                        InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, client.ManagerID, propertyOrder);
                                                    }
                                                    else
                                                    {
                                                        int total = 1;

                                                        decimal totalPrice = total * 150;
                                                        InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, totalPrice, orderId, client.ManagerID, propertyOrder);
                                                    }
                                                }

                                            }
                                        }

                                        //send an email

                                        int eventID = EventSettings.HighInstallationProductNeedSetup;
                                        string sub = "Abc Notification: Please Check High Installation To Make Sure - Order: " + orderId;
                                        string xmlData = string.Empty;
                                        xmlData = @"<EVENT>
									        <OrderID>" + orderId.ToString() + @"</OrderID>
									        <ClientID>" + propertyOrder.ClientId.ToString() + @"</ClientID>
									        <ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
									        </EVENT>";

                                        string textData = "Abc Notification: Please Check High Installation To Make Sure - Order: " + orderId;
                                        string source = "OnlineBL_Workflow_CreateNewOrder";

                                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, orderId, null, null, null, source, String.Empty);

                                    }
                                    else if (propertyOrder.BoardInstallationType == BoardInstallationType.HigherThanFirstLevel)
                                        InsertErectionFeeIntoOrderDetails(ctx, item.ProductId, 0, orderId, client.ManagerID, propertyOrder);
                                    else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed500mmOfTheGround)
                                    {
                                        InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 225, orderId, client.ManagerID, propertyOrder);
                                    }
                                    else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1000mmOfTheGround)
                                    {
                                        InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 325, orderId, client.ManagerID, propertyOrder);
                                    }
                                    else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1250mmOfTheGround)
                                    {
                                        InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 525, orderId, client.ManagerID, propertyOrder);
                                    }
                                    else if (propertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed2000mmOfTheGround)
                                    {
                                        InsertAussieSignErectionFeeIntoOrderDetails(ctx, item.ProductId, 0, orderId, client.ManagerID, propertyOrder);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, string.Format("{0} {1}", orderId, item.ProductId));
                                }
                            }
                        }
                        else if (item.ProductId > 0 && !item.ProductName.Contains("High Installation") && item.ProductId != ProductSettings.Travel)
                        {
                            InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    item.FindFormat(false), false, orderId, null, null, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, "orderId: " + orderId + " -- item.TypeId: " + item.TypeId + " -- item.ProductId: " + item.ProductId);
                        throw ex;
                    }
                }
                if (item.TypeId == ProductTypes.Overlay && !propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard))
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    item.FindFormat(false), false, orderId, null, null, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("{0} {1}", orderId, item.ProductId ));
                        throw ex;
                    }
                }
                if (item.TypeId == ProductTypes.StockboardOverlay && !propertyOrder.Cart.Any(i => i.TypeId == ProductTypes.BillBoard))
                {
                    try
                    {
                        if (item.ProductId > 0)
                        {
                            item.OrderDetailsID = InsertProductIntoOrderDetails(ctx, propertyOrder.ClientId, item.ProductId, item.ItemQty,
                                    item.FindFormat(false), false, orderId, null, null, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("{0} {1}", orderId, item.ProductId ));
                        throw ex;
                    }
                }
                #endregion
            }
        }

        #region InsertErectionFeeIntoOrderDetails
        private static void InsertErectionFeeIntoOrderDetails(AbcDataContext ctx, int ErectionFeeProductId, decimal amount, int tempOrderId, string managerId, Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder)
        {

            int? orderDetailId = 0;
            ctx.CDAS_OrderDetailInsertErectionFee(tempOrderId, ErectionFeeProductId,
                                        amount, (propertyOrder.BoardInstallationType == BoardInstallationType.HigherThanFirstLevel),
                                        managerId, ref orderDetailId);
        }

        private static void InsertAussieSignErectionFeeIntoOrderDetails(AbcDataContext ctx, int ErectionFeeProductId, decimal amount, int tempOrderId, string managerId, Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder)
        {

            int? orderDetailId = 0;
            ctx.CDAS_OrderDetailInsertAussieSignErectionFee(tempOrderId, ErectionFeeProductId,
                                        amount, (int)(propertyOrder.BoardInstallationType),
                                        ref orderDetailId);
        }
        #endregion

        private static int InsertProductIntoOrderDetails(AbcDataContext ctx, int clientId,
                int productId, int qty, string format, bool userDesignOnline, int tempOrderId, string itemNote, int? parentID, bool usePackageContentPrice)
        {
            int? orderDetailId = null;
            ctx.AIS_OrderDetailInsert(clientId, tempOrderId,
                                        productId, qty,
                                        format, ref orderDetailId,
                                        userDesignOnline,
                                        itemNote, parentID, usePackageContentPrice);

            return orderDetailId.HasValue ? orderDetailId.Value : 0;
        }

        private static int InsertUpgradeProductIntoOrderDetails(AbcDataContext ctx, int clientId,
                int productId, int qty, string format, bool userDesignOnline, int tempOrderId, string itemNote, int? parentID, bool usePackageContentPrice, decimal price)
        {
            int? orderDetailId = null;
            Logger.Warn(clientId + "--" + tempOrderId + " --" + productId + "--" + qty + "--" + format + " --" + userDesignOnline + "--" + itemNote + "--" + parentID + "--" + usePackageContentPrice + "--" + price);

            if (!string.IsNullOrEmpty(itemNote))
            {
                if (itemNote.Length > 492)
                {
                    itemNote = itemNote.Substring(0, 492) + "...";
                }
            }

            ctx.AIS_OrderDetailInsertUpgradeProduct(clientId, tempOrderId,
                                        productId, qty,
                                        format, ref orderDetailId,
                                        userDesignOnline,
                                        itemNote, parentID, usePackageContentPrice, price);

            return orderDetailId.HasValue ? orderDetailId.Value : 0;
        }

        private static int InsertPhotoProductIntoOrderDetails(AbcDataContext ctx, int clientId,
                int productId, int qty, string format, bool userDesignOnline, int tempOrderId, string itemNote, int? parentID, bool usePackageContentPrice,
                bool allowInvoicing)
        {
            int? orderDetailId = null;
            ctx.AIS_PhotoOrderDetailInsert(clientId, tempOrderId,
                                        productId, qty,
                                        format, ref orderDetailId,
                                        userDesignOnline,
                                        itemNote, parentID, usePackageContentPrice, allowInvoicing);

            return orderDetailId.HasValue ? orderDetailId.Value : 0;
        }

        #endregion

        public static void SendStockBoardInPackEmail(AbcDataContext ctx, string sub, string notes)
        {
            int eventID = EventSettings.StockboardOrderInAPack;
            string xmlData = notes;

            string textData = notes;
            string source = "OrderProcessor_InsertOrder";

            ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, null, null, null, null, source, String.Empty);
        }

        #region GenerateBoardAndStockboardInTheSameOrderEvent
        public static void GenerateBoardAndStockboardInTheSameOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var client = (from c in ctx.Clients
                                  where c.ClientID == nOrderEvent.ClientId
                                  select c).FirstOrDefault();
                    if (client != null)
                    {
                        //send email notification to admin
                        int eventID = EventSettings.BoardAndStockboard;
                        string sub = "Board and Stockboard in the same order";
                        string xmlData = string.Empty;

                        xmlData = @"<EVENT>
								<OrderID>" + nOrderEvent.OrderId + @"</OrderID>
                                <AgentName>" + client.ClientName.Replace("&", "&amp;") + @"</AgentName>
                                <AgentOffice>" + client.Office + @"</AgentOffice>
								<PAddress>" + nOrderEvent.Prop.Replace("&", "&amp;") + @"</PAddress>
								<Email>" + client.Email.Replace("&", "&amp;") + @"</Email>
                                <ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
                                <ChangeDetails>Board and Stockboard on the same DIY Order, consider seperate them</ChangeDetails>
								</EVENT>";

                        string textData = xmlData;
                        string source = "ProcessOrder";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, nOrderEvent.OrderId, null, null, null, source, String.Empty);
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateBoardAndStockboardInTheSameOrderEvent'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GenerateBmpOrderConfirmationEmail
        public static void GenerateBmpOrderConfirmationEmail(int orderId, string emailAddress, string emailCCAddress, string emailBCCAddress)
        {
            if (orderId <= 0)
            {
                throw new ArgumentNullException("orderId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.FrontOffice_ServiceQueueAdd("OrderConfirmation",
                        String.Format("OrderID={0}", orderId),
                        0,
                        1,
                        emailAddress,
                        null,
                        emailCCAddress,
                        emailBCCAddress,
                        "GenerateBmpOrderConfirmationEmail",
                        1);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, String.Format("Error occured in 'GenerateBmpOrderConfirmationEmail' reportParameter:{0}", orderId));
                throw;
            }
        }
        #endregion

        #region GenerateNotifyAccountEvent
        public static void GenerateNotifyAccountEvent(int orderID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Account);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order order = ctx.Orders.SingleOrDefault(c => c.OrderID == orderID);

                    bool payAsYouGoAccount = false;
                    if (order != null && order.Account != null && order.Account.IsPayAsYouGo.HasValue && order.Account.IsPayAsYouGo.Value)
                    {
                        payAsYouGoAccount = true;
                    }

                    if(!payAsYouGoAccount && order.OrderStatus > 0)
                    {
                        //Notify Account if applicable
                        ctx.ServiceQueue_NotifyAccountOnHold(orderID);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNotifyAccountEvent'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region GenerateNewSBDIYOrderEvent
        public static void GenerateNewSBDIYOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string subject = "DIY Stockboard Order Received: " + nOrderEvent.Prop + " - Job No: " + nOrderEvent.OrderId.ToString();

                    ctx.SP_EventQueueAdd(EventSettings.DIYStockBoardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateNewSBDIYOrderEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewSBDIYOrderEvent'");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GenerateSBOrderEvent
        public static void GenerateSBOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nSBOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string subject = "Stockboard Order Received: " + nOrderEvent.Prop + " - Job No: " + nOrderEvent.OrderId.ToString();

                    ctx.SP_EventQueueAdd(EventSettings.WorkshopStockBoardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateSBOrderEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateSBOrderEvent'");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GenerateNewOverlayStickerOrderEvent
        public static void GenerateNewOverlayStickerOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Order order = ctx.Orders.SingleOrDefault(o => o.OrderID == nOrderEvent.OrderId);

                    string managerID = "";
                    if(order != null)
                    {
                        managerID = order.ManagerID;
                    }

                    string subject = "Overlay/Sold Sticker Order Received: - Job No: " + nOrderEvent.OrderId.ToString() + " Manager ID: " + managerID;

                    ctx.SP_EventQueueAdd(EventSettings.WorkshopStockBoardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateNewOverlayStickerOrderEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewOverlayStickerOrderEvent'");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GenerateNewCorfluteOrderEvent
        public static void GenerateNewCorfluteOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string subject = "Corflute Order Received: - Job No: " + nOrderEvent.OrderId.ToString();

                    ctx.SP_EventQueueAdd(EventSettings.WorkshopStockBoardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateNewOverlayStickerOrderEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewCorfluteOrderEvent'");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GenerateNewMarketingOrderEvent
        public static void GenerateNewMarketingOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string subject = "Marketing Products Order Received:" + " - Job No: " + nOrderEvent.OrderId.ToString();

                    ctx.SP_EventQueueAdd(EventSettings.WorkshopStockBoardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateNewOverlayStickerOrderEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewMarketingOrderEvent'");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GenerateNewCommunityBoardDIYOrderEvent
        public static void GenerateNewCommunityBoardDIYOrderEvent(NewOrderEvent nOrderEvent)
        {
            if (nOrderEvent == null)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string subject = "DIY Community Board Order Received: " + nOrderEvent.Prop + " - Job No: " + nOrderEvent.OrderId.ToString();

                    ctx.SP_EventQueueAdd(EventSettings.AutomateStockboardOrderReceived, subject, nOrderEvent.HtmlBodyUS, nOrderEvent.HtmlBodyUS, nOrderEvent.OrderId, nOrderEvent.ClientId, nOrderEvent.ManagerID, null, "GenerateNewCommunityBoardDIYOrderEvent", nOrderEvent.FileName);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateNewCommunityBoardDIYOrderEvent'");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region PutOrderOnHold
        public static void PutOrderOnHold(int orderID)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Order od = (from o in ctx.Orders
                                where o.OrderID == orderID
                                select o).FirstOrDefault();
                    if (od != null)
                    {
                        od.OnHold = DateTime.Now;
                    }
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'PutOrderOnhold'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region GenerateEmailOxbridgeOrderOnHold
        public static void GenerateEmailOxbridgeOrderOnHold(int orderID, string email)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.OnlineServiceQueue_AddEmailOxbridgeOrderOnHoldToQueue(orderID, email);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateEmailOxbridgeOrderOnHold'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region GenerateOxbridgeEmailNeedApproval
        public static void GenerateOxbridgeEmailNeedApproval(int orderID, string contactName)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("nOrderEvent");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.FrontOffice_ServiceQueueAdd("OrderNeedApproval", "OrderID=" + orderID + ";Contact=" + contactName, 0, 1, "webdesign@photosigns.com.au", null, null, null, "Express_OrderProcessor", 2);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateOxbridgeEmailNeedApproval'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region GenerateOrderLink
        public static void GenerateOrderLink(int orderID, int originalOrderID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (originalOrderID < 0)
            {
                throw new ArgumentNullException("originalOrderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.ABCWRKFLOW_SaveOrderLinks(orderID, originalOrderID);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateOrderLink'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region GenerateApprovePhotoPackOrderQueue
        public static void GenerateApprovePhotoPackOrderQueue(int orderID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var oOtherProduct = (from od in ctx.OrderDetails
                                     where od.OrderID == orderID
                                     && od.Product.TypeID != ProductTypes.Photography && od.Product.TypeID != ProductTypes.FloorPlans
                                     && od.Product.CategoryId != CategoryTypes.Packages
                                     select od).ToList();

                    if (oOtherProduct != null && oOtherProduct.Count > 0)
                    {
                        Logger.Warn("Order has other products than just photography or floorplan: " + orderID);
                    }
                    else
                    {
                        ctx.OnlineServiceQueue_AddApprovePhotoOrderToQueue(orderID);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateApprovePhotoPackOrderQueue'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region GenerateOrderDeliveryFee
        public static void GenerateOrderDeliveryFee(int orderID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.ABCWRKFLOW_ApplyDeliveryFee(orderID);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateOrderDeliveryFee'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region CheckManager
        public static void CheckManager(int orderID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var oSBProduct = (from od in ctx.OrderDetails
                                      where od.OrderID == orderID
                                      && od.Product.TypeID == ProductTypes.Stockboard
                                      && od.UserDesignOnline == false
                                      && od.ParentID.HasValue
                                      select od).ToList();

                    if (oSBProduct != null && oSBProduct.Count > 0)
                    {
                        Logger.Warn("Order has non DIY - stockboard in pack: " + orderID);

                        Order od = (from o in ctx.Orders
                                    where o.OrderID == orderID
                                    select o).FirstOrDefault();
                        if (od != null)
                        {
                            od.ManagerID = ManagerSettings.InHouse;
                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CheckManager'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        public static void SyncProduct(Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item)
        {
            if (item != null && item.ProductConfig != null && item.ProductConfig.Fields.Field != null && item.ProductConfig.Fields.Field.Count > 0)
            {
                if (!string.IsNullOrEmpty(item.ProductConfig.Fields.Field[0].Value) && !string.IsNullOrEmpty(item.ProductConfig.Fields.Field[1].Value))
                {
                    string printing = item.ProductConfig.Fields.Field[0].Value;
                    string size = item.ProductConfig.Fields.Field[1].Value;

                    if(printing == "DS")
                    {
                        switch (size)
                        {
                            case "300x450":
                                item.ProductId = 1602;
                                break;
                            case "450x600":
                                item.ProductId = 1604;
                                break;
                            case "600x600":
                                item.ProductId = 476;
                                break;
                            case "600x900":
                                item.ProductId = 627;
                                break;
                            case "1200x900":
                                item.ProductId = 629;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (size)
                        {
                            case "300x450":
                                item.ProductId = 1601;
                                break;
                            case "450x600":
                                item.ProductId = 1603;
                                break;
                            case "600x600":
                                item.ProductId = 475;
                                break;
                            case "600x900":
                                item.ProductId = 628;
                                break;
                            case "1200x900":
                                item.ProductId = 630;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        #region SendPaygInvoiceToClient
        public static void SendPaygInvoiceToClient(int orderID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Account);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order order = ctx.Orders.SingleOrDefault(c => c.OrderID == orderID);

                    bool sendInvoice = false;
                    if(order != null && order.OrderStatus > 0 && order.Account != null && order.Account.IsPayAsYouGo.HasValue && order.Account.IsPayAsYouGo.Value)
                    {
                        sendInvoice = true;
                    }

                    //Send payg invoice if applicable
                    if(sendInvoice)
                    {
                        Invoice invoice = ctx.Invoices.SingleOrDefault(i => i.OrderID == orderID);
                        if(invoice != null)
                        {
                            if(invoice.AmountDue > (decimal)0.5)
                            {
                                invoice.DateInvoiced = DateTime.Now;

                                ctx.ServiceQueue_SendPaygInvoiceToClient(orderID);
                                ctx.SubmitChanges();
                            }
                            else
                            {
                                order.OrderStatus = 0;
                                ctx.SubmitChanges();
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'SendPaygInvoiceToClient'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region UpdateManagerForRegionalProperty
        public static void UpdateManagerForRegionalProperty(int orderID, string managerID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Logger.Warn("Regional Property Order - Update Manager: " + orderID);

                    Order od = (from o in ctx.Orders
                                where o.OrderID == orderID
                                select o).FirstOrDefault();
                    if (od != null)
                    {
                        od.ManagerID = managerID;
                    }

                    //ctx.OnlineServiceQueue_Add(orderID, "OrderInRegionalLocation", "signs@photosigns.com.au");

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateManagerForRegionalProperty'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region SaveOrderDetailIdToDesignDocument
        public static void SaveOrderDetailIdToDesignDocument(int propertyId, CartItem item)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var jd = ctx.Design_Documents.FirstOrDefault(j => j.PropertyId == propertyId && j.ProductId == item.ProductId && j.CartItemId == item.Id && j.TemplateProductId == item.SelectedDIYTemplateId && j.UniqueId == item.ProductGuid); // && (dc.PropertyStatus == (int)AopPropertyStatusList.InProgress || dc.PropertyStatus == (int)AopPropertyStatusList.OnHold)
                    if (jd != null)
                    {
                        jd.OrderDetailId = item.OrderDetailsID;
                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SaveOrderDetailIdToDesignDocument ({0})", propertyId);
                throw;
            }
        }
        #endregion

        #region SaveOrderDetailIdToProductGroupDesignDocument
        public static void SaveOrderDetailIdToProductGroupDesignDocument(int propertyId, PackageContentProduct item, int groupID)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var jd = ctx.Design_Documents.FirstOrDefault(j => j.PropertyId == propertyId && j.ProductId == item.ProductId && j.GroupId == groupID && j.TemplateProductId == item.SelectedDIYTemplateId); // && (dc.PropertyStatus == (int)AopPropertyStatusList.InProgress || dc.PropertyStatus == (int)AopPropertyStatusList.OnHold)
                    if (jd != null)
                    {
                        jd.OrderDetailId = item.OrderDetailsID;
                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SaveOrderDetailIdToProductGroupDesignDocument ({0})", propertyId);
                throw;
            }
        }
        #endregion

        #region UpdateOrderOtherDetail
        public static void UpdateOrderOtherDetail(int orderID, bool hasInstallFile)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    OrderOtherDetail od = (from o in ctx.OrderOtherDetails
                                           where o.OrderId == orderID
                                           select o).FirstOrDefault();
                    if (od != null)
                    {
                        od.HasInstallFile = hasInstallFile;
                    }
                    else
                    {
                        od = new OrderOtherDetail();
                        od.OrderId = orderID;
                        od.HasInstallFile = hasInstallFile;
                        ctx.OrderOtherDetails.InsertOnSubmit(od);
                    }
                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateOrderOtherDetail'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion

        #region CheckFlagHolderToAddNotes
        public static void CheckFlagHolderToAddNotes(int orderID)
        {
            if (orderID < 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var ordDetail = (from od in ctx.OrderDetails
                                      where od.OrderID == orderID
                                      && od.ProductID == 429
                                      select od).ToList();

                    if (ordDetail != null && ordDetail.Count > 0)
                    {
                        //Logger.Warn("Order has flag holder: " + orderID);

                        Order od = (from o in ctx.Orders
                                    where o.OrderID == orderID
                                    select o).FirstOrDefault();
                        if (od != null && od.ManagerID == ManagerSettings.WorkshopVictoria)
                        {
                            if(od.LocationID == 26 || od.LocationID == 27 || od.LocationID == 28 || od.LocationID == 29 || od.LocationID == 31 || od.LocationID == 10706 || od.LocationID == 487 || od.LocationID == 1132 || od.LocationID == 1133 || od.LocationID == 24404 || od.LocationID == 1509 || od.LocationID == 10699 || od.LocationID == 1778 || od.LocationID == 27812 || od.LocationID == 27970 || od.LocationID == 1829 
                                || od.LocationID == 2188 || od.LocationID == 2189)
                            {
                                if (string.IsNullOrWhiteSpace(od.ErectionNotes))
                                {
                                    od.ErectionNotes = "Hobson Bay council - Flag holders on back of board.";
                                }
                                else
                                {
                                    od.ErectionNotes = od.ErectionNotes + Environment.NewLine + "Hobson Bay council - Flag holders on back of board.";
                                }
                            }
                            else if (od.LocationID == 462 || od.LocationID == 24179 || od.LocationID == 710 || od.LocationID == 711 || od.LocationID == 1048 || od.LocationID == 1213 || od.LocationID == 1232 || od.LocationID == 1785 || od.LocationID == 2012 
                                || od.LocationID == 2281 || od.LocationID == 2282)
                            {
                                if (string.IsNullOrWhiteSpace(od.ErectionNotes))
                                {
                                    od.ErectionNotes = "Maribyrnong council - Flag holders forward over the footpath.";
                                }
                                else
                                {
                                    od.ErectionNotes = od.ErectionNotes + Environment.NewLine + "Maribyrnong council - Flag holders forward over the footpath.";
                                }
                            }
                            else if (od.LocationID == 10 || od.LocationID == 14 || od.LocationID == 47 || od.LocationID == 10702 || od.LocationID == 28674 || od.LocationID == 550 || od.LocationID == 552 || od.LocationID == 10818 || od.LocationID == 907 || od.LocationID == 1000 || od.LocationID == 1001 || od.LocationID == 1002 || od.LocationID == 1003 || od.LocationID == 1004 || od.LocationID == 24526 || od.LocationID == 27343 || od.LocationID == 1044 || od.LocationID == 1839 || od.LocationID == 1889 || od.LocationID == 1890 || od.LocationID == 1891 || od.LocationID == 1904 
                                || od.LocationID == 1949 || od.LocationID == 2033)
                            {
                                if (string.IsNullOrWhiteSpace(od.ErectionNotes))
                                {
                                    od.ErectionNotes = "Brimbank council - Flag holders forward over the footpath.";
                                }
                                else
                                {
                                    od.ErectionNotes = od.ErectionNotes + Environment.NewLine + "Brimbank council - Flag holders forward over the footpath.";
                                }
                            }

                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CheckFlagHolderToAddNotes'.");
                Logger.Exception(ex, message);
            }
        }
        #endregion
    }
}
