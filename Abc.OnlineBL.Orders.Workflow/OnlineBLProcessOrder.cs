using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Workflow.Activities;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Orders.Workflow.Model;
using Dom = Abc.OnlinePublication.Common.DOM;
using System.Xml.Linq;
using Abc.OnlineBL.VirtualFileSystem;

namespace Abc.OnlineBL.Orders.Workflow
{
    public sealed partial class OnlineBLProcessOrder : SequentialWorkflowActivity
    {
        private OrderDataExchange orderDataExchange;
        private string fileName = "", xmlFileName = "", stockFileName = "", photoFileName = "";


        #region Public Properties
        public OrderDataExchange OrderDataExchange
        {
            get { return orderDataExchange; }
            set { orderDataExchange = value; }
        }
        #endregion

        public OnlineBLProcessOrder()
        {
            InitializeComponent();
        }

        private void InitializeOrder_ExecuteCode(object sender, EventArgs e)
        {
            List<EntityRelations> options = new List<EntityRelations>();
            options.Add(EntityRelations.Property_To_Location);
            orderDataExchange.Property = OrderProcessor.GetPropertyById(orderDataExchange.PropertyOrder.PropertyId, options);
            orderDataExchange.PropertyOrder.Property = OrderProcessor.GetPropertyById(orderDataExchange.PropertyOrder.PropertyId, options);

            if (!orderDataExchange.PropertyOrder.IsDIYOrder && orderDataExchange.PropertyOrder.TextDetails != null)
            {
                int firstIDTemp = 0, secondIDtemp = 0, thirdIDTemp = 0;
                if (orderDataExchange.PropertyOrder.TextDetails.FirstContactID != null && int.TryParse(orderDataExchange.PropertyOrder.TextDetails.FirstContactID, out firstIDTemp))
                {
                    orderDataExchange.PropertyOrder.TextDetails.FirstContact = OrderProcessor.GetClientContactById(firstIDTemp, null);
                }
                if (orderDataExchange.PropertyOrder.TextDetails.SecondContactID != null && int.TryParse(orderDataExchange.PropertyOrder.TextDetails.SecondContactID, out secondIDtemp))
                {
                    orderDataExchange.PropertyOrder.TextDetails.SecondContact = OrderProcessor.GetClientContactById(secondIDtemp, null);
                }
                if (orderDataExchange.PropertyOrder.TextDetails.ThirdContactID != null && int.TryParse(orderDataExchange.PropertyOrder.TextDetails.ThirdContactID, out thirdIDTemp))
                {
                    orderDataExchange.PropertyOrder.TextDetails.ThirdContact = OrderProcessor.GetClientContactById(thirdIDTemp, null);
                }
            }

            options.Clear();
            options.Add(EntityRelations.Client_To_Manager);
            options.Add(EntityRelations.Client_To_ClientsDisplayInfo);
            orderDataExchange.Client = OrderProcessor.GetClientById(orderDataExchange.PropertyOrder.ClientId, options);

            ClientInfo clientInfo = new ClientInfo();
            clientInfo.ClientID = orderDataExchange.Client.ClientID;
            clientInfo.ClientName = orderDataExchange.Client.ClientsDisplayInfo.Office;
            clientInfo.ActualClientName = orderDataExchange.Client.ClientName;
            clientInfo.ActualOffice = orderDataExchange.Client.Office;
            clientInfo.Address = orderDataExchange.Client.ClientsDisplayInfo.Address;
            clientInfo.Office = orderDataExchange.Client.ClientsDisplayInfo.Office;
            clientInfo.Suburb = orderDataExchange.Client.ClientsDisplayInfo.Suburb;
            clientInfo.State = orderDataExchange.Client.ClientsDisplayInfo.State;
            clientInfo.Country = orderDataExchange.Client.ClientsDisplayInfo.Country;
            clientInfo.PostCode = orderDataExchange.Client.ClientsDisplayInfo.PostCode;
            clientInfo.Phone = orderDataExchange.Client.ClientsDisplayInfo.Phone;
            clientInfo.Fax = orderDataExchange.Client.ClientsDisplayInfo.Fax;
            clientInfo.Email = orderDataExchange.Client.ClientsDisplayInfo.Email;
            clientInfo.WebSite = orderDataExchange.Client.ClientsDisplayInfo.WebSite;
            clientInfo.LicenseNo = orderDataExchange.Client.LicenceNo;
            orderDataExchange.ClientInfo = clientInfo;

            #region Make the Caption 1st Letter captial and rest small
            string caption = orderDataExchange.PropertyOrder.TextDetails.Heading;
            if (!string.IsNullOrEmpty(caption))
            {
                caption = caption.Substring(0, 1).ToUpper() + caption.Substring(1).ToLower();
                //if (caption.Length > 45)
                //{
                //    caption = caption.Substring(0, 45) + "...";
                //}
            }
            orderDataExchange.PropertyOrder.TextDetails.Heading = caption;
            #endregion

            orderDataExchange.PropertyOrder.SyncItemQty();
        }

        private void CheckIfPropertyOrder(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.PropertyOrder.GetType() == typeof(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder);
        }

        private void GenerateOrderID_ExecuteCode(object sender, EventArgs e)
        {
            OrderDataExchange.OrderId = GenerateOrder(false);
        }

