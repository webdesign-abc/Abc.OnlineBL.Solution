using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.Photography;
using Abc.OnlineBL.Utility;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Abc.OnlineBL.Service.Implementation
{
    public class PhotoSysRunSheetService : IPhotoSysRunSheetService
    {
        #region OLD
        /// <summary>
        /// Gets the todays photo sys run sheet.
        /// </summary>
        /// <param name="photographerId">The photographer id.</param>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        /// <returns>
        /// Returns the list of the PHOTOSYS_RunsheetResult obj.
        /// </returns>
        public List<PhotoSysRunSheet> GetPhotoSysRunSheet(int photographerId, DateTime dateFrom, DateTime dateTo)
        {
            if (photographerId <= 0 || dateFrom == null || dateTo == null)
            {
                return null;
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    // List<PHOTOSYS_RunsheetResult> photoSysRunsheetResultObjList = ctx.PHOTOSYS_Runsheet((int?)photographerId, (DateTime?)dateFrom, (DateTime?)dateTo).ToList();
                    List<PHOTOSYS_Runsheet2Result> photoSysRunsheetResultObjList = ctx.PHOTOSYS_Runsheet2((int?)photographerId).ToList();

                    if (photoSysRunsheetResultObjList == null || photoSysRunsheetResultObjList.Count == 0)
                    {
                        return null;
                    }

                    List<PhotoSysRunSheet> photoSysRunSheetListObj = new List<PhotoSysRunSheet>();

                    foreach (PHOTOSYS_Runsheet2Result photoSysRunSheetObjItem in photoSysRunsheetResultObjList)
                    {
                        PhotoSysRunSheet photoSysRunSheetObj = new PhotoSysRunSheet(photoSysRunSheetObjItem);
                        photoSysRunSheetListObj.Add(photoSysRunSheetObj);
                    }

                    return photoSysRunSheetListObj;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'photoSysRunsheetResult List'. photographerId:{0}", photographerId);
                throw;
            }
        }

        /// <summary>
        /// Gets the photo sys run sheet details.
        /// </summary>
        /// <param name="photographerId">The photographer id.</param>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        /// <param name="orderID">The order ID.</param>
        /// <returns>
        /// Returns the PHOTOSYS_RunsheetResult obj.
        /// </returns>
        public PhotoSysRunSheetDetail GetPhotoSysRunSheetDetails(int photographerId, DateTime dateFrom, DateTime dateTo, int orderID)
        {
            if (photographerId <= 0 || dateFrom == null || dateTo == null || orderID == 0)
            {
                return null;
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    PHOTOSYS_RunsheetResult photoSysRunSheetDetail = new PHOTOSYS_RunsheetResult();

                    List<PHOTOSYS_RunsheetResult> photoSysRunsheetResultObjList = ctx.PHOTOSYS_Runsheet((int?)photographerId, (DateTime?)dateFrom, (DateTime?)dateTo).ToList();

                    /// Get single Runsheet Obj by OrderID.
                    if (photoSysRunsheetResultObjList != null && photoSysRunsheetResultObjList.Count >= 1)
                    {
                        photoSysRunSheetDetail = (from rs in photoSysRunsheetResultObjList
                                                  where rs.OrderID == orderID
                                                  select rs).SingleOrDefault();

                        if (photoSysRunSheetDetail == null)
                        {
                            return null;
                        }

                        PhotoSysRunSheetDetail photoSysRunSheetDetailObj = new PhotoSysRunSheetDetail(photoSysRunSheetDetail);

                        return photoSysRunSheetDetailObj;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'photoSysRunsheetResult'. OrderID:{0}", orderID);
                throw;
            }
        }

        /// <summary>
        /// Gets the photographers IMEI.
        /// </summary>
        /// <param name="photographerID">The photographer ID.</param>
        /// <returns>
        /// Rentuns the IMEI number.
        /// </returns>
        //public string GetPhotographerIMEI(int photographerID)
        //{
        //    try
        //    {
        //        if (photographerID <= 0)
        //        {
        //            return null;
        //        }

        //        using (AbcDataContext ctx = new AbcDataContext())
        //        {
        //            string imei = ctx.Photographers.SingleOrDefault(x => x.PgId == photographerID).IMEI;
        //            return imei;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Exception(ex, "Error occured in 'Photographer'. photographerID:{0}", photographerID);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Gets the photographer's ID.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns>
        /// Returns the Photographer ID
        /// </returns>
        public int GetPhotographerID(string imei)
        {
            int photographerID = 0;

            if (imei == null || imei == string.Empty)
            {
                return photographerID;
            }
            using (AbcDataContext ctx = new AbcDataContext())
            {
                photographerID = ctx.Photographers.SingleOrDefault(x => x.IMEI == imei).PgId;
            }

            return photographerID;
        }

        public int GetPhotographerIdByLogin(string IMEI, string username, string password)
        {
            //Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            //Guard.ArgumentNotNullOrEmptyString(username, "username");
            //Guard.ArgumentNotNullOrEmptyString(password, "password");

            if (string.IsNullOrEmpty(IMEI) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return -1;

            using (AbcDataContext ctx = new AbcDataContext())
            {
                var photographer = ctx.Photographers.FirstOrDefault(x => x.UserId == username && x.Password == password);

                return photographer == null ? -1 : photographer.PgId;
            }
        }

        /// <summary>
        /// Gets the first name of the photographer.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns></returns>
        //public string GetPhotographerFirstName(string imei)
        //{
        //    string photographerFirstName = string.Empty;

        //    if (imei == null || imei == string.Empty)
        //    {
        //        return photographerFirstName;
        //    }
        //    using (AbcDataContext ctx = new AbcDataContext())
        //    {
        //        photographerFirstName = ctx.Photographers.SingleOrDefault(x => x.IMEI == imei).FName;
        //    }

        //    return photographerFirstName;
        //}

        /// <summary>
        /// Gets the content of the photographer config.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns></returns>
        public string GetPhotographerConfigContent(string imei)
        {
            string configContent = string.Empty;

            try
            {
                if (imei == null || imei == string.Empty)
                {
                    return configContent;
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    configContent = ctx.Photographers.SingleOrDefault(x => x.IMEI == imei).ConfigContent;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in ConfigContent of the Photographers. IMEI:{0}", imei);
                throw;
            }

            return configContent;
        }

        public string GetPhotographerConfigContentVersion2(string IMEI, int photographerId, string username)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNullOrEmptyString(username, "username");

            string configContent = string.Empty;

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    configContent = ctx.Photographers.SingleOrDefault(x => x.PgId == photographerId).ConfigContent;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in ConfigContent of the Photographers. Photographer ID:{0}", photographerId);
                throw;
            }

            return configContent;
        }

        /// <summary>
        /// Gets the photographer email.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns>
        /// Returns the email Address of the Photographer.
        /// </returns>
        //public string GetPhotographerEmail(string imei)
        //{
        //    string photographerEmail =string.Empty;

        //    if (imei == null || imei == string.Empty)
        //    {
        //        return null;
        //    }

        //    using (AbcDataContext ctx = new AbcDataContext())
        //    {
        //        photographerEmail = ctx.Photographers.SingleOrDefault(x => x.IMEI == imei).Email;
        //    }
        //    return photographerEmail;
        //}

        /// <summary>
        /// Gets the photographer.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns>
        /// Returns the Photographer.
        /// </returns>
        public Photographer GetPhotographer(string imei)
        {
            Photographer photographer = null;

            if (imei == null || imei == string.Empty)
            {
                return photographer;
            }
            using (AbcDataContext ctx = new AbcDataContext())
            {
                photographer = ctx.Photographers.SingleOrDefault(x => x.IMEI == imei);
            }

            return photographer;
        }

        public Photographer GetPhotographerVersion2(string IMEI, int photographerId, string username)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNullOrEmptyString(username, "username");

            Photographer photographer = null;

            using (AbcDataContext ctx = new AbcDataContext())
            {
                photographer = ctx.Photographers.SingleOrDefault(x => x.PgId == photographerId);
            }

            return photographer;
        }

        /// <summary>
        /// Updates the run sheet photo order Table.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="photographerId">The photographer id.</param>
        public void UpdateRunSheetPhotoOrder(int orderId, int photographerId)
        {

            if (orderId <= 0 || photographerId <= 0)
            {
                throw new ArgumentNullException("OrderId or PhotographerId is not valid.");
            }

            using (AbcDataContext ctx = new AbcDataContext())
            {
                PhotoOrder photoOrder = new PhotoOrder();

                photoOrder = (from po in ctx.PhotoOrders
                              where po.OrderID == orderId && po.PgId == photographerId
                              select po).SingleOrDefault();


                if (photoOrder == null)
                {
                    return;
                }

                /// Update the Photo Orders.
                photoOrder.Completed = DateTime.Now;
                ctx.SubmitChanges();
            }
        }

        /// <summary>
        /// Gets the FTP path by imei.
        /// </summary>
        /// <param name="imei"></param>
        /// <returns>
        /// Returns the ftp Folder Path.
        /// </returns>
        public string GetFtpFolderPathByImei(string imei)
        {
            if (imei == null || imei == string.Empty)
            {
                return string.Empty;
            }

            string ftpFolderPath = string.Empty;

            using (AbcDataContext ctx = new AbcDataContext())
            {

                string ftpPath = (from bg in ctx.BusinessRegions
                                  where
                              (from pg in ctx.Photographers
                               where pg.IMEI == imei
                               select pg.BusinessRegionId).Contains(bg.BusinessRegionID)
                                  select bg.FTPFolderPath).SingleOrDefault();

                ftpFolderPath = ftpPath;


            }

            return ftpFolderPath;
        }

        public string GetFtpFolderPathByImeiVersion2(string IMEI, int photographerId, string username)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNullOrEmptyString(username, "username");

            string ftpFolderPath = string.Empty;

            using (AbcDataContext ctx = new AbcDataContext())
            {

                string ftpPath = (from bg in ctx.BusinessRegions
                                  where
                              (from pg in ctx.Photographers
                               where pg.PgId == photographerId
                               select pg.BusinessRegionId).Contains(bg.BusinessRegionID)
                                  select bg.FTPFolderPath).SingleOrDefault();

                ftpFolderPath = ftpPath;


            }

            return ftpFolderPath;
        }

        #endregion

        #region Photography

        public List<PhotoOrder> GetPhotoOrder(string IMEI, DateTime dateFrom, List<EntityRelations> loadOptions)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var pg = (from pgh in ctx.Photographers
                              where pgh.IMEI == IMEI
                              select pgh).FirstOrDefault();

                    if (pg == null)
                    {
                        throw new ApplicationException("Invalid Security Id Number");
                    }

                    var query = from po in ctx.PhotoOrders
                                where po.PgId == pg.PgId && (po.Order.DateReceived >= dateFrom || po.Completed == null)
                                orderby po.OrderID
                                select po;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetPhotoOrder'. IMEI:{0}. dateFrom:{1}", IMEI, dateFrom);
                throw;
            }
        }

        public List<PhotoOrder> GetPhotoOrderVersion2(string IMEI, int photographerId, string username, DateTime dateFrom, List<EntityRelations> loadOptions)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNullOrEmptyString(username, "username");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var pg = (from pgh in ctx.Photographers
                              where pgh.PgId == photographerId && pgh.UserId == username && pgh.IsActive
                              select pgh).FirstOrDefault();

                    if (pg == null)
                    {
                        throw new ApplicationException("Invalid Security Id Number");
                    }

                    var query = from po in ctx.PhotoOrders
                                where po.PgId == pg.PgId && (po.Order.DateReceived >= dateFrom || po.Completed == null)
                                orderby po.OrderID
                                select po;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetPhotoOrder'. Photographer ID:{0}. dateFrom:{1}", photographerId, dateFrom);
                throw;
            }
        }

        public PhotoOrder GetPhotoOrderById(string IMEI, int orderId, List<EntityRelations> loadOptions)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var pg = (from pgh in ctx.Photographers
                              where pgh.IMEI == IMEI
                              select pgh).FirstOrDefault();

                    if (pg == null)
                    {
                        throw new ApplicationException("Invalid Security Id Number");
                    }

                    var query = (from po in ctx.PhotoOrders
                                 where po.PgId == pg.PgId && po.OrderID == orderId
                                 select po).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetPhotoOrderById'. IMEI:{0}. OrderId:{1}", IMEI, orderId);
                throw;
            }
        }

        public PhotoOrder GetPhotoOrderByIdVersion2(string IMEI, int photographerId, string username, int orderId, List<EntityRelations> loadOptions)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNullOrEmptyString(username, "username");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var pg = (from pgh in ctx.Photographers
                              where pgh.PgId == photographerId && pgh.UserId == username
                              select pgh).FirstOrDefault();

                    if (pg == null)
                    {
                        throw new ApplicationException("Invalid photographerId/username");
                    }

                    var query = (from po in ctx.PhotoOrders
                                 where po.PgId == pg.PgId && po.OrderID == orderId
                                 select po).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetPhotoOrderById'. Photographer ID:{0}. OrderId:{1}", photographerId, orderId);
                throw;
            }
        }

        public PhotoOrder UpdatePhotoOrderSubset(string IMEI, PhotoOrder order, List<EntityRelations> loadOptions)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNull(order, "order");
            Guard.IsPositive(order.OrderID, "order.OrderId");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var pg = (from pgh in ctx.Photographers
                              where pgh.IMEI == IMEI
                              select pgh).FirstOrDefault();

                    if (pg == null)
                    {
                        throw new ApplicationException("Invalid Security Id Number");
                    }

                    var query = (from po in ctx.PhotoOrders
                                 where po.PgId == pg.PgId && po.OrderID == order.OrderID
                                 select po).FirstOrDefault();

                    if (query != null)
                    {
                        query.Appointment = order.Appointment;
                        query.AppointmentDuration = order.AppointmentDuration;
                        query.Completed = order.Completed;
                        query.FolderNo = order.FolderNo;
                        query.Notes = order.Notes;
                        query.NightWeekendRequest = order.NightWeekendRequest;
                        ctx.SubmitChanges();

                        var fname = (from ph in ctx.Photographers where ph.PgId == order.PgId select ph.FName).FirstOrDefault();

                        if (order.NotifyAccounts)
                        {
                            ctx.PHOTOSYS_EventGen_NotifyNightWork(order.OrderID, fname, DateTime.Now, "OnlineBL:PhotoSyncDroid");
                        }
                    }

                    return query;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'UpdatePhotoOrderSubset'. IMEI:{0}. OrderId:{1}", IMEI, order.OrderID);
                throw;
            }
        }

        public PhotoOrder UpdatePhotoOrderSubsetVersion2(string IMEI, int photographerId, string username, PhotoOrder order, List<EntityRelations> loadOptions)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNullOrEmptyString(username, "username");

            Guard.ArgumentNotNull(order, "order");
            Guard.IsPositive(order.OrderID, "order.OrderId");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var pg = (from pgh in ctx.Photographers
                              where pgh.PgId == photographerId && pgh.UserId == username
                              select pgh).FirstOrDefault();

                    if (pg == null)
                    {
                        throw new ApplicationException("Invalid photographerId/username");
                    }

                    var query = (from po in ctx.PhotoOrders
                                 where po.PgId == pg.PgId && po.OrderID == order.OrderID
                                 select po).FirstOrDefault();

                    if (query != null)
                    {
                        bool bookingAppointment = false;
                        if (!query.Appointment.HasValue && order.Appointment.HasValue && !order.Completed.HasValue)
                        {
                            bookingAppointment = true;
                        }

                        query.Appointment = order.Appointment;
                        query.AppointmentDuration = order.AppointmentDuration;
                        query.Completed = order.Completed;
                        query.FolderNo = order.FolderNo;
                        query.Notes = order.Notes;
                        query.NightWeekendRequest = order.NightWeekendRequest;
                        ctx.SubmitChanges();

                        var fname = (from ph in ctx.Photographers where ph.PgId == order.PgId select ph.FName).FirstOrDefault();

                        if (order.NotifyAccounts)
                        {
                            ctx.PHOTOSYS_EventGen_NotifyNightWork(order.OrderID, fname, DateTime.Now, "OnlineBL:PhotoSyncDroid");
                        }
                        else
                        {
                            if (order.Completed.HasValue)
                            {
                                //try to read data from source one more time
                                try
                                {
                                    bool hasNightPhotos = query.Order.OrderDetails.Any(x =>
                                    {
                                        return x.Product.Name.IndexOf("night", StringComparison.OrdinalIgnoreCase) >= 0
                                            || x.Product.Name.IndexOf("+n", StringComparison.OrdinalIgnoreCase) >= 0
                                            || x.Product.Name.IndexOf("twilight", StringComparison.OrdinalIgnoreCase) >= 0;
                                    });

                                    if (hasNightPhotos)
                                    {
                                        ctx.PHOTOSYS_EventGen_NotifyNightWork(order.OrderID, fname, DateTime.Now, "PhotoSyncDroid");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, "Error occured in 'UpdatePhotoOrderSubsetVersion2' sending Notify Night Work email. Photographer ID:{0}. OrderId:{1}", photographerId, order.OrderID);
                                }
                            }
                        }

                        try
                        {
                            if (order.Completed.HasValue)
                            {
                                if (query.Order.OrderDetails.Any(x => x.ProductID == 9797))
                                {
                                    ctx.Photo_EventGen_NotifyDroneDone(order.OrderID, fname, DateTime.Now, "PhotoSyncDroid:UpdatePhotoOrder");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, "Error occured in 'UpdatePhotoOrderSubsetVersion2' sending drone email. Photographer ID:{0}. OrderId:{1}", photographerId, order.OrderID);
                        }

                        try
                        {
                            var pod = (from po in ctx.PhotoOrderDetails
                                       where po.OrderID == order.OrderID
                                       select po).FirstOrDefault();

                            if (pod != null && order.Completed.HasValue)
                            {
                                Order od = (from o in ctx.Orders
                                            where o.OrderID == pod.LinkSBOrderID
                                            select o).FirstOrDefault();
                                if (od != null)
                                {

                                    if (od.OnHold.HasValue)
                                    {
                                        od.OnHold = null;
                                        if (!string.IsNullOrEmpty(od.Notes) && od.Notes.Contains("On Hold – awaiting photography to be completed"))
                                        {
                                            od.Notes = od.Notes.Replace("On Hold – awaiting photography to be completed", "");
                                        }
                                    }

                                    //send email notification to sb admin
                                    int eventID = EventSettings.PhotographyCompletedStockboardInventory;
                                    string sub = "Photography Completed - Stockboard Inventory";
                                    string xmlData = string.Empty;

                                    xmlData = @"<EVENT>
								                    <OrderID>" + pod.LinkSBOrderID + @"</OrderID>
                                                    <AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + "/" + od.Client.Office + @"</AgentName>
                                                    <AgentOffice>" + od.Client.Office + @"</AgentOffice>
								                    <PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + @"</PAddress>
                                                    <Email>" + od.SendProofTo + @"</Email>
                                                    <ReceivedOn>" + DateTime.Now.ToString() +  " - Photo Order ID: " + order.OrderID + @"</ReceivedOn>
								                    </EVENT>";

                                    string textData = xmlData;
                                    string source = "UpdatePhotoOrderSubsetVersion2";

                                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, pod.LinkSBOrderID, null, null, null, source, String.Empty);
                                    ctx.SubmitChanges();
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, "Error occured in 'UpdatePhotoOrderSubsetVersion2' sending email. Photographer ID:{0}. OrderId:{1}", photographerId, order.OrderID);
                        }

                        try
                        {
                            if (bookingAppointment)
                            {
                                Order od = (from o in ctx.Orders
                                            where o.OrderID == order.OrderID
                                            select o).FirstOrDefault();
                                if (od != null && od.ClientID == ClientSettings.BuyMyPlaceSouthMelbourne)
                                {
                                    Logger.Warn("App: " + order.Appointment.Value.ToString() + " - OrderID: " + order.OrderID);

                                    ctx.FrontOffice_ServiceQueueAdd("PhotographyAppointment",
                                        String.Format("OrderID={0}", order.OrderID),
                                        0,
                                        1,
                                        od.SendProofTo,
                                        null,
                                        string.Empty,
                                        string.Empty,
                                        "PhotoSys_UpdatePhotoOrder",
                                        1);

                                    ctx.SubmitChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, "Error occured in 'UpdatePhotoOrderSubsetVersion2' sending bmp email. Photographer ID:{0}. OrderId:{1}", photographerId, order.OrderID);
                        }
                        
                    }

                    return query;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'UpdatePhotoOrderSubset'. Photographer ID:{0}. OrderId:{1}", photographerId, order.OrderID);
                throw;
            }
        }

        public List<PhotoSysOrderProduct> GetProductsByOrderID(string IMEI, int orderId)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (orderId > 0)
                    {
                        DataLoadOptions dl = new DataLoadOptions();

                        dl.LoadWith<Order>(e => e.OrderDetails);
                        dl.LoadWith<OrderDetail>(e => e.Product);
                        ctx.LoadOptions = dl;
                        var resultOders = (from passedOrder in ctx.Orders
                                           where passedOrder.OrderID == orderId
                                           select passedOrder).ToList();

                        List<PhotoSysOrderProduct> productInfo = new List<PhotoSysOrderProduct>();
                        foreach (Order item in resultOders)
                        {
                            foreach (var details in item.OrderDetails)
                            {
                                var info = new PhotoSysOrderProduct();
                                info.OrderID = orderId;
                                info.ProductID = details.ProductID;
                                info.ProductName = details.Product.Name;
                                productInfo.Add(info);

                            }

                        }
                        return productInfo;
                    }
                    return null;
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "An error Occured in GetProductsByOrderID()");
                throw;
            }

        }

        public List<PhotoSysOrderProduct> GetProductsByOrderIDVersion2(string IMEI, int photographerId, string username, int orderId)
        {
            Guard.ArgumentNotNullOrEmptyString(IMEI, "IMEI");
            Guard.ArgumentNotNullOrEmptyString(username, "username");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (orderId > 0)
                    {
                        DataLoadOptions dl = new DataLoadOptions();

                        dl.LoadWith<Order>(e => e.OrderDetails);
                        dl.LoadWith<OrderDetail>(e => e.Product);
                        ctx.LoadOptions = dl;
                        var resultOders = (from passedOrder in ctx.Orders
                                           where passedOrder.OrderID == orderId
                                           select passedOrder).ToList();

                        List<PhotoSysOrderProduct> productInfo = new List<PhotoSysOrderProduct>();
                        foreach (Order item in resultOders)
                        {
                            foreach (var details in item.OrderDetails)
                            {
                                var info = new PhotoSysOrderProduct();
                                info.OrderID = orderId;
                                info.ProductID = details.ProductID;
                                info.ProductName = details.Product.Name;
                                productInfo.Add(info);

                            }

                        }
                        return productInfo;
                    }
                    return null;
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "An error Occured in GetProductsByOrderID()");
                throw;
            }

        }

        public PhotographerRunsheetDTO GetRunsheet(int photographerId)
        {
            var loaded = 0;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(new List<EntityRelations>()
                    {
                        EntityRelations.PhotoOrder_To_Order,
                        EntityRelations.Order_To_Client,
                        EntityRelations.Order_To_Location,
                        EntityRelations.Order_To_OrderDetails, EntityRelations.OrderDetail_To_Product,
                        EntityRelations.Order_To_FloorPlanOrders,
                    });

                    var photoOrders = ctx.PhotoOrders
                        .Where(x => x.PgId == photographerId)
                        .Where(x => !x.Completed.HasValue || x.Completed > DateTime.Now.AddDays(-7))
                        .OrderBy(x => x.OrderID)
                        .ToList();

                    if (photoOrders != null)
                        loaded = photoOrders.Count;

                    var dto = new PhotographerRunsheetDTO
                    {
                        UserId = photographerId,
                        Orders = photoOrders.Select(x => new PhotoOrderDTO(x)).ToList(),
                        OrderDetails = photoOrders.SelectMany(x => x.Order.OrderDetails).Select(x => new PhotoOrderDetailDTO(x)).ToList(),
                        DroneFlightRequests = new List<DroneFlightRequestDTO>(),
                        DronePilots = new List<DronePilotDTO>()
                    };

                    if (IsChiefDronePilot(photographerId))
                    {
                        dto.DroneFlightRequests = ctx.DroneAuthorisations.Select(x => new DroneFlightRequestDTO(x)).ToList();
                        dto.DronePilots = ctx.Photographers.Where(x => x.IsActive).Select(x => new DronePilotDTO(x)).ToList();
                    }

                    return dto;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetRunsheet(photographerId:{0}), photoOrders.Count: {1} ", photographerId, loaded);
                throw;
            }
        }

        #endregion

        #region Drones

        public List<DroneAuthorisation> GetDroneAuthorisationRequests(int photographerId, string username)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (!IsChiefDronePilot(photographerId))
                        throw new InvalidOperationException("User is not a chief drone pilot.");

                    var oneMonthAgo = DateTime.Now - TimeSpan.FromDays(30);

                    var droneAuthorisations = ctx.DroneAuthorisations
                        .Where(x => x.FlightScheduledOn >= oneMonthAgo || x.FlightScheduledOn == null || x.Status == 0)
                        .ToList();

                    return droneAuthorisations;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in '{0}'. photographerId:{1}", MethodInfo.GetCurrentMethod().Name, photographerId);
                throw;
            }
        }

        public void SaveDroneAuthorisationRequests(int photographerId, string username, DroneAuthorisation droneAuthorisation)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    if (!IsChiefDronePilot(photographerId))
                        throw new InvalidOperationException("User is not a chief drone pilot.");


                    var existingDroneAuthorisations = ctx.DroneAuthorisations.FirstOrDefault(x => x.DroneAuthorisationID == droneAuthorisation.DroneAuthorisationID);

                    if (existingDroneAuthorisations == null)
                        throw new InvalidOperationException("DroneAuthorisationID not found.");

                    // Update
                    existingDroneAuthorisations.Status = droneAuthorisation.Status;
                    existingDroneAuthorisations.FlightScheduledOn = droneAuthorisation.FlightScheduledOn;
                    existingDroneAuthorisations.FlightAddress = droneAuthorisation.FlightAddress;
                    existingDroneAuthorisations.RPASSystem = droneAuthorisation.RPASSystem;
                    existingDroneAuthorisations.RemotePilot = droneAuthorisation.RemotePilot;
                    existingDroneAuthorisations.ChiefRemotePilot = droneAuthorisation.ChiefRemotePilot;
                    existingDroneAuthorisations.Description = droneAuthorisation.Description;
                    existingDroneAuthorisations.Observer = droneAuthorisation.Observer;
                    existingDroneAuthorisations.LocalFrequencies = droneAuthorisation.LocalFrequencies;
                    existingDroneAuthorisations.EmergencyContact = droneAuthorisation.EmergencyContact;
                    existingDroneAuthorisations.Notes = droneAuthorisation.Notes;
                    existingDroneAuthorisations.LastUpdatedOn = droneAuthorisation.LastUpdatedOn;
                    existingDroneAuthorisations.LastUpdatedBy = droneAuthorisation.LastUpdatedBy;

                    ctx.SubmitChanges();

                    GenerateDronePhotographyStatusChangeEvent(droneAuthorisation.OrderID);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in '{0}'. photographerId:{1}", MethodInfo.GetCurrentMethod().Name, photographerId);
                throw;
            }
        }

        public List<PhotographerName> GetPhotographers(int photographerId, string username)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    if (!IsChiefDronePilot(photographerId))
                        throw new InvalidOperationException("User is not a chief drone pilot.");

                    var photographers = ctx.Photographers
                        .Where(x => x.IsActive)
                        .Select(x => new PhotographerName() { PhotographerID = x.PgId, Name = x.FName + ' ' + x.LName })
                       .ToList();

                    return photographers;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in '{0}'. photographerId:{1}", MethodInfo.GetCurrentMethod().Name, photographerId);
                throw;
            }
        }

        public PhotographerLogin GetPhotographerLogin(string IMEI, string username, string password)
        {
            if (string.IsNullOrEmpty(IMEI)) return PhotographerLogin.MissingIMEI;
            if (string.IsNullOrEmpty(username)) return PhotographerLogin.MissingUsername;
            if (string.IsNullOrEmpty(password)) return PhotographerLogin.MissingPassword;

            using (AbcDataContext ctx = new AbcDataContext())
            {
                var photographer = ctx.Photographers                   
                    .FirstOrDefault(x => x.UserId == username && x.Password == password);

                if (photographer == null)
                    return PhotographerLogin.InvalidUsernamePassword;

                var isChiefDronePilot = IsChiefDronePilot(photographer.PgId);
                var photographerName = photographer.FName + ' ' + photographer.LName;
                return new PhotographerLogin(photographer.PgId, photographer.UserId, photographerName, photographer.IsActive, isChiefDronePilot);
                
            }
        }

        private bool IsChiefDronePilot(int photographerId)
        {
            if (photographerId == 195 || photographerId == 214 || photographerId == 216 || photographerId == 208 || photographerId == 165)
                return true;

            return false;
        }

        #endregion

        #region Floor Plans

        [Obsolete("Floorplans are supplied through GetRunsheet() now.")]
        public List<FloorPlan> GetFloorPlans(int photographerId)
        {
            using (AbcDataContext ctx = new AbcDataContext())
            {
                var query = ctx.ViewOutstandingFloorPlans
                    .Where(x => x.PgId == photographerId)
                    .ToList();

                // You need to do this otherwise linq to sql will recompose the query and treat NewGuid as a fixed value. Wow.
                var floorPlans = query.Select(x => new FloorPlan
                {
                    Guid = Guid.NewGuid(),
                    PhotographerId = x.PgId,
                    FloorPlanId = x.FloorPlanID,
                    OrderId = x.OrderID,
                    Address = x.PropertyAddress,
                    Location = x.Location,
                    State = x.State,
                    PostCode = x.PostCode,
                    ProductId = x.ProductID,
                    ProductName = x.Name,
                    UpdatedOn = x.DatePhotoUploadedFromPhotographer,
                    Notes = string.Empty,
                    Urgent = false
                }).ToList();

                return floorPlans;
            }
        }

        [Obsolete("Use 'AddFloorPlanImage' and 'SetFloorPlanNote'")]
        public void SaveFloorPlan(FloorPlan floorPlan)
        {
            if (floorPlan == null) throw new ArgumentNullException("floorPlan");

            try
            {
                var photographer = GetPhotographerById(floorPlan.PhotographerId);
                if (photographer == null) throw new ApplicationException("Invalid photographerId.");

                var order = GetOrderById(floorPlan.OrderId, EntityRelations.Order_To_OrderDetails, EntityRelations.OrderDetail_To_Product,
                    EntityRelations.Order_To_FloorPlanOrders, EntityRelations.Order_To_Client, EntityRelations.Order_To_Location);
                if (order == null) throw new ApplicationException("No order with this ID exists.");

                //if (order.FloorPlanOrders.Any()) throw new ApplicationException("A floor plan has already been submitted for this order.");

                var product = order.OrderDetails.FirstOrDefault(x => x.ProductID == floorPlan.ProductId);
                if (product == null) throw new ApplicationException("Order does not contain this product.");

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    FloorPlanOrder floorPlanOrder = null;

                    if (floorPlan.FloorPlanId.HasValue)
                        floorPlanOrder = ctx.FloorPlanOrders.First(x => x.FloorPlanID == floorPlan.FloorPlanId);

                    if (floorPlanOrder == null)
                    {
                        floorPlanOrder = new FloorPlanOrder();
                        ctx.FloorPlanOrders.InsertOnSubmit(floorPlanOrder);
                    }

                    floorPlanOrder.OrderID = floorPlan.OrderId;
                    floorPlanOrder.ProductID = floorPlan.ProductId;
                    floorPlanOrder.Notes = floorPlan.Notes;
                    floorPlanOrder.DateFloorPlanUploadedFromPhotographer = DateTime.Now;
                    floorPlanOrder.FloorPlanDraftImageFile = floorPlan.Images;

                    ctx.SubmitChanges();
                }

                ROnlineBLeFloorPlanDraftEvent(order, photographer, floorPlan.Notes, floorPlan.Images);
            }
            catch (Exception ex)
            {
                var message = "Error occured in 'SubmitFloorPlan'. Photographer ID: {0}. Order ID: {1}. FloorPlanImagePath: {2}.";
                Logger.Exception(ex, message, floorPlan.PhotographerId, floorPlan.OrderId, floorPlan.Images);
                throw;
            }
        }

        public void AddFloorPlanImage(int photographerId, int orderId, string filePath)
        {
            try
            {
                var photographer = GetPhotographerById(photographerId);
                if (photographer == null) throw new ApplicationException("Invalid photographerId.");

                var order = GetOrderById(orderId,
                     EntityRelations.Order_To_OrderDetails, EntityRelations.OrderDetail_To_Product,
                     EntityRelations.Order_To_FloorPlanOrders,
                     EntityRelations.Order_To_Client,
                     EntityRelations.Order_To_Location);
                if (order == null) throw new ApplicationException("No order with this ID exists.");

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var products = order.OrderDetails.Select(x => x.Product).Where(x => x.TypeID == ProductTypes.FloorPlans);
                    if (products == null || !products.Any()) throw new ApplicationException("Order does not contain any floorplans.");

                    foreach (var product in products)
                    {
                        var floorplan = order.FloorPlanOrders.SingleOrDefault(x => x.ProductID == product.ProductID);

                        if (floorplan == null)
                            floorplan = new FloorPlanOrder
                            {
                                FloorPlanID = -1,
                                OrderID = order.OrderID,
                                ProductID = product.ProductID,
                                Notes = string.Empty,
                                FloorPlanDraftImageFile = string.Empty
                            };

                        floorplan.DateFloorPlanUploadedFromPhotographer = floorplan.DateFloorPlanUploadedFromPhotographer ?? DateTime.Now;
                        floorplan.FloorPlanDraftImageFile = string.IsNullOrEmpty(floorplan.FloorPlanDraftImageFile)
                                ? filePath : floorplan.FloorPlanDraftImageFile + ';' + filePath;

                        if (floorplan.FloorPlanID == -1)
                            ctx.FloorPlanOrders.InsertOnSubmit(floorplan);
                        else
                            ctx.FloorPlanOrders.Attach(floorplan, true);
                    }

                    ctx.SubmitChanges();
                }

                ROnlineBLeFloorPlanDraftEvent(order, photographer, string.Empty, filePath);
            }
            catch (Exception ex)
            {
                var message = "Error occured in 'AddFloorPlanImage'. photographerId: {0}. orderId: {1}. filePath: {2}.";
                Logger.Exception(ex, message, photographerId, orderId, filePath);
                throw;
            }
        }

        public void SetFloorPlanNote(int photographerId, int orderId, string notes)
        {
            try
            {
                var photographer = GetPhotographerById(photographerId);
                if (photographer == null) throw new ApplicationException("Invalid photographerId.");

                var order = GetOrderById(orderId,
                    EntityRelations.Order_To_OrderDetails, EntityRelations.OrderDetail_To_Product,
                    EntityRelations.Order_To_FloorPlanOrders,
                    EntityRelations.Order_To_Client,
                    EntityRelations.Order_To_Location);
                if (order == null) throw new ApplicationException("No order with this ID exists.");

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var products = order.OrderDetails.Select(x => x.Product).Where(x => x.TypeID == ProductTypes.FloorPlans);
                    if (products == null || !products.Any()) throw new ApplicationException("Order does not contain any floorplans.");

                    foreach (var product in products)
                    {
                        var floorplan = order.FloorPlanOrders.SingleOrDefault(x => x.ProductID == product.ProductID);

                        if (floorplan == null)
                            floorplan = new FloorPlanOrder
                            {
                                FloorPlanID = -1,
                                OrderID = order.OrderID,
                                ProductID = product.ProductID,
                                Notes = string.Empty,
                                FloorPlanDraftImageFile = string.Empty
                            };

                        floorplan.Notes = notes;

                        if (floorplan.FloorPlanID == -1)
                            ctx.FloorPlanOrders.InsertOnSubmit(floorplan);
                        else
                            ctx.FloorPlanOrders.Attach(floorplan, true);
                    }

                    ctx.SubmitChanges();
                }

                ROnlineBLeFloorPlanDraftEvent(order, photographer, notes, string.Empty);
            }
            catch (Exception ex)
            {
                var message = "Error occured in 'SetFloorPlanNote'. photographerId: {0}. orderId: {1}. notes: {2}.";
                Logger.Exception(ex, message, photographerId, orderId, notes);
                throw;
            }
        }

        private void ROnlineBLeFloorPlanDraftEvent(Order order, Photographer photographer, string notes, string imagePath)
        {
            const int eventID = EventSettings.FloorplanDraftImage;

            string subject = string.Format("Floorplan Draft Image - {0}", order.OrderID);
            string textData = string.Format("Floorplan Draft Image has been uploaded for order {0}", order.OrderID);
            string source = "PhotoSysRunSheetService.SubmitFloorPlan(...)";
            string xmlEventData =
                "<EVENT>" +
                string.Format("<OrderID>{0}</OrderID>", order.OrderID) +
                string.Format("<AgentName>{0}</AgentName>", XMLEncode(order.Client.ClientName)) +
                string.Format("<AgentOffice>{0}</AgentOffice>", XMLEncode(order.Client.Office)) +
                string.Format("<PAddress>{0}, {1}</PAddress>", XMLEncode(order.PropertyAddress), XMLEncode(order.Location.Location1)) +
                string.Format("<Email>{0}</Email>", XMLEncode(photographer.Email)) +
                string.Format("<PhotographerName>{0} {1}</PhotographerName>", XMLEncode(photographer.FName), XMLEncode(photographer.LName)) +
                string.Format("<ContactNumber>{0}</ContactNumber>", XMLEncode(photographer.Mobile)) +
                string.Format("<RequestDetails>{0}</RequestDetails>", "Floorplan") +
                string.Format("<Notes>{0}</Notes>", XMLEncode(notes)) +
                "</EVENT>";

            using (AbcDataContext ctx = new AbcDataContext())
                ctx.SP_EventQueueAdd(eventID, subject, xmlEventData, textData, order.OrderID, null, null, null, source, imagePath);
        }

        #endregion

        #region Infrastructure

        [DebuggerStepThrough]
        private string XMLEncode(string xmlText)
        {
            return xmlText
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        [DebuggerStepThrough]
        private Order GetOrderById(int orderId, params EntityRelations[] loadOptions)
        {
            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(new List<EntityRelations>(loadOptions));
                return ctx.Orders.SingleOrDefault(o => o.OrderID == orderId);
            }
        }

        [DebuggerStepThrough]
        private Photographer GetPhotographerById(int photographerId, params EntityRelations[] loadOptions)
        {
            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(new List<EntityRelations>(loadOptions));
                return ctx.Photographers.SingleOrDefault(x => x.PgId == photographerId);
            }
        }

        public void GenerateDronePhotographyStatusChangeEvent(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.DroneAuthorisation_To_Photographer1);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string orderEmail = od.SendProofTo;

                    if (string.IsNullOrEmpty(orderEmail))
                    {
                        orderEmail = od.Client.Email;
                    }

                    DroneAuthorisation da = ctx.DroneAuthorisations.SingleOrDefault(o => o.OrderID == orderID);
                    string changedBy = da.Photographer1.FName + " " + da.Photographer1.LName;
                    string statusChangedTo = string.Empty;

                    if (da.Status == 1)
                    {
                        statusChangedTo = "Approval";
                    }
                    else if (da.Status == 2)
                    {
                        statusChangedTo = "Pending";
                    }
                    else
                    {
                        statusChangedTo = "Can not do";
                    }

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.DronePhotographyStatusChange;
                    string sub = "Drone Photography Status Change";
                    string xmlData = @"
									<EVENT>
									<OrderID>" + orderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                    <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
									<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
									<StatusChangedBy>" + changedBy + @"</StatusChangedBy>
									<StatusChangedTo>" + statusChangedTo + @"</StatusChangedTo>
									</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Drone Photography Status Change." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Status Changed By: " + changedBy +
                                            Environment.NewLine +
                                            @"Status Changed To: " + statusChangedTo;

                    string source = "OnlineBL_PhotoSysRunSheetService_GenerateDronePhotographyStatusChangeEvent";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, null, null, null, source, String.Empty);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GenerateDronePhotographyStatusChangeEvent'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion
    }
}