        #region GenerateOrder
        private int GenerateOrder(bool isStockBoardOrder)
        {
            int tempOrderId = 0;

            try
            {
                NewOrder nOrder = new NewOrder();
                nOrder.ClientId = orderDataExchange.PropertyOrder.ClientId;
                nOrder.AgentContactId = orderDataExchange.PropertyOrder.AgentContactId;

                nOrder.LocId = orderDataExchange.Property.LocationId;
                nOrder.Property = orderDataExchange.Property.PropertyAddressWithoutLocation;

                if (!isStockBoardOrder)
                {
                    if ((orderDataExchange.PropertyOrder.OrderHasBoard() || (orderDataExchange.PropertyOrder.OrderHasBoardOverlay() && orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference) || orderDataExchange.PropertyOrder.OrderHasBrochure()
                        || orderDataExchange.PropertyOrder.OrderHasWindowCard() || orderDataExchange.PropertyOrder.OrderHasCorflute() || orderDataExchange.PropertyOrder.OrderHasDIYSticker())
                        && orderDataExchange.PropertyOrder.IsDIYOrder
                        && string.IsNullOrEmpty(orderDataExchange.Client.Notes)
                        && string.IsNullOrEmpty(orderDataExchange.PropertyOrder.Notes))
                    {
                        nOrder.Caption = "DIY";
                    }
                    else
                    {
                        nOrder.Caption = orderDataExchange.PropertyOrder.TextDetails.Heading;
                    }

                    if (!orderDataExchange.PropertyOrder.OrderHasBoard() && !orderDataExchange.PropertyOrder.OrderHasBrochure() && !orderDataExchange.PropertyOrder.OrderHasCorflute()
                        && !orderDataExchange.PropertyOrder.OrderHasWindowCard() && orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck() && orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference)
                    {
                        nOrder.Caption = string.Empty;

                        if (string.IsNullOrEmpty(nOrder.Caption))
                        {
                            string namePlate = string.Empty;
                            var nItem = orderDataExchange.PropertyOrder.Cart.FindAll(c => c.TypeId == ProductTypes.Overlay && c.ProductId == ProductSettings.NamePlatesStockBoards);
                            if (nItem != null && nItem.Count > 0)
                            {
                                foreach (var itemx in nItem)
                                {
                                    if (itemx != null && itemx.ProductConfig != null)
                                    {
                                        if (string.IsNullOrEmpty(namePlate))
                                        {
                                            namePlate = itemx.GetValueByFieldName("NamePlateDetails");
                                        }
                                        else
                                        {
                                            namePlate = namePlate + " -- " + itemx.GetValueByFieldName("NamePlateDetails");
                                        }
                                    }
                                }
                            }

                            var boItem = orderDataExchange.PropertyOrder.Cart.FindAll(c => c.TypeId == ProductTypes.StockboardOverlay);
                            if (boItem != null && boItem.Count > 0)
                            {
                                foreach (var itemu in boItem)
                                {
                                    if (itemu != null && itemu.ProductConfig != null)
                                    {
                                        string fieldValue = itemu.GetValueByFieldName("NamePlateDetails");

                                        if (string.IsNullOrEmpty(namePlate))
                                        {
                                            namePlate = itemu.GetValueByFieldName("NamePlateDetails");
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(fieldValue))
                                            {
                                                namePlate = namePlate + " -- " + fieldValue;
                                            }
                                        }
                                    }
                                }
                            }

                            string name = orderDataExchange.PropertyOrder.GetStockboardName();
                            if (!string.IsNullOrEmpty(name))
                            {
                                if (!string.IsNullOrEmpty(namePlate))
                                    nOrder.Caption = name + " - " + namePlate;
                                else
                                    nOrder.Caption = name;
                            }
                            nOrder.Caption = "DIY";
                        }
                    }
                }
                else
                {
                    nOrder.Caption = string.Empty;
                    if (string.IsNullOrEmpty(nOrder.Caption))
                    {
                        string namePlate = string.Empty;
                        var nItem = orderDataExchange.PropertyOrder.Cart.FindAll(c => c.TypeId == ProductTypes.Overlay && c.ProductId == ProductSettings.NamePlatesStockBoards);
                        if (nItem != null && nItem.Count > 0)
                        {
                            foreach (var itemx in nItem)
                            {
                                if (itemx != null && itemx.ProductConfig != null)
                                {
                                    if (string.IsNullOrEmpty(namePlate))
                                    {
                                        namePlate = itemx.GetValueByFieldName("NamePlateDetails");
                                    }
                                    else
                                    {
                                        namePlate = namePlate + " -- " + itemx.GetValueByFieldName("NamePlateDetails");
                                    }
                                }
                            }
                        }

                        var boItem = orderDataExchange.PropertyOrder.Cart.FindAll(c => c.TypeId == ProductTypes.StockboardOverlay);
                        if (boItem != null && boItem.Count > 0)
                        {
                            foreach (var itemu in boItem)
                            {
                                if (itemu != null && itemu.ProductConfig != null)
                                {
                                    string fieldValue = itemu.GetValueByFieldName("NamePlateDetails");

                                    if (string.IsNullOrEmpty(namePlate))
                                    {
                                        namePlate = itemu.GetValueByFieldName("NamePlateDetails");
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(fieldValue))
                                        {
                                            namePlate = namePlate + " -- " + fieldValue;
                                        }
                                    }
                                }
                            }
                        }

                        var sboItem = orderDataExchange.PropertyOrder.Cart.FindAll(c => c.TypeId == ProductTypes.StockboardOverlay);
                        if (sboItem != null && sboItem.Count > 0)
                        {
                            foreach (var itemu in sboItem)
                            {
                                if (itemu != null && itemu.ProductConfig != null)
                                {
                                    string fieldValue = itemu.GetValueByFieldName("OverlayDetails");

                                    if (string.IsNullOrEmpty(namePlate))
                                    {
                                        namePlate = itemu.GetValueByFieldName("OverlayDetails");
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(fieldValue))
                                        {
                                            namePlate = namePlate + " -- " + fieldValue;
                                        }
                                    }
                                }
                            }
                        }

                        var sbItem = orderDataExchange.PropertyOrder.Cart.Find(c => c.TypeId == ProductTypes.Stockboard);
                        if (sbItem != null)
                        {
                            if (!string.IsNullOrEmpty(namePlate))
                                nOrder.Caption = sbItem.ProductName + " - " + namePlate;
                            else
                                nOrder.Caption = sbItem.ProductName;

                        }
                    }
                }

                nOrder.Notes = OnlineOrder.GetOrderNotes(orderDataExchange.PropertyOrder);
                nOrder.NoBoards = orderDataExchange.PropertyOrder.OrderOnlyHasNonBoardExcludeFlatPackItems();

                if (!nOrder.NoBoards)
                {

                    if (!orderDataExchange.PropertyOrder.OrderHasBoard() && orderDataExchange.PropertyOrder.OrderHasBrochure()
                        && orderDataExchange.PropertyOrder.OrderHasPackAndStockboardInsideThePack() && !orderDataExchange.PropertyOrder.IsDIYOrder && orderDataExchange.Client.ManagerID != "WORKS")
                    {
                        nOrder.NoBoards = true;
                    }
                }

                if (orderDataExchange.Property.IsRegularOrder)
                {
                    nOrder.NoBoards = true;
                }

                nOrder.ErectionNotes = OnlineOrder.GetErectionNotes(orderDataExchange.PropertyOrder);
                if (!string.IsNullOrEmpty(orderDataExchange.Client.ErectionNotes))
                {
                    nOrder.ErectionNotes = nOrder.ErectionNotes + " - " + orderDataExchange.Client.ErectionNotes;
                }
                nOrder.SendBy = orderDataExchange.PropertyOrder.SendProofBy;
                nOrder.SendTo = orderDataExchange.PropertyOrder.SendProofTo;
                nOrder.RefNo = orderDataExchange.PropertyOrder.ClientRefNumber;
                nOrder.TransformListing = orderDataExchange.PropertyOrder.IsListingonAbcRe;

                nOrder.InddTemplatesAvail = true;

                if (orderDataExchange.PropertyOrder.Cart.Exists(c => c.TypeId == ProductTypes.BillBoard || c.TypeId == ProductTypes.Stockboard
                                                                || c.TypeId == ProductTypes.BoardPackages || c.TypeId == ProductTypes.Packages))
                {

                    if (orderDataExchange.PropertyOrder.PreferredBoardErectionDate != null && orderDataExchange.PropertyOrder.PreferredBoardErectionType != PreferredDateType.NotSelected)
                    {
                        nOrder.PreferredErectionDate = orderDataExchange.PropertyOrder.PreferredBoardErectionDate.Value;
                        nOrder.PreferredErectionType = (int)orderDataExchange.PropertyOrder.PreferredBoardErectionType;
                    }
                    if (orderDataExchange.PropertyOrder.PreferredBoardRemovalDate != null && orderDataExchange.PropertyOrder.PreferredBoardRemovalType != PreferredDateType.NotSelected)
                    {
                        nOrder.PreferredRemovalDate = orderDataExchange.PropertyOrder.PreferredBoardRemovalDate.Value;
                        nOrder.PreferredRemovalType = (int)orderDataExchange.PropertyOrder.PreferredBoardRemovalType;
                    }
                }

                nOrder.PropertyId = orderDataExchange.PropertyOrder.PropertyId;
                nOrder.PropertyOrder = orderDataExchange.PropertyOrder;
                nOrder.ManagerId = orderDataExchange.Client.ManagerID;
                nOrder.IsStockBoardOrder = isStockBoardOrder;
                if (orderDataExchange.PropertyOrder.TextDetails != null)
                {
                    nOrder.HasTextReceived = orderDataExchange.PropertyOrder.TextDetails.HasTextReceived;
                    if (!string.IsNullOrEmpty(orderDataExchange.PropertyOrder.TextDetails.TextSelection) &&
                            orderDataExchange.PropertyOrder.TextDetails.TextSelection != "0")
                    {
                        nOrder.HasTextReceived = true;
                    }
                }

                nOrder.InstallFile = orderDataExchange.PropertyOrder.InstallationFile;

                nOrder.IsSiteInpectionRequired = orderDataExchange.PropertyOrder.IsSiteInspectionRequired;
                nOrder.SiteInspectionNotes = orderDataExchange.PropertyOrder.SiteInspectionNotes;
                nOrder.IsDIYOrder = orderDataExchange.PropertyOrder.IsDIYOrder;
                nOrder.IsExpressOrder = orderDataExchange.PropertyOrder.IsExpressOrder;

                //assign the delivery info value
                if (orderDataExchange.PropertyOrder.DeliveryInfo != null)
                {
                    nOrder.HasDeliveryDetails = true;
                    nOrder.DeliveryName = orderDataExchange.PropertyOrder.DeliveryInfo.Name;
                    nOrder.DeliveryEmail = orderDataExchange.PropertyOrder.DeliveryInfo.ContactEmail;
                    nOrder.DeliveryAddress = orderDataExchange.PropertyOrder.DeliveryInfo.StreetAddress;
                    nOrder.DeliverySuburb = orderDataExchange.PropertyOrder.DeliveryInfo.Suburb;
                    nOrder.DeliveryPostCode = orderDataExchange.PropertyOrder.DeliveryInfo.PostCode;
                    nOrder.DeliveryLocationId = orderDataExchange.PropertyOrder.DeliveryInfo.LocationId;
                    nOrder.DeliveryState = orderDataExchange.PropertyOrder.DeliveryInfo.State;
                    nOrder.ManagerState = orderDataExchange.Client.Manager.State;

                    nOrder.DeliveryPreference = orderDataExchange.PropertyOrder.DeliveryPref.ToString();

                    //Logger.Warn(nOrder.DeliverySuburb + " - " + nOrder.DeliveryPostCode + " - " + nOrder.DeliveryState + " - " + nOrder.DeliveryLocationId);

                    if (orderDataExchange.Property.IsRegularOrder)
                    {
                        if (orderDataExchange.PropertyOrder.MarketingDeliveryType == MarketingDeliveryType.ToOFfice)
                        {
                            nOrder.Notes = nOrder.Notes + " Delivery Address: " + orderDataExchange.PropertyOrder.OfficeDeliveryAddress;

                            nOrder.HasDeliveryDetails = true;
                            nOrder.DeliveryName = orderDataExchange.Client.ClientName;
                            nOrder.DeliveryAddress = orderDataExchange.Client.Address;
                            nOrder.DeliverySuburb = orderDataExchange.Client.Suburb;
                            nOrder.DeliveryPostCode = orderDataExchange.Client.PostCode;
                            nOrder.DeliveryLocationId = 0;
                            nOrder.DeliveryState = orderDataExchange.Client.State;
                            nOrder.ManagerState = orderDataExchange.Client.Manager.State;
                        }
                        else if (orderDataExchange.PropertyOrder.MarketingDeliveryType == MarketingDeliveryType.PickUp)
                        {
                            nOrder.Notes = " PLEASE BRING TO RECEPTION FOR COLLECTION !! " + nOrder.Notes;
                            nOrder.DeliveryPreference = "Pickup";
                        }
                        else if (orderDataExchange.PropertyOrder.MarketingDeliveryType == MarketingDeliveryType.OtherAddress)
                        {
                            nOrder.Notes = nOrder.Notes + " Delivery Address: " + orderDataExchange.PropertyOrder.MarketingDeliveryAddress.StreetNo + " " + orderDataExchange.PropertyOrder.MarketingDeliveryAddress.StreetName + " " +
                                        orderDataExchange.PropertyOrder.MarketingDeliveryAddress.Suburb + " " +
                                        orderDataExchange.PropertyOrder.MarketingDeliveryAddress.State + " " + orderDataExchange.PropertyOrder.MarketingDeliveryAddress.PostCode;

                            nOrder.HasDeliveryDetails = true;
                            nOrder.DeliveryName = orderDataExchange.Client.ClientName;
                            nOrder.DeliveryAddress = orderDataExchange.PropertyOrder.MarketingDeliveryAddress.StreetNo + " " + orderDataExchange.PropertyOrder.MarketingDeliveryAddress.StreetName;
                            nOrder.DeliverySuburb = orderDataExchange.PropertyOrder.MarketingDeliveryAddress.Suburb;
                            nOrder.DeliveryPostCode = orderDataExchange.PropertyOrder.MarketingDeliveryAddress.PostCode;
                            nOrder.DeliveryLocationId = 0;
                            nOrder.DeliveryState = orderDataExchange.PropertyOrder.MarketingDeliveryAddress.State;
                            nOrder.ManagerState = orderDataExchange.Client.Manager.State;
                        }
                    }
                }

                tempOrderId = OrderProcessor.CreateNewOrder(nOrder, orderDataExchange.Client.Manager.IsWorkshop);

            }
            catch (Exception ex)
            {
                Logger.Exception(ex, string.Format("{0}", orderDataExchange.PropertyOrder.GetHTMLString()));
                return 0;
            }
            return tempOrderId;
        }

        private string FormatPath(string path)
        {
            StringBuilder sb = new StringBuilder(path);
            sb.Replace("?", "~").Replace("*", "~").Replace(@"\", "~").Replace("/", "~").Replace(":", "~").Replace("\"", "~")
                                                                                                                                            .Replace("<", "~").Replace(">", "~").Replace("|", "~");

            return sb.ToString();
        }
        #endregion

        private void CreateOrderFile_ExecuteCode(object sender, EventArgs e)
        {
            FormatOrder formatOrder = new FormatOrder(orderDataExchange, OrderDataExchange.OrderId);

            if (OrderDataExchange.OrderId > 0)
            {
                fileName = formatOrder.FileName;
                try
                {
                    xmlFileName = formatOrder.FileNameXml;
                    WriteOrderFile(xmlFileName, formatOrder.GetXmlFileContents());
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, string.Format("{0} {1}", xmlFileName, formatOrder.GetXmlFileContents()));
                }
            }
            else
            {
                fileName = formatOrder.FileNameNoJobNo;
            }

            WriteOrderFile(fileName, formatOrder.GetRichTextFileContents(OrderDisplayType.NormalOrderOnly), true);
        }

        #region WriteOrderFile
        private bool WriteOrderFile(string fileNameToUse, string contents)
        {
            bool ret = false;
            System.IO.StreamWriter sw = null;
            int cc = 0;
            RetryCreate:

            try
            {
                sw = new StreamWriter(fileNameToUse);
                sw.WriteLine(contents);
                sw.Close();
                ret = true;
            }
            catch (Exception ex)
            {
                cc++; if (cc < 2) goto RetryCreate;
                Logger.Exception(ex, string.Format("{0} {1}"), fileNameToUse, contents);
            }

            return ret;
        }

        private bool WriteOrderFile(string fileNameToUse, string contents, bool useDefaultCoding)
        {
            bool ret = false;
            System.IO.StreamWriter sw = null;
            int cc = 0;
            RetryCreate:

            try
            {
                if (useDefaultCoding)
                    sw = new StreamWriter(fileNameToUse, false, System.Text.Encoding.ASCII);
                else
                    sw = new StreamWriter(fileNameToUse, false, System.Text.Encoding.UTF8);
                sw.WriteLine(contents);
                sw.Close();
                ret = true;
            }
            catch (Exception ex)
            {
                cc++; if (cc < 2) goto RetryCreate;
                Logger.Exception(ex, string.Format("{0} {1}"), fileNameToUse, contents);
            }

            return ret;
        }
        #endregion

        private void CheckIfOtherProduct(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.PropertyOrder.OrderHasOtherProductsOtherThanStockboardAndPhotography();
        }

        private void DIYCheckIfOtherProduct(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.PropertyOrder.DIYOrderHasOtherProductsOtherThanPhotographyAndFloorplan();
        }

        private void CheckIfB2BOrder(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.PropertyOrder.IsB2BOrder;
        }

        private void GenerateB2BOrderEvent_ExecuteCode(object sender, EventArgs e)
        {
            GenerateOrderEventWrapper(false, true, true);
        }

        private void GenerateOrderEvent_ExecuteCode(object sender, EventArgs e)
        {
            GenerateOrderEventWrapper(false, true, false);
        }

        private void GenerateOrderEventWrapper(bool forClient, bool forAbc, bool forB2B)
        {
            try
            {
                //if it's a SB Only Order then we will not gonna have OrderId. Need to use StockId instead.
                //this bug causing us to send email to Clients Email Address Rather then to SendProofTo Address
                int rightOrderId = orderDataExchange.OrderId;
                if (forClient == true && orderDataExchange.OrderId == 0 && orderDataExchange.StockId > 0)
                {
                    rightOrderId = orderDataExchange.StockId;
                }
                else if (forClient == true && orderDataExchange.OrderId == 0 && orderDataExchange.StockId == 0 && orderDataExchange.PhotoOrderId > 0)
                {
                    rightOrderId = orderDataExchange.PhotoOrderId;
                }

                FormatOrder formatOrder = new FormatOrder(orderDataExchange, rightOrderId);

                HtmlFormats html = formatOrder.GetHtmlFileContents();

                string attachments = "";
                IFile file = VirtualFileSystemFactory.GetFile();

                if (file.Exists(fileName)) attachments = fileName;

                NewOrderEvent nOrderEvent = new NewOrderEvent();

                nOrderEvent.OrderId = rightOrderId;
                nOrderEvent.ClientId = orderDataExchange.PropertyOrder.ClientId;

                if (forClient)
                    nOrderEvent.HtmlBody = html.ForClientNewEmail;
                else
                    nOrderEvent.HtmlBody = null;

                if (forB2B)
                {
                    nOrderEvent.HtmlBodyUS = html.ForB2B;
                }
                else if (forAbc)
                {
                    nOrderEvent.HtmlBodyUS = html.ForAbc;
                }
                else
                    nOrderEvent.HtmlBodyUS = null;


                nOrderEvent.FileName = attachments;
                nOrderEvent.Prop = orderDataExchange.Property.PropertyAddressWithSuburb;

                //Logger.Info("forAbc: " + forAbc + " OrderId: " + orderDataExchange.OrderId + " Notes: " + orderDataExchange.PropertyOrder.Notes);

                if (forAbc && orderDataExchange.OrderId > 0 && orderDataExchange.PropertyOrder.IsDIYOrder
                    && (orderDataExchange.PropertyOrder.OrderHasBoard() || ((orderDataExchange.PropertyOrder.OrderHasBoardOverlay() || orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck()) && orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference) || orderDataExchange.PropertyOrder.OrderHasBrochure()
                    || orderDataExchange.PropertyOrder.OrderHasWindowCard() || orderDataExchange.PropertyOrder.OrderHasCorflute() || orderDataExchange.PropertyOrder.OrderHasDIYSticker())
                    && string.IsNullOrEmpty(orderDataExchange.PropertyOrder.Notes))
                {
                    //check stockboard DIY order
                    if (forAbc && orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference && orderDataExchange.PropertyOrder.IsDIYOrder && !orderDataExchange.PropertyOrder.OrderHasBoard() && (orderDataExchange.PropertyOrder.OrderHasBoardOverlay() || orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck()))
                    {
                        OrderProcessor.GenerateNewSBDIYOrderEvent(nOrderEvent);
                    }
                    else
                    {
                        OrderProcessor.GenerateNewOrderNoNotesEvent(nOrderEvent);
                    }
                }
                else
                {
                    //check overlay sold sticker
                    if (forAbc && !orderDataExchange.PropertyOrder.IsDIYOrder && !orderDataExchange.PropertyOrder.OrderHasBoard() && !orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck()
                        && (orderDataExchange.PropertyOrder.OrderHasSoldSticker() || orderDataExchange.PropertyOrder.OrderHasOverlay() || orderDataExchange.PropertyOrder.OrderHasBoardOverlay()))
                    {
                        //check stockboard DIY order
                        if (forAbc && orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference && orderDataExchange.PropertyOrder.IsDIYOrder && !orderDataExchange.PropertyOrder.OrderHasBoard() && (orderDataExchange.PropertyOrder.OrderHasBoardOverlay() || orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck()))
                        {
                            OrderProcessor.GenerateNewSBDIYOrderEvent(nOrderEvent);
                        }
                        else
                        {
                            if (orderDataExchange.PropertyOrder.OrderHasOverlay() || orderDataExchange.PropertyOrder.OrderHasBoardOverlay())
                            {
                                OrderProcessor.GenerateNewOverlayStickerOrderEvent(nOrderEvent);
                            }
                            else if (orderDataExchange.PropertyOrder.OrderHasCorflute())
                            {
                                OrderProcessor.GenerateNewCorfluteOrderEvent(nOrderEvent);
                            }
                            else
                            {
                                OrderProcessor.GenerateNewMarketingOrderEvent(nOrderEvent);
                            }
                        }
                    }
                    else
                    {
                        //check stockboard DIY order
                        if (forAbc && orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference && orderDataExchange.PropertyOrder.IsDIYOrder && !orderDataExchange.PropertyOrder.OrderHasBoard() && (orderDataExchange.PropertyOrder.OrderHasBoardOverlay() || orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck()))
                        {
                            OrderProcessor.GenerateNewSBDIYOrderEvent(nOrderEvent);
                        }
                        else
                        {
                            if (forB2B)
                            {
                                if (orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck() || orderDataExchange.PropertyOrder.OrderHasBoardOverlay())
                                {
                                    OrderProcessor.GenerateNewOrderEvent(nOrderEvent);
                                }
                                else
                                {
                                    Logger.Info("New artwork OrderID: " + nOrderEvent.OrderId + " file: " + fileName);
                                }
                            }
                            else
                            {
                                if (forAbc && orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.OrderId > 0 && !orderDataExchange.PropertyOrder.OrderHasOtherProductNotJustPhotographyorFloorPlan()
                                    && orderDataExchange.PropertyOrder.OrderHasPhotographyorFloorPlan() && orderDataExchange.PropertyOrder.OrderHasPackAndPhotographyOrFloorplanInsideThePack())
                                {
                                    Logger.Warn("Photo Pack Order: " + orderDataExchange.OrderId + " -- " + orderDataExchange.PhotoOrderId + " -- for abc " + forAbc.ToString() + " -- " + orderDataExchange.Client.Manager.IsWorkshop.ToString());
                                }
                                else if (forAbc && orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasBoardInstallationExtension())
                                {
                                    Logger.Info("New extension OrderID: " + nOrderEvent.OrderId);
                                }
                                else if (forAbc && !orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasBoardInstallationExtension())
                                {
                                    Logger.Info("New extension OrderID: " + nOrderEvent.OrderId);
                                }
                                else if (forAbc && orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasVirtualTourExtension())
                                {
                                    Logger.Info("New virtual extension OrderID: " + nOrderEvent.OrderId);
                                }
                                else if (forAbc && orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasPackAndStockboardInsideThePack())
                                {
                                    OrderProcessor.GenerateSBOrderEvent(nOrderEvent);
                                }
                                else if (forAbc && orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.IsDIYOrder && orderDataExchange.PropertyOrder.OrderHasCommunityBoardOnly())
                                {
                                    OrderProcessor.GenerateNewCommunityBoardDIYOrderEvent(nOrderEvent);
                                }
                                else if (forAbc && (orderDataExchange.PropertyOrder.OrderDescription == "notemplate" || orderDataExchange.PropertyOrder.OrderDescription == "CreateSingleOrder"))
                                {
                                    Logger.Warn("OrderID: " + nOrderEvent.OrderId + " - " + orderDataExchange.PropertyOrder.OrderDescription);

                                    //OrderProcessor.GenerateNewOrderEvent(nOrderEvent);

                                    OrderProcessor.GenerateNewOrderNoTemplateEvent(nOrderEvent);
                                }
                                else
                                {
                                    //Logger.Warn(forAbc + " OrderID: " + nOrderEvent.OrderId + " - " + orderDataExchange.PropertyOrder.OrderDescription);

                                    OrderProcessor.GenerateNewOrderEvent(nOrderEvent);
                                }
                            }
                        }
                    }
                }

                if (forAbc && orderDataExchange.PropertyOrder.OrderHas3DLetteringBoard() && orderDataExchange.Client.ManagerID != ManagerSettings.WorkshopVictoria)
                {
                    //generate new event to send email to managers
                    OrderProcessor.Generate3DLetteringBoardOrderToManagerEvent(nOrderEvent);
                }

                if (orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference && orderDataExchange.PropertyOrder.IsDIYOrder && orderDataExchange.PropertyOrder.OrderHasBoard() && orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck())
                {
                    //send a new notification to admin
                    OrderProcessor.GenerateBoardAndStockboardInTheSameOrderEvent(nOrderEvent);
                }

            }
            catch (Exception ex)
            {
                Logger.Exception(ex, orderDataExchange.PropertyOrder.GetHTMLString());
            }
        }

        //Use Design_Document on new site
        private void ProcessAOPOrder_ExecuteCode(object sender, EventArgs e)
        {

            try
            {
                foreach (CartItem item in orderDataExchange.PropertyOrder.Cart)
                {
                    if (item.TypeId != ProductTypes.BoardPackages && item.TypeId != ProductTypes.OtherPackages && item.TypeId != ProductTypes.Packages)
                    {
                        int templateProductId = item.SelectedDIYTemplateId;
                        if (templateProductId <= 0)
                            continue;

                        OrderProcessor.SaveOrderDetailIdToDesignDocument(orderDataExchange.PropertyOrder.PropertyId, item);
                    }
                    else
                    {
                        List<int> templateIDs = new List<int>();

                        foreach (PackageGroup pgkitem in item.PackageGroups)
                        {
                            if (pgkitem.IsUpgradeProductApplicable)
                            {
                                int templateProductId = pgkitem.UpgradedProduct.SelectedDIYTemplateId;
                                if (templateProductId <= 0)
                                    continue;

                                OrderProcessor.SaveOrderDetailIdToDesignDocument(orderDataExchange.PropertyOrder.PropertyId, item);

                            }
                            else
                            {

                                foreach (PackageContentProduct contentProductItem in pgkitem.Products)
                                {

                                    int templateProductId = contentProductItem.SelectedDIYTemplateId;
                                    if (templateProductId > 0 && contentProductItem.UniqueId == pgkitem.SelectedUniqueId)
                                    {
                                        OrderProcessor.SaveOrderDetailIdToProductGroupDesignDocument(orderDataExchange.PropertyOrder.PropertyId, contentProductItem, pgkitem.GroupId); //contentProductItem
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, orderDataExchange.PropertyOrder.PropertyId.ToString());
            }

        }

        private void CheckIfPhotography(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.PropertyOrder.OrderHasPhotographyorFloorPlan();
        }

        private void CheckIfWorkshopClient(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.Client.Manager.IsWorkshop;
        }

        private void CheckIfDIYOrder(object sender, ConditionalEventArgs e)
        {
            //e.Result = (orderDataExchange.PropertyOrder.IsDIYOrder && orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference);
            e.Result = orderDataExchange.PropertyOrder.IsDIYOrder;
        }

        private void GeneratePhotoOrderId_ExecuteCode(object sender, EventArgs e)
        {
            orderDataExchange.PhotoOrderId = GeneratePhoto();
        }

        #region GeneratePhoto
        private int GeneratePhoto()
        {
            int tempPhotoOrderId = 0;

            try
            {
                string addNoteAll = string.Empty;

                if (!string.IsNullOrEmpty(orderDataExchange.PropertyOrder.Notes))
                    addNoteAll = orderDataExchange.PropertyOrder.Notes + "\r\n";

                // Try to get the photoOrder from Cart
                CartItem photoOrder = null;
                PackageContentProduct photoProductOrder = null;
                foreach (CartItem item in orderDataExchange.PropertyOrder.Cart)
                {
                    if (item.TypeId == ProductTypes.Photography || item.TypeId == ProductTypes.FloorPlans)
                    {
                        photoOrder = item;
                        break;
                    }
                }

                if (photoOrder == null)
                {
                    foreach (CartItem item in orderDataExchange.PropertyOrder.Cart)
                    {
                        if (item.TypeId == ProductTypes.Packages || item.TypeId == ProductTypes.BoardPackages || item.TypeId == ProductTypes.OtherPackages)
                        {
                            foreach (PackageGroup itemGroup in item.PackageGroups)
                            {
                                foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                                {
                                    if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                                    {
                                        if (contentProductItem.TypeId == ProductTypes.Photography || contentProductItem.TypeId == ProductTypes.FloorPlans)
                                        {
                                            photoProductOrder = contentProductItem;
                                            break;
                                        }
                                    }
                                }
                                if (photoProductOrder != null)
                                    break;
                            }
                        }
                        if (photoProductOrder != null)
                            break;
                    }
                }

                NewPhotoOrder nPhotoOrder = new NewPhotoOrder();

                nPhotoOrder.ClientId = orderDataExchange.Client.ClientID;
                nPhotoOrder.LocId = orderDataExchange.Property.LocationId;
                nPhotoOrder.Property = orderDataExchange.Property.PropertyAddressWithoutLocation;
                nPhotoOrder.Notes = addNoteAll;
                nPhotoOrder.ErectionNotes = orderDataExchange.PropertyOrder.ErectionNotes;
                nPhotoOrder.RefNo = orderDataExchange.PropertyOrder.ClientRefNumber;

                string caption = string.Empty;
                bool hasPhoto = false;
                bool hasColor = false;
                bool hasRedraw = false;
                bool hasFloorPlan = false;
                bool hasSitePlan = false;
                foreach (CartItem item in orderDataExchange.PropertyOrder.Cart)
                {
                    if (item.TypeId == ProductTypes.Photography)
                    {
                        hasPhoto = true;
                    }
                    else if (item.TypeId == ProductTypes.FloorPlans)
                    {
                        if (item.ProductName.ToLower().Contains("colour"))
                        {
                            hasColor = true;
                        }
                        if (item.ProductName.ToLower().Contains("re-draw"))
                        {
                            hasRedraw = true;
                        }
                        else if (item.ProductName.ToLower().Contains("floor"))
                        {
                            hasFloorPlan = true;
                        }
                        if (item.ProductName.ToLower().Contains("site"))
                        {
                            hasSitePlan = true;
                        }
                    }
                    else if (item.TypeId == ProductTypes.Packages || item.TypeId == ProductTypes.BoardPackages || item.TypeId == ProductTypes.OtherPackages)
                    {
                        foreach (PackageGroup itemGroup in item.PackageGroups)
                        {
                            foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                            {
                                if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                                {
                                    if (contentProductItem.TypeId == ProductTypes.Photography)
                                    {
                                        hasPhoto = true;
                                    }
                                    else if (contentProductItem.TypeId == ProductTypes.FloorPlans)
                                    {
                                        if (contentProductItem.ProductName.ToLower().Contains("colour"))
                                        {
                                            hasColor = true;
                                        }
                                        if (contentProductItem.ProductName.ToLower().Contains("floor"))
                                        {
                                            hasFloorPlan = true;
                                        }
                                        if (contentProductItem.ProductName.ToLower().Contains("site"))
                                        {
                                            hasSitePlan = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (hasPhoto)
                    caption = "Photos and ";
                if (hasColor)
                    caption += "colour ";
                if (hasFloorPlan)
                    caption += "floor plan and ";
                if (hasSitePlan)
                    caption += "site plan and ";
                if (hasRedraw)
                    caption += "Re-draw floor plan";

                if (!string.IsNullOrEmpty(caption) && caption.EndsWith("and "))
                    caption = caption.Substring(0, caption.Length - 5);
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Substring(0, 1).ToUpper() + caption.Substring(1).ToLower();
                nPhotoOrder.Caption = caption;

                //P1: Create a helper method to generate those fileds
                if (photoOrder != null)
                {
                    nPhotoOrder.Instructions = photoOrder.GetValueByFieldName("OtherInstructions");
                    nPhotoOrder.VendorName = photoOrder.GetValueByFieldName("ContactName");
                    nPhotoOrder.VendorPhone = photoOrder.GetValueByFieldName("ContactPhone");
                    if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("KeySafe")))
                    {
                        nPhotoOrder.IsKeySafe = bool.Parse(photoOrder.GetValueByFieldName("KeySafe"));
                    }
                    if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("PickupKeys")))
                    {
                        nPhotoOrder.IsPickupKeys = bool.Parse(photoOrder.GetValueByFieldName("PickupKeys"));
                    }
                    nPhotoOrder.PhotoContact = photoOrder.GetValueByFieldName("ArrangeWith");
                    nPhotoOrder.HouseFaces = photoOrder.GetValueByFieldName("HouseFaces");
                    nPhotoOrder.Melway = photoOrder.GetValueByFieldName("MapRef");

                    if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("DimensionInputOption")))
                    {
                        string inputDimOptionval = photoOrder.GetValueByFieldName("DimensionInputOption");
                        if (inputDimOptionval == "Supply Dimension")
                        {
                            StringBuilder sbDimensionNotes = new StringBuilder();
                            sbDimensionNotes.Append(string.Format("Input Dimensions Option - {0}", inputDimOptionval));
                            sbDimensionNotes.Append("::");
                            if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("Frontage")))
                            {
                                sbDimensionNotes.Append(string.Format("Frontage - {0}, ", photoOrder.GetValueByFieldName("Frontage")));
                            }
                            if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("LeftBorder")))
                            {
                                sbDimensionNotes.Append(string.Format("Left Border - {0}, ", photoOrder.GetValueByFieldName("LeftBorder")));
                            }
                            if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("RightBorder")))
                            {
                                sbDimensionNotes.Append(string.Format("Right Border - {0}, ", photoOrder.GetValueByFieldName("RightBorder")));
                            }
                            if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("RearBorder")))
                            {
                                sbDimensionNotes.Append(string.Format("Rear Border - {0}, ", photoOrder.GetValueByFieldName("RearBorder")));
                            }
                            if (!string.IsNullOrEmpty(photoOrder.GetValueByFieldName("ApproximateSquareMeters")))
                            {
                                sbDimensionNotes.Append(string.Format("Approximate Square Meters - {0}", photoOrder.GetValueByFieldName("ApproximateSquareMeters")));
                            }
                            nPhotoOrder.Instructions = nPhotoOrder.Instructions + " -- " + sbDimensionNotes.ToString();
                        }
                    }
                }
                else if (photoProductOrder != null)
                {
                    nPhotoOrder.Notes = nPhotoOrder.Notes + " -- NC part of pack " + orderDataExchange.OrderId.ToString();
                    nPhotoOrder.Instructions = photoProductOrder.GetValueByFieldName("OtherInstructions");
                    nPhotoOrder.VendorName = photoProductOrder.GetValueByFieldName("ContactName");
                    nPhotoOrder.VendorPhone = photoProductOrder.GetValueByFieldName("ContactPhone");
                    if (!string.IsNullOrEmpty(photoProductOrder.GetValueByFieldName("KeySafe")))
                    {
                        nPhotoOrder.IsKeySafe = bool.Parse(photoProductOrder.GetValueByFieldName("KeySafe"));
                    }
                    if (!string.IsNullOrEmpty(photoProductOrder.GetValueByFieldName("PickupKeys")))
                    {
                        nPhotoOrder.IsPickupKeys = bool.Parse(photoProductOrder.GetValueByFieldName("PickupKeys"));
                    }
                    nPhotoOrder.PhotoContact = photoProductOrder.GetValueByFieldName("ArrangeWith");
                    nPhotoOrder.HouseFaces = photoProductOrder.GetValueByFieldName("HouseFaces");
                    nPhotoOrder.Melway = photoProductOrder.GetValueByFieldName("MapRef");
                }

                nPhotoOrder.SendBy = orderDataExchange.PropertyOrder.SendProofBy;
                nPhotoOrder.SendTo = orderDataExchange.PropertyOrder.SendProofTo;
                nPhotoOrder.ContactName = orderDataExchange.PropertyOrder.ContactDetailName;
                if (orderDataExchange.PropertyOrder.ContactDetailName.Length > 20)
                {
                    nPhotoOrder.ContactName = orderDataExchange.PropertyOrder.ContactDetailName.Substring(0, 19);
                }
                nPhotoOrder.ContactNumber = orderDataExchange.PropertyOrder.ContactNumber;

                nPhotoOrder.PropertyId = orderDataExchange.PropertyOrder.PropertyId;

                nPhotoOrder.PropertyOrder = orderDataExchange.PropertyOrder;


                nPhotoOrder.SitePlanInstructionFile = orderDataExchange.PropertyOrder.SitePlanInstructionFile;

                nPhotoOrder.PhotographyFile = orderDataExchange.PropertyOrder.PhotographyFile;
                if (!string.IsNullOrEmpty(orderDataExchange.PropertyOrder.LandOverlayDescription))
                    nPhotoOrder.Notes += " Drone Photography Description: " + orderDataExchange.PropertyOrder.LandOverlayDescription;


                if (orderDataExchange.PropertyOrder.ClientHasStockboardDIYPreference && orderDataExchange.OrderId > 0 && orderDataExchange.PropertyOrder.IsDIYOrder
                    && orderDataExchange.PropertyOrder.OrderHasPhotographyorFloorPlan() && orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck())
                {
                    nPhotoOrder.OrderId = orderDataExchange.OrderId;
                }

                tempPhotoOrderId = OrderProcessor.CreateNewPhotoOrder(nPhotoOrder);

            }
            catch (Exception ex)
            {
                Logger.Exception(ex, string.Format("{0}", orderDataExchange.PropertyOrder.GetHTMLString()));
                return 0;
            }

            return tempPhotoOrderId;
        }
        #endregion

        private void CreatePhotoFile_ExecuteCode(object sender, EventArgs e)
        {
            FormatOrder formatOrder = new FormatOrder(orderDataExchange, orderDataExchange.PhotoOrderId);

            if (orderDataExchange.PhotoOrderId > 0)
            {
                photoFileName = formatOrder.FileNamePhoto;
            }
            else
            {
                photoFileName = formatOrder.FileNamePhotoNoJobNo;
            }

            WriteOrderFile(photoFileName, formatOrder.GetRichTextFileContents(OrderDisplayType.PhotographyOrderOnly), true);
        }

        private void GeneratePhotoEvent_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                FormatOrder formatOrder = new FormatOrder(orderDataExchange, orderDataExchange.PhotoOrderId);
                HtmlFormats html = formatOrder.GetHtmlFileContents();

                NewOrderEvent nOrderEvent = new NewOrderEvent();

                if (orderDataExchange.PhotoOrderId > 0)
                    nOrderEvent.OrderId = orderDataExchange.PhotoOrderId;
                else
                    nOrderEvent.OrderId = 0;

                nOrderEvent.ClientId = orderDataExchange.Client.ClientID;
                nOrderEvent.HtmlBody = html.ForClient;
                nOrderEvent.HtmlBodyUS = html.ForPhotography;
                nOrderEvent.FileName = (File.Exists(photoFileName) ? photoFileName : "");
                nOrderEvent.Prop = orderDataExchange.Property.PropertyAddressWithSuburb;

                nOrderEvent.PhotoEmail = formatOrder.GetTextFileContents(OrderDisplayType.PhotographyOrderOnly);
                nOrderEvent.OrderHasMudMapOrReDrawFloorplan = orderDataExchange.PropertyOrder.OrderHasMudMapOrReDrawFloorplan();
                nOrderEvent.OrderHasVirtualWalkThrough = orderDataExchange.PropertyOrder.OrderHasVirtualWalkThrough();
                nOrderEvent.OrderHasProfessionalSlideshowVideo = orderDataExchange.PropertyOrder.OrderHasProfessionalSlideshowVideo();
                nOrderEvent.SendProofToEmail = orderDataExchange.PropertyOrder.SendProofTo;

                if (orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.OrderId > 0 && orderDataExchange.PhotoOrderId > 0 && orderDataExchange.StockId == 0 && !orderDataExchange.PropertyOrder.OrderHasOtherProductNotJustPhotographyorFloorPlan())
                {
                    //Logger.Warn("Photo Order email: " + orderDataExchange.OrderId + " -- " + orderDataExchange.PhotoOrderId);
                    nOrderEvent.SendPhotoEmailToAdmin = false;
                }
                else
                {
                    nOrderEvent.SendPhotoEmailToAdmin = true;
                }

                OrderProcessor.GenerateNewPhotoOrderEvent(nOrderEvent);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, string.Format("{0} {1}"), new Object[] { orderDataExchange.PropertyOrder.GetHTMLString() });
            }
        }

        private void CheckIfStockBoard(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.PropertyOrder.OrderHasStockboard();
        }

        private void CheckIfWorkshopClientForSB(object sender, ConditionalEventArgs e)
        {
            e.Result = orderDataExchange.Client.Manager.IsWorkshop;
        }

        private void GenerateNormalStockId_ExecuteCode(object sender, EventArgs e)
        {
            orderDataExchange.StockId = GenerateOrder(true);
        }

        private void GenerateManagerStockId_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.Client.Manager.ManageOwnBoards)
            {
                try
                {
                    NewStockBoardOrder nSBOrder = new NewStockBoardOrder();

                    nSBOrder.ClientId = orderDataExchange.Client.ClientID;
                    nSBOrder.State = orderDataExchange.Property.Location.State;
                    nSBOrder.Loc = orderDataExchange.Property.Location.Location1;
                    nSBOrder.Property = orderDataExchange.Property.PropertyAddressWithoutLocation;
                    nSBOrder.ManagerID = orderDataExchange.Client.ManagerID;
                    nSBOrder.Notes = orderDataExchange.PropertyOrder.Notes;

                    if (!string.IsNullOrEmpty(orderDataExchange.PropertyOrder.ErectionNotes))
                    {
                        if (!string.IsNullOrEmpty(nSBOrder.Notes))
                        {
                            nSBOrder.Notes += " -- " + orderDataExchange.PropertyOrder.ErectionNotes;
                        }
                        else
                        {
                            nSBOrder.Notes = orderDataExchange.PropertyOrder.ErectionNotes;
                        }

                        if (!string.IsNullOrEmpty(orderDataExchange.Client.ErectionNotes))
                        {
                            nSBOrder.Notes += " - " + orderDataExchange.Client.ErectionNotes;
                        }
                    }

                    if (orderDataExchange.PropertyOrder.PreferredBoardErectionDate != null && orderDataExchange.PropertyOrder.PreferredBoardErectionType != PreferredDateType.NotSelected)
                    {
                        nSBOrder.PreferredErectionDate = orderDataExchange.PropertyOrder.PreferredBoardErectionDate.Value;
                        nSBOrder.PreferredErectionType = (int)orderDataExchange.PropertyOrder.PreferredBoardErectionType;
                    }

                    nSBOrder.PropertyId = orderDataExchange.Property.PropertyId;
                    nSBOrder.PropertyOrder = orderDataExchange.PropertyOrder;
                    nSBOrder.AgentContactId = orderDataExchange.PropertyOrder.AgentContactId;

                    orderDataExchange.StockId = OrderProcessor.CreateNewSBOrder(nSBOrder);

                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, string.Format("{0}"), new Object[] { orderDataExchange.PropertyOrder.GetHTMLString() });
                    orderDataExchange.StockId = 0;
                }
            }
        }

        private void CreateStockFile_ExecuteCode(object sender, EventArgs e)
        {
            FormatOrder formatOrder = new FormatOrder(orderDataExchange, orderDataExchange.StockId);

            if (orderDataExchange.StockId > 0)
            {
                stockFileName = formatOrder.FileNameStock;
            }
            else
            {
                stockFileName = formatOrder.FileNameStockNoJobNo;
            }

            WriteOrderFile(stockFileName, formatOrder.GetRichTextFileContents(OrderDisplayType.StockboardOrderOnly), true);
        }

        private void GenerateStockEvent_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                FormatOrder formatOrder = new FormatOrder(orderDataExchange, orderDataExchange.StockId);
                HtmlFormats html = formatOrder.GetHtmlFileContents();

                NewOrderEvent nOrderEvent = new NewOrderEvent();
                if (orderDataExchange.StockId > 0)
                    nOrderEvent.OrderId = orderDataExchange.StockId;
                else
                    nOrderEvent.OrderId = 0;

                nOrderEvent.ClientId = orderDataExchange.Client.ClientID;
                nOrderEvent.GroupId = orderDataExchange.Client.GroupId;
                nOrderEvent.HtmlBody = html.ForClient;
                nOrderEvent.HtmlBodyUS = html.ForStockboard;
                nOrderEvent.Notes = orderDataExchange.PropertyOrder.Notes;
                nOrderEvent.OrderHasOverlayExcludeUnitStickerAndNamePlates = orderDataExchange.PropertyOrder.OrderHasOverlayExcludeUnitStickerAndNamePlates();
                nOrderEvent.InterstateOrderHasOverlayExcludeUnitStickerAndNamePlates = orderDataExchange.PropertyOrder.InterstateOrderHasOverlayExcludeUnitStickerAndNamePlates();
                nOrderEvent.OrderHasStockboard = orderDataExchange.PropertyOrder.OrderHasStockboard();
                nOrderEvent.OrderHasUnitStickerForStockBoardProduct = orderDataExchange.PropertyOrder.OrderHasUnitStickerForStockBoardProduct();
                nOrderEvent.InterstateOrderHasOverlayOrUnitStickerOrNamePlates = orderDataExchange.PropertyOrder.InterstateOrderHasOverlayOrUnitStickerOrNamePlates();
                nOrderEvent.ManagerID = orderDataExchange.Client.ManagerID;
                IFile file = VirtualFileSystemFactory.GetFile();

                if (file.Exists(stockFileName))
                    nOrderEvent.FileName = stockFileName;
                else
                    nOrderEvent.FileName = "";

                nOrderEvent.Prop = orderDataExchange.Property.PropertyAddressWithSuburb;
                nOrderEvent.OrderType = orderDataExchange.PropertyOrder.OrderType;
                nOrderEvent.InstallFile = orderDataExchange.PropertyOrder.InstallationFile;

                if (orderDataExchange.PropertyOrder.PreferredBoardErectionDate != null && orderDataExchange.PropertyOrder.PreferredBoardErectionDate.HasValue)
                {
                    DateTime now = DateTime.Now;
                    DateTime later = orderDataExchange.PropertyOrder.PreferredBoardErectionDate.Value;

                    int days = (later - now).Days;

                    if (days > 4)
                        nOrderEvent.SendJobsheetToPrintTeam = false;
                    else
                        nOrderEvent.SendJobsheetToPrintTeam = true;
                }
                else
                {
                    nOrderEvent.SendJobsheetToPrintTeam = true;
                }

                OrderProcessor.GenerateNewSBOrderEvent(nOrderEvent);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, orderDataExchange.PropertyOrder.GetHTMLString());
            }
        }

        private void NotifyClient_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.OrderId > 0 || orderDataExchange.PhotoOrderId > 0 || orderDataExchange.StockId > 0)
            {
                if (orderDataExchange.Client.ClientID == ClientSettings.BuyMyPlaceSouthMelbourne)
                {
                    string deliveryEmail = orderDataExchange.PropertyOrder.SendProofTo;
                    if (orderDataExchange.PropertyOrder.DeliveryInfo != null && !string.IsNullOrEmpty(orderDataExchange.PropertyOrder.DeliveryInfo.ContactEmail))
                    {
                        deliveryEmail = orderDataExchange.PropertyOrder.DeliveryInfo.ContactEmail;
                    }

                    int rightOrderId = orderDataExchange.OrderId;
                    if (orderDataExchange.OrderId > 0)
                    {
                        OrderProcessor.GenerateBmpOrderConfirmationEmail(orderDataExchange.OrderId, deliveryEmail, string.Empty, "support@buymyplace.com.au");
                    }
                    if (orderDataExchange.StockId > 0)
                    {
                        OrderProcessor.GenerateBmpOrderConfirmationEmail(orderDataExchange.StockId, deliveryEmail, string.Empty, "support@buymyplace.com.au");
                    }
                    if (orderDataExchange.PhotoOrderId > 0)
                    {
                        OrderProcessor.GenerateBmpOrderConfirmationEmail(orderDataExchange.PhotoOrderId, deliveryEmail, string.Empty, "support@buymyplace.com.au");
                    }
                }
                else if (!orderDataExchange.PropertyOrder.IsB2BOrder)
                {
                    GenerateOrderEventWrapper(true, false, false);
                }

            }
        }

        private void ErectionFeeCheck_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.PropertyOrder.BoardInstallationType == BoardInstallationType.High
                || orderDataExchange.PropertyOrder.BoardInstallationType == BoardInstallationType.HigherThanFirstLevel
                || orderDataExchange.PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed500mmOfTheGround
                || orderDataExchange.PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1000mmOfTheGround
                || orderDataExchange.PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1250mmOfTheGround
                || orderDataExchange.PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed2000mmOfTheGround)
            {
                if (orderDataExchange.Client.Manager.IsWorkshop)
                {
                    CartItem cItem = new CartItem();
                    cItem.ProductId = OnlineBLConfig.ERECTION_FEE_PRODUCT_ID;
                    cItem.ProductName = "High Installation";
                    cItem.TypeId = ProductTypes.BoardAccessory;
                    cItem.WebFriendlyName = "High Installation";
                    cItem.ItemQty = 1;
                    orderDataExchange.PropertyOrder.Cart.Add(cItem);
                }
            }
        }

        private void WFExceptionHandler_ExecuteCode(object sender, EventArgs e)
        {
            if (wfFaultHandlerActivity.Fault != null)
                Logger.Exception(wfFaultHandlerActivity.Fault, "Error on Order Processing Workflow");
        }

        private void CreateClientAccount_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.PropertyOrder.OrderHasPhotographyorFloorPlan() || orderDataExchange.PropertyOrder.OrderHasSpotlight())
                OrderProcessor.CreateClientPersonalAccount(orderDataExchange.PropertyOrder.ClientId);
        }

        private void TravelFeeCheck_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.Property != null && orderDataExchange.Property.TravelFee.HasValue && orderDataExchange.Property.TravelFee.Value > (decimal)0.5)
            {
                if (!orderDataExchange.PropertyOrder.OrderHasBoardInstallationExtension())
                {
                    CartItem cItem = new CartItem();
                    cItem.ProductId = ProductSettings.Travel;
                    cItem.ProductName = "Travel";
                    cItem.TypeId = ProductTypes.BoardAccessory;
                    cItem.WebFriendlyName = "Travel";
                    cItem.ItemQty = 1;
                    cItem.ProductPrice = orderDataExchange.Property.TravelFee.Value;
                    orderDataExchange.PropertyOrder.Cart.Add(cItem);
                }
            }
        }

        private void NotifyAccount_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.OrderId > 0 || orderDataExchange.PhotoOrderId > 0 || orderDataExchange.StockId > 0)
            {
                int rightOrderId = orderDataExchange.OrderId;
                if (orderDataExchange.OrderId == 0 && orderDataExchange.StockId > 0)
                {
                    rightOrderId = orderDataExchange.StockId;
                }
                else if (orderDataExchange.OrderId == 0 && orderDataExchange.StockId == 0 && orderDataExchange.PhotoOrderId > 0)
                {
                    rightOrderId = orderDataExchange.PhotoOrderId;
                }

                if (rightOrderId < 99000000)
                {
                    OrderProcessor.GenerateNotifyAccountEvent(rightOrderId);
                }

            }
        }

        private void PutOrderOnHold_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.Client.GroupId == ClientGroupSettings.OxbridgePropertyGroup)
            {
                if (orderDataExchange.OrderId > 0)
                {
                    OrderProcessor.PutOrderOnHold(orderDataExchange.OrderId);
                    OrderProcessor.GenerateOxbridgeEmailNeedApproval(orderDataExchange.OrderId, orderDataExchange.PropertyOrder.ContactDetailName.Replace("&", " "));
                }
                if (orderDataExchange.PhotoOrderId > 0)
                {
                    OrderProcessor.PutOrderOnHold(orderDataExchange.PhotoOrderId);
                    OrderProcessor.GenerateOxbridgeEmailNeedApproval(orderDataExchange.PhotoOrderId, orderDataExchange.PropertyOrder.ContactDetailName.Replace("&", " "));
                }
                if (orderDataExchange.StockId > 0)
                {
                    OrderProcessor.PutOrderOnHold(orderDataExchange.StockId);
                    OrderProcessor.GenerateOxbridgeEmailNeedApproval(orderDataExchange.StockId, orderDataExchange.PropertyOrder.ContactDetailName.Replace("&", " "));
                }

                int rightOrderId = orderDataExchange.OrderId;
                if (orderDataExchange.OrderId == 0 && orderDataExchange.StockId > 0)
                {
                    rightOrderId = orderDataExchange.StockId;
                }
                else if (orderDataExchange.OrderId == 0 && orderDataExchange.StockId == 0 && orderDataExchange.PhotoOrderId > 0)
                {
                    rightOrderId = orderDataExchange.PhotoOrderId;
                }

                if (rightOrderId > 0)
                {
                    OrderProcessor.GenerateEmailOxbridgeOrderOnHold(rightOrderId, orderDataExchange.PropertyOrder.SendProofTo);
                }
            }
        }

        private void LinkToOriginalOrder_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasPackAndStockboardInsideThePack())
            {
                if (orderDataExchange.StockId > 0)
                {
                    if (orderDataExchange.StockId < 99000000)
                        OrderProcessor.GenerateOrderLink(orderDataExchange.StockId, orderDataExchange.OrderId);
                }
            }

            if (orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasPackAndPhotographyOrFloorplanInsideThePack())
            {
                if (orderDataExchange.PhotoOrderId > 0)
                    OrderProcessor.GenerateOrderLink(orderDataExchange.PhotoOrderId, orderDataExchange.OrderId);
            }

            if (orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.OrderId > 0 && orderDataExchange.PhotoOrderId > 0 && orderDataExchange.PropertyOrder.OrderHasPackAndPhotographyOrFloorplanInsideThePack())
            {
                //Logger.Warn("Check to see if we can Approve Order: " + orderDataExchange.OrderId + " -- " + orderDataExchange.PhotoOrderId);
                OrderProcessor.GenerateApprovePhotoPackOrderQueue(orderDataExchange.OrderId);
            }
        }

        private void SendPaygInvoice_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.OrderId > 0)
            {
                OrderProcessor.SendPaygInvoiceToClient(orderDataExchange.OrderId);
            }
            if (orderDataExchange.PhotoOrderId > 0)
            {
                OrderProcessor.SendPaygInvoiceToClient(orderDataExchange.PhotoOrderId);
            }
            if (orderDataExchange.StockId > 0)
            {
                OrderProcessor.SendPaygInvoiceToClient(orderDataExchange.StockId);
            }
        }

        private void CheckClientProduct_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.PropertyOrder.OrderHasBoard() || orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck())
            {
                if (orderDataExchange.OrderId > 0)
                {
                    OrderProcessor.UpdateOrderOtherDetail(orderDataExchange.OrderId, orderDataExchange.PropertyOrder.HasInstallFile);
                }
            }
        }

        private void BoardAccessoryCheck_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.PropertyOrder.OrderHasBoard() == false && orderDataExchange.PropertyOrder.OrderHasStockboardIncludePackageCheck() == false
                && (orderDataExchange.PropertyOrder.OrderHasOverlay() == true || orderDataExchange.PropertyOrder.OrderHasBoardOverlay() == true)
                && orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasCustomOverlayInstalled() == false)
            {
                Logger.Warn("Overlay order: " + orderDataExchange.PropertyOrder.ClientId + " -- " + orderDataExchange.Property.PropertyAddressWithSuburb);

                CartItem cItem = new CartItem();
                cItem.ProductId = 19635;
                cItem.ProductName = "Install of Board Accessory";
                cItem.TypeId = ProductTypes.BoardAccessory;
                cItem.WebFriendlyName = "Install of Board Accessory";
                cItem.ItemQty = 1;
                orderDataExchange.PropertyOrder.Cart.Add(cItem);
            }
        }

        private void AddOrderNotes_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.OrderId > 0)
            {
                OrderProcessor.CheckFlagHolderToAddNotes(orderDataExchange.OrderId);
            }
        }

        private void ApplyDeliveryFee_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.Client.ClientID == ClientSettings.BarryPlantBendigo || orderDataExchange.Client.ClientID == ClientSettings.McKeanMcGregorBendigo)
            {
                if (orderDataExchange.OrderId > 0 && (orderDataExchange.PropertyOrder.OrderHasBrochure() || orderDataExchange.PropertyOrder.OrderHasWindowCard()))
                {
                    OrderProcessor.GenerateOrderDeliveryFee(orderDataExchange.OrderId);
                }
            }
            else if (orderDataExchange.OrderId > 0 && orderDataExchange.PropertyOrder.OrderHasA3WindowCard() && orderDataExchange.PropertyOrder.Cart.Count == 1)
            {
                OrderProcessor.GenerateOrderDeliveryFee(orderDataExchange.OrderId);
            }
        }

        private void CheckManager_ExecuteCode(object sender, EventArgs e)
        {
            if (orderDataExchange.Property != null && !string.IsNullOrWhiteSpace(orderDataExchange.Property.TravelManagerId) && orderDataExchange.Property.TravelManagerId != orderDataExchange.Client.ManagerID)
            {
                if (orderDataExchange.OrderId > 0)
                {
                    OrderProcessor.UpdateManagerForRegionalProperty(orderDataExchange.OrderId, orderDataExchange.Property.TravelManagerId);
                }
                if (orderDataExchange.StockId > 0)
                {
                    OrderProcessor.UpdateManagerForRegionalProperty(orderDataExchange.StockId, orderDataExchange.Property.TravelManagerId);
                }
            }
            else if (orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.Client.Manager.ManagerID != "INHOU" && orderDataExchange.OrderId > 0
                && !orderDataExchange.PropertyOrder.OrderHasBoard() && orderDataExchange.PropertyOrder.OrderHasBrochure() && orderDataExchange.PropertyOrder.OrderHasPackAndStockboardInsideThePack())
            {
                OrderProcessor.CheckManager(orderDataExchange.OrderId);
            }

        }

        private void GenerateRegionalPhotoOrder_ExecuteCode(object sender, EventArgs e)
        {
            Logger.Warn("Regional Photo order: " + orderDataExchange.PropertyOrder.ClientId + " -- " + orderDataExchange.Property.PropertyAddressWithSuburb);
            if (!orderDataExchange.Client.Manager.IsWorkshop && orderDataExchange.PropertyOrder.OrderHasMudMapOrReDrawFloorplan())
            {
                orderDataExchange.PhotoOrderId = GeneratePhoto();
            }
        }

    }
}
