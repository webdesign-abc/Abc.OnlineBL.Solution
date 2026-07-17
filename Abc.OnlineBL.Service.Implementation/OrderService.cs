using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Xml;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Service.Implementation.BusinessLogic;
using Abc.OnlineBL.Utility.Security;
using Abc.OnlinePublication.Common.Utilities;
using Dom = Abc.OnlinePublication.Common.DOM;
using System.Data.SqlClient;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Entities.Model.WebClient;
using Abc.OnlineBL.VirtualFileSystem;
using Abc.OnlineBL.Entities.Model.OrderService;

namespace Abc.OnlineBL.Service.Implementation
{
    public partial class OrderService : IOrderService
    {
        private const string ITEM_NOTE = "DONT PRINT STOCKBOARD - USE FROM STOCK.";

        #region SayHello
        public string SayHello(string name)
        {
            string sName = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
            //Thread.Sleep(2 * 1000);
            return string.Format("Hello {0}. You are also {1}...", name, sName);
        }
        #endregion

        #region GetOrderById
        public Order GetOrderById(int orderId, List<EntityRelations> loadOptions)
        {
            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                Order order = ctx.Orders.SingleOrDefault(o => o.OrderID == orderId);

                return order;
            }
        }
        #endregion

        #region GetOrderDetailById
        public OrderDetail GetOrderDetailById(int orderDetailId, List<EntityRelations> loadOptions)
        {
            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                OrderDetail od = ctx.OrderDetails.SingleOrDefault(o => o.OrderDetailsID == orderDetailId);
                return od;
            }
        }
        #endregion

        #region UploadPhoto
        public List<UploadPhotoResponse> UploadPhoto(List<UploadPhotoRequest> requests, OrderTrackingEventParameter orderTracking)
        {
            IFile File = VirtualFileSystemFactory.GetFile();
            List<string> outFiles = new List<string>();
            List<UploadPhotoResponse> uploadPhotoResponses = new List<UploadPhotoResponse>();
            int intFileCount = 1;
            string finalInstallLoc = string.Empty;
            #region Sanity Check
            int clientId;
            GetOrderImageRequirementsResult imageRequirements = null;

            if (orderTracking == null)
            {
                throw new ArgumentNullException("OrderTrackingEventParameter");
            }
            if (orderTracking.OrderId <= 0)
            {
                throw new ArgumentOutOfRangeException("OrderId");
            }

            using (AbcDataContext ctx = new AbcDataContext())
            {
                Order od = (from o in ctx.Orders
                            where o.OrderID == orderTracking.OrderId
                            select o).FirstOrDefault();
                if (od == null)
                {
                    orderTracking.OrderId = 0;
                    throw new System.Exception("Job number not found in server");
                }
                clientId = od.ClientID;
                imageRequirements = ctx.GetOrderImageRequirements(orderTracking.OrderId).SingleOrDefault();
            }

            #endregion

            //Mark order to short state Image Extracting
            orderTracking.Message = "Extracting Images";
            UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.ImageExtracting, false);

            foreach (UploadPhotoRequest req in requests)
            {

                #region Sanity Check
                if (string.IsNullOrEmpty(req.UncFilePath))
                {
                    throw new ArgumentNullException("uncFilePath");
                }
                //if (!req.UncFilePath.StartsWith("\\\\"))
                //{
                //    throw new ArgumentException("uncFilePath doesn't start with \\\\");
                //}
                if (!File.Exists(req.UncFilePath))
                {
                    throw new ApplicationException("uncFilePath doesn't exist");
                }
                if (req.FileSelectMode == UploadedFileType.AgentPhoto && String.IsNullOrEmpty(req.AgentContactName))
                {
                    throw new ArgumentNullException("contactAgentName");
                }
                #endregion

                string outPath = OnlineBLConfig.RED_PROPERTY_PHOTO_OUTPUT_FOLDER;
                string outFile = string.Format("{0}_{1}", orderTracking.OrderId, req.FileName);
                UploadPhotoResponse uploadPhotoResponse = new UploadPhotoResponse();
                uploadPhotoResponse.FileSelectMode = req.FileSelectMode;
                uploadPhotoResponse.Quality = UploadedFileQualityType.RED;

                try
                {
                    if (req.FileSelectMode == UploadedFileType.ImagingPhoto)
                    {
                        outPath = OnlineBLConfig.IMAGING_PROPERTY_PHOTO_OUTPUT_FOLDER;

                        outFile = string.Format("{0}_{1}", orderTracking.OrderId, req.FileName);
                        uploadPhotoResponse.Quality = UploadedFileQualityType.IMAGING;
                    }
                    else if (req.FileSelectMode == UploadedFileType.InstallationInstructions)
                    {
                        outPath = OnlineBLConfig.INSTALLATION_FILE_PATH;

                        outFile = string.Format("{0}_{1}", orderTracking.OrderId, Guid.NewGuid().ToString("N") + Path.GetExtension(req.FileName));
                        uploadPhotoResponse.Quality = UploadedFileQualityType.INSTALLATION_INSTRUCTIONS;
                        if (intFileCount == 1)
                            finalInstallLoc = Path.Combine(outPath, outFile);
                        else
                            finalInstallLoc = finalInstallLoc + ";" + Path.Combine(outPath, outFile);
                        intFileCount++;

                    }
                    else if (req.FileSelectMode == UploadedFileType.GraphicsPhoto)
                    {
                        outPath = OnlineBLConfig.GRAPHICS_PROPERTY_PHOTO_OUTPUT_FOLDER;

                        outFile = string.Format("{0}_{1}", orderTracking.OrderId, req.FileName);
                        uploadPhotoResponse.Quality = UploadedFileQualityType.GRAPHICS;
                    }
                    else if (req.FileSelectMode == UploadedFileType.AgentPhoto)
                    {
                        outFile = string.Format("{0}_AGENTPHOTO_{1}_{2}{3}", orderTracking.OrderId, req.AgentContactName.Replace("'", ""), clientId, Path.GetExtension(req.UncFilePath).ToLower());
                        uploadPhotoResponse.Quality = UploadedFileQualityType.IMAGING;
                    }
                    else
                    {

                        if (imageRequirements != null && imageRequirements.ImageRequirements_MinimumMegaPixels > 0)
                        {
                            string ext = Path.GetExtension(req.UncFilePath).ToLower();
                            if (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp" || ext == ".tiff")
                            {
                                try
                                {
                                    //build up image quality and decide right folder image should go to
                                    ImageQuality quality = ImageQuality.GetImageQuality(req.UncFilePath);
                                    decimal inMegaPixel = quality.GetInMegaPixel();

                                    if (inMegaPixel >= (decimal)imageRequirements.ImageRequirements_RecommendedMegaPixels)
                                    {
                                        uploadPhotoResponse.Quality = UploadedFileQualityType.GREEN;
                                        outPath = OnlineBLConfig.GREEN_PROPERTY_PHOTO_OUTPUT_FOLDER;

                                    }
                                    else if (inMegaPixel >= (decimal)imageRequirements.ImageRequirements_MinimumMegaPixels)
                                    {
                                        uploadPhotoResponse.Quality = UploadedFileQualityType.GRAY;
                                        outPath = OnlineBLConfig.GRAY_PROPERTY_PHOTO_OUTPUT_FOLDER;
                                    }

                                    outFile = string.Format("{0}_{1}", orderTracking.OrderId, req.FileName);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, "Could not read the image " + req.UncFilePath);
                                }
                                
                            }
                        }
                    }

                    outFile = Path.Combine(outPath, outFile);
                    outFiles.Add(outFile);

                    //Put image into right folder
                    if (File.Exists(outFile))
                    {
                        //File.SetAttributes(outFile, FileAttributes.Normal);
                        File.Delete(outFile);
                    }
                    File.Copy(req.UncFilePath, outFile, true);

                    //Delete the temp file
                    if (req.UncFilePath != null && File.Exists(req.UncFilePath))
                    {
                        //File.SetAttributes(req.UncFilePath, FileAttributes.Normal);
                        File.Delete(req.UncFilePath);
                    }



                    //return the value back to the client
                    uploadPhotoResponse.FileName = outFile;
                    uploadPhotoResponse.FileProcesed = true;
                    uploadPhotoResponses.Add(uploadPhotoResponse);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "GradeImageQuality");
                    uploadPhotoResponse.FileProcesed = false;
                    uploadPhotoResponse.ErrorMessage = String.Format("System Error - {0}", ex.Message);
                    uploadPhotoResponses.Add(uploadPhotoResponse);
                }
            }

            if (!string.IsNullOrEmpty(finalInstallLoc))
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.Order_To_OrderOtherDetail);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderTracking.OrderId);

                    if (od != null)
                    {
                        if (string.IsNullOrEmpty(od.ErectionNotes))
                            od.ErectionNotes = DateTime.Now.ToString() + " Installation File has been uploaded";
                        else
                            od.ErectionNotes = od.ErectionNotes + " -- " + DateTime.Now.ToString() + " Installation File has been uploaded";

                        if (od.OrderOtherDetail != null)
                        {
                            if (string.IsNullOrEmpty(od.OrderOtherDetail.InstallFile))
                                od.OrderOtherDetail.InstallFile = finalInstallLoc;
                            else
                                od.OrderOtherDetail.InstallFile = od.OrderOtherDetail.InstallFile + ";" + finalInstallLoc;
                        }
                        else
                        {
                            od.OrderOtherDetail = new OrderOtherDetail();
                            od.OrderOtherDetail.OrderId = orderTracking.OrderId;
                            od.OrderOtherDetail.InstallFile = finalInstallLoc;
                        }
                        ctx.SubmitChanges();
                    }

                }
            }

            //If all success
            if (!uploadPhotoResponses.Exists(u => u.FileProcesed == false))
            {
                StringBuilder sb = new StringBuilder();
                string colorCode = string.Empty;
                if (uploadPhotoResponses.Where(u => u.Quality == UploadedFileQualityType.GRAPHICS).Any())
                {
                    orderTracking.Message = "Waiting for Graphic";
                    UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.WaitingForGraphic, false);
                }
                else if (uploadPhotoResponses.Exists(u => u.Quality == UploadedFileQualityType.RED))
                {
                    orderTracking.Message = "Waiting for Image to be processed";
                    UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.WaitingForImageToBeProcessed, false);
                    sb.AppendFormat("<h1>{0}</h1><br>", ServiceConfig.RED_PROPERTY_PHOTO_OUTPUT_MESSAGE);
                    colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.RED);

                }
                else if (uploadPhotoResponses.Exists(u => (u.Quality == UploadedFileQualityType.IMAGING
                                                                     || u.Quality == UploadedFileQualityType.GRAY)))
                {
                    orderTracking.Message = "Waiting for Image to be processed";
                    UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.WaitingForImageToBeProcessed, false);
                    sb.AppendFormat("<h1>{0}</h1><br>", ServiceConfig.GRAY_PROPERTY_PHOTO_OUTPUT_MESSAGE);
                    colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.GRAY);

                }
                else
                {
                    orderTracking.Message = "Image Processed";
                    UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.ImageProcessed, false);
                    sb.AppendFormat("<h1>{0}</h1><br>", ServiceConfig.GREEN_PROPERTY_PHOTO_OUTPUT_MESSAGE);
                    colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.GREEN);

                }

                //May not need as Web site now use different logic
                #region Old Code
                if (orderTracking.LoggedBy.StartsWith("Abc.Web.Client:"))
                {
                    if (outFiles.Count > 0 && !string.IsNullOrEmpty(outFiles[0]))
                    {
                        sb.Append("<TABLE style=\"font-family:Tahoma;font-size:9pt;border:1px solid gray;margin:5px;\">");
                        sb.AppendFormat("<TR><TD width=\"20%\"><B>File Name</B> :</TD><TD width=\"80%\"><A href=\"file:{0}\"><font color=\"{2}\">{1}</font></A> - Quality: <font color=\"{2}\">{2}</font></TD></TR>", outFiles[0].Replace("\\", "/"), outFiles[0], colorCode);
                        sb.Append("</TD></TR></TABLE>");
                    }

                    //Take this out when roll out upload photos
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        List<EntityRelations> options = new List<EntityRelations>();
                        options.Add(EntityRelations.Order_To_Client);
                        options.Add(EntityRelations.Order_To_Location);

                        Order od = (from o in ctx.Orders
                                    where o.OrderID == orderTracking.OrderId
                                    select o).FirstOrDefault();
                        if (od != null)
                        {
                            //ROnlineBLe an Event to send email notification to Admin
                            int eventID = EventSettings.FileUpload;
                            string sub = "File Uploaded for Order " + orderTracking.OrderId;
                            string xmlEventData = @"<HTML><head></head><body>
												<p>
													An Agent has uploaded File in our system.
												</p>
												<p>
													<b><u>Details:</u></b>
												</p>
												<p>
													Agent Name: " + od.Client.ClientName + "/" + od.Client.Office + @"<br />
													Job No    : " + orderTracking.OrderId + @"<br />
												</p>
												<p>
													" + sb.ToString() + @"</p>
												</body>
												</html>";

                            string textData = xmlEventData;
                            string source = "OnlineBL_OrderService_UploadPhoto";

                            ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, orderTracking.OrderId, od.ClientID, null, null, source, "");
                        }
                        ctx.SubmitChanges();
                    }
                }
                #endregion
            }

            return uploadPhotoResponses;
        }

        #endregion

        #region GetArtistNameByOrderID
        public string GetArtistNameByOrderID(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var query = from a in ctx.Artists
                                join pd in ctx.ProofDetails on a.ArtistID equals pd.ArtistID
                                join o in ctx.Orders on pd.OrderID equals o.OrderID
                                where o.OrderID == orderID
                                select a.Name;

                    return query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetArtistNameByOrderID'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsDesignNowApplicable
        public bool IsDesignNowApplicable(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var query = from o in ctx.Orders
                                join pd in ctx.ProofDetails on o.OrderID equals pd.OrderID
                                join od in ctx.OrderDetails on o.OrderID equals od.OrderID
                                where o.OrderID == orderID && pd.DateApproved == null
                                && od.UserDesignOnline != null && od.UserDesignOnline.Value == true
                                select o;


                    return (query.FirstOrDefault() != null);

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsDesignNowApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsAOPDesignInComplete
        public bool IsAOPDesignInComplete(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var query = from aop in ctx.AOP_JobDocuments
                                join o in ctx.Orders on aop.JobId equals o.OrderID
                                where o.OrderID == orderID
                                && aop.StatusId == 0
                                select aop;

                    return (query.FirstOrDefault() != null);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsAOPDesignInComplete'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsApproveJobApplicable
        public bool IsApproveJobApplicable(int orderID)
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
                    //loadOptions.Add(EntityRelations.Order_To_AOP_JobDocuments);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);
                    loadOptions.Add(EntityRelations.ProofDetail_To_Proofs);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (od.OrderDetails.Any(odt => odt.UserDesignOnline.HasValue && odt.UserDesignOnline.Value == true))
                    {
                        if (!od.ProofDetail.DateApproved.HasValue &&
                            !od.OnHold.HasValue)
                        {
                            if (od.OrderDetails.Count == 1 && od.OrderDetails.Any(odt => (odt.Product.TypeID == ProductTypes.Stockboard)) && od.ProofDetail.Proofs.Count <= 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        return false;

                    }
                    else
                    {
                        if (!od.ProofDetail.DateApproved.HasValue &&
                            !od.OnHold.HasValue)
                        {
                            if (od.OrderDetails.Count == 1 && od.OrderDetails.Any(odt => (odt.Product.TypeID == ProductTypes.Stockboard)) && od.ProofDetail.Proofs.Count <= 0)
                            {
                                return false;
                            }
                            else if (od.OrderDetails.Any(odt => (odt.Product.TypeID == ProductTypes.BillBoard)) && od.ProofDetail.Proofs.Count <= 0)
                            {
                                return false;
                            }
                            else if (od.OrderDetails.Any(odt => (odt.Product.TypeID == ProductTypes.Overlay)) && od.ProofDetail.Proofs.Count <= 0)
                            {
                                return false;
                            }
                            else
                            {
                                return SBApprove(od.OrderDetails.ToList());
                            }
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsApproveJobApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        private bool SBApprove(List<OrderDetail> ods)
        {
            List<OrderDetail> sbList = new List<OrderDetail>();
            List<OrderDetail> NamePlateList = new List<OrderDetail>();
            List<OrderDetail> OtherList = new List<OrderDetail>();
            foreach (OrderDetail item in ods)
            {
                if (item.Product.TypeID == ProductTypes.Stockboard)
                {
                    sbList.Add(item);
                }
                else if (item.Product.TypeID == ProductTypes.Overlay && (item.Product.Name.Contains("Name") || item.Product.Name.Contains("Overlay") ||
                        item.Product.ProductID == ProductSettings.UnitSticker))
                {
                    NamePlateList.Add(item);
                }
                else if (item.Product.TypeID == ProductTypes.BoardAccessory && item.Product.ProductID == ProductSettings.FlagHolder)
                {
                    NamePlateList.Add(item);
                }
                else
                {
                    OtherList.Add(item);
                }
            }

            if (OtherList.Count > 0)
            {
                return true;
            }
            else if (sbList.Count == 0 && NamePlateList.Count > 0)
            {
                return true;
            }
            else if (sbList.Count > 0 && NamePlateList.Count >= 0)
            {
                return false;
            }

            return false;
        }

        #endregion

        #region IsChangeRequestReproofAndApproveApplicable
        public bool IsChangeRequestReproofAndApproveApplicable(int orderID)
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
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (!od.ProofDetail.DateApproved.HasValue &&
                         !od.OrderDetails.Any(ods => (ods.UserDesignOnline.HasValue && ods.UserDesignOnline.Value)))
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsChangeRequestReproofAndApproveApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsRequestForRemovalApplicable
        public bool IsRequestForRemovalApplicable(int orderID)
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
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                    loadOptions.Add(EntityRelations.Client_To_Manager);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (od.DespatchDetail.DateDespatched.HasValue &&
                    od.DespatchDetail.DateBoardErected.HasValue &&
                    !od.DespatchDetail.DateRemovalRequested.HasValue &&
                    od.OrderDetails.Any(ods => (ods.Product.TypeID == ProductTypes.BillBoard || ods.Product.TypeID == ProductTypes.Stockboard || ods.Product.TypeID == ProductTypes.BoardPackages)))
                    {
                        return true;
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsRequestForRemovalApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsBoardErectionDetailsApplicable
        public bool IsBoardErectionDetailsApplicable(int orderID)
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
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (!od.DespatchDetail.InTransitDt.HasValue && !od.DespatchDetail.DateBoardErected.HasValue)
                    {
                        if (od.OrderDetails != null && od.OrderDetails.Count > 0)
                        {
                            foreach (OrderDetail item in od.OrderDetails)
                            {
                                int id = item.Product.TypeID;
                                if (id == 1 || id == 4 || id == 9 || id == 10 ||
                                     id == 14 || id == 16)
                                {
                                    return true;
                                }
                                else if (id == 5 && (od.Client.ManagerID == ManagerSettings.WorkshopVictoria || od.Client.ManagerID == ManagerSettings.SignshopVictoria))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsBoardErectionDetailsApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region IsBoardRemovalDetailsApplicable
        public bool IsBoardRemovalDetailsApplicable(int orderID)
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
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (!od.DespatchDetail.DateRemovalRequested.HasValue && !od.DespatchDetail.DateBoardRemoved.HasValue)
                    {
                        if (od.OrderDetails != null && od.OrderDetails.Count > 0)
                        {
                            foreach (OrderDetail item in od.OrderDetails)
                            {
                                int id = item.Product.TypeID;
                                if (id == 1 || id == 4 || id == 9 || id == 10 ||
                                     id == 14 || id == 16)
                                {
                                    return true;
                                }
                                else if (id == 5 && (od.Client.ManagerID == ManagerSettings.WorkshopVictoria || od.Client.ManagerID == ManagerSettings.SignshopVictoria))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsBoardRemovalDetailsApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsCancelOnlineDesignApplicable

        public bool IsCancelOnlineDesignApplicable(int orderID)
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
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (!od.ProofDetail.DateApproved.HasValue &&
                         od.OrderDetails.Any(ods => (ods.UserDesignOnline.HasValue && ods.UserDesignOnline.Value)))
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsCancelOnlineDesignApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        public List<Entities.Order> GetOrdersSubsetByPropertyAddress(string submittedPropertyAddress)
        {
            try
            {
                if (!string.IsNullOrEmpty(submittedPropertyAddress))
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        var ord = (from order in ctx.Orders
                                   join location in ctx.Locations on order.LocationID equals location.LocationID
                                   where order.PropertyAddress.Contains(submittedPropertyAddress.ToLower())
                                   orderby order.OrderID descending
                                   select order).ToList().Select(e => new Order
                                      {
                                          OrderID = e.OrderID,
                                          Location = e.Location != null ? new Location()
                                          {
                                              LocationID = e.Location.LocationID,
                                              Location1 = e.Location.Location1
                                          } : null,
                                          PropertyAddress = e.PropertyAddress,
                                          LocationID = e.LocationID,

                                      }).ToList();

                        return ord;
                    }
                }

                return null;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetOrdersSubsetByPropertyAddress()");
                throw oEx;
            }
        }

        public List<Order> GetOrderSubsetByOrderRange(int submittedGroupStartId, int submittedGroupEndOrderId)
        {
            try
            {
                if (submittedGroupStartId >= 0 && submittedGroupEndOrderId >= submittedGroupStartId)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        var ord = (from order in ctx.Orders
                                   join location in ctx.Locations on order.LocationID equals location.LocationID
                                   orderby order.OrderID
                                   where (order.OrderID >= submittedGroupStartId && order.OrderID <= submittedGroupEndOrderId)
                                   select order).ToList().Select(e => new Order
                                      {
                                          OrderID = e.OrderID,
                                          Location = e.Location != null ? new Location()
                                          {
                                              LocationID = e.Location.LocationID,
                                              Location1 = e.Location.Location1
                                          } : null,
                                          PropertyAddress = e.PropertyAddress,
                                          LocationID = e.LocationID,
                                      }).ToList();
                        return ord;
                    }
                }

                return null;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetOrderSubsetByOrderRange()");
                throw oEx;
            }
        }

        #region AddBoardErectionDetails
        public void AddBoardErectionDetails(int orderID, string eNotes, int prefType, DateTime? prefDate, string installationFile)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (prefType < -1 || prefType > 1)
            {
                throw new ArgumentNullException("prefType");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Order_To_OrderOtherDetail);


                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    //Update Erection Notes and Preferred Datetime
                    if (string.IsNullOrEmpty(od.ErectionNotes))
                        od.ErectionNotes = eNotes;
                    else
                        od.ErectionNotes = od.ErectionNotes + ". " + eNotes;

                    od.DespatchDetail.PreferredErectionDate = prefDate;
                    od.DespatchDetail.PreferredErectionType = prefType;

                    string prefErectionDate = string.Empty;
                    if (prefDate != null)
                    {
                        if (prefType == -1)
                            prefErectionDate = "Before or On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 0)
                            prefErectionDate = "On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 1)
                            prefErectionDate = "On or After" + prefDate.Value.ToString("dd/MM/yyyy");
                    }

                    //Try to move installation file to server first
                    bool moveInsFileSuccess = false;
                    string destFile = string.Empty;
                    string diagramUploaded = string.Empty;
                    if (!string.IsNullOrEmpty(installationFile))
                    {
                        if (System.IO.File.Exists(installationFile))
                        {
                            string fileExt = Path.GetExtension(installationFile);
                            string fName = orderID.ToString() + "_" + Guid.NewGuid().ToString("N") + fileExt;

                            //remove as install file
                            //string filter = string.Format("{0}*", orderID.ToString() + "_InstallFile");
                            //foreach (string oldfile in Directory.GetFiles(OnlineBLConfig.INSTALLATION_FILE_PATH, filter))
                            //{
                            //    try
                            //    {
                            //        if (File.Exists(oldfile))
                            //            File.Delete(oldfile);
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Logger.Exception(ex, oldfile);
                            //    }
                            //}


                            destFile = System.IO.Path.Combine(OnlineBLConfig.INSTALLATION_FILE_PATH, fName);
                            System.IO.File.Copy(installationFile, destFile, true);
                            System.IO.File.Delete(installationFile);
                            moveInsFileSuccess = true;
                            diagramUploaded = " -- " + DateTime.Now.ToString() + " Installation File has been uploaded";
                        }
                    }

                    if (moveInsFileSuccess)
                    {
                        if (string.IsNullOrEmpty(od.ErectionNotes))
                            od.ErectionNotes = diagramUploaded;
                        else
                            od.ErectionNotes = od.ErectionNotes + ".\r\n" + diagramUploaded;

                        if (od.OrderOtherDetail != null)
                        {
                            od.OrderOtherDetail.InstallFile = destFile;
                        }
                        else
                        {
                            od.OrderOtherDetail = new OrderOtherDetail();
                            od.OrderOtherDetail.OrderId = orderID;
                            od.OrderOtherDetail.InstallFile = destFile;
                            ctx.OrderOtherDetails.InsertOnSubmit(od.OrderOtherDetail);
                        }

                    }


                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.ErectionNotesChanged;
                    string sub = "Abc Notification: Erection Notes Changed for Job No " + od.OrderID;
                    string xmlData = @"<EVENT>
										<OrderID>" + od.OrderID + @"</OrderID>
										<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
										<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
										<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
										<Notes>" + od.ErectionNotes.Replace("&", "&amp;") + @"</Notes>
										<PreferredErectionDate>" + prefErectionDate + @"</PreferredErectionDate>
										<ReceivedOn>" + DateTime.Today.ToString("dd/MM/yyyy") + @"</ReceivedOn>
										</EVENT>";

                    string textData = "Erection Notes Changed for Job No " + od.OrderID;
                    string source = "OnlineBL_OrderService_AddBoardErectionDetails";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, moveInsFileSuccess ? destFile : String.Empty);

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'AddBoardErectionDetails'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region AddBoardRemovalDetails
        public string AddBoardRemovalDetails(int orderID, string rNotes, int prefType, DateTime? prefDate, string byWhom)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (prefType < -1 || prefType > 1)
            {
                throw new ArgumentNullException("prefType");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);


                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    //Update Request Note and Preferred Datetime
                    if (string.IsNullOrEmpty(od.RemovalNotes))
                        od.RemovalNotes = rNotes;
                    else
                        od.RemovalNotes = od.RemovalNotes + ". " + rNotes;

                    od.RemovalNotes = od.RemovalNotes + " - Requested by: " + byWhom;

                    od.DespatchDetail.PreferredRemovalDate = prefDate;
                    od.DespatchDetail.PreferredRemovalType = prefType;

                    string prefRemovalDate = string.Empty;
                    if (prefDate != null)
                    {
                        if (prefType == -1)
                            prefRemovalDate = "Before or On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 0)
                            prefRemovalDate = "On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 1)
                            prefRemovalDate = "On or After" + prefDate.Value.ToString("dd/MM/yyyy");

                    }

                    od.RemovalNotes = od.RemovalNotes + " " + prefRemovalDate;

                    if (string.IsNullOrEmpty(rNotes))
                    {
                        rNotes = " ";
                    }

                    //ROnlineBLe an Event to send email notification to Info
                    int eventID = EventSettings.RemovalNotesChanged;
                    string sub = "Abc Notification: Removal Notes Changed for Job No " + od.OrderID;
                    string xmlData = @"<EVENT>
										<OrderID>" + od.OrderID + @"</OrderID>
										<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
										<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
										<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
										<Notes>" + rNotes.Replace("&", "&amp;") + @"</Notes>
										<PreferredRemovalDate>" + prefRemovalDate + @"</PreferredRemovalDate>
										<ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
										</EVENT>";

                    string textData = "Removal Notes Changed for Job No " + od.OrderID;
                    string source = "OnlineBL_OrderService_AddBoardRemovalDetails";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);

                    ctx.SubmitChanges();
                    return "Success";

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'AddBoardRemovalDetails'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region SendOtherCommentQueries
        public void SendOtherCommentQueries(int orderID, string comment, string byWhom)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(comment))
            {
                throw new ArgumentNullException("comment");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.AgentsCommentsRegardingJob;
                    string sub = "Agents Comments/Queries for Job No " + od.OrderID;
                    string xmlData = @"<HTML><HEAD></HEAD><BODY>
										<P>
										<H3>An Agent has sent Comments/Queries Regarding a Job in our system.</H3>
										</P>
										<P>
										<B><U>Details:</U></B>
										</P>
										<P>
										Agent Name   : " + od.Client.ClientName + "/" + od.Client.Office + @"<BR>
										Email Address: " + od.Client.Email + @"<BR>
										Job No       : " + od.OrderID + @"<BR>
										Send By      : " + byWhom + @"<BR>
										Query        : <BR>" + comment + @"<BR>
										</P>
										</BODY>
										</HTML>";

                    string source = "OnlineBL_OrderService_SendOtherCommentQueries";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, xmlData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'SendOtherCommentQueries'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region CancelOnlineDesignJob
        public void CancelOnlineDesignJob(int orderID, string comment, string byWhom)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(comment))
            {
                throw new ArgumentNullException("comment");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                bool isExpressClient = false;
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Client_To_ClientsPrefs);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    ClientsPref cp = (from p in ctx.ClientsPrefs
                                      join c in ctx.Clients on p.ClientId equals c.ClientID
                                      join o in ctx.Orders on c.ClientID equals o.ClientID
                                      where o.OrderID == orderID && p.PrefID == ClientsPref.RedirectToExpressSite
                                      select p).FirstOrDefault();

                    if (cp != null && cp.BitValue.HasValue && cp.BitValue == true)
                    {
                        isExpressClient = true;
                    }
                }

                if (isExpressClient)
                {
                    Logger.Warn("Could not cancel as this is express client: orderid -- " + orderID.ToString());
                    return;
                }

                int clientID;
                bool hasAOPJob = false;
                try
                {
                    // check if order has AOP job
                    hasAOPJob = IsDesignNowApplicable(orderID);
                    if (hasAOPJob)
                    {
                        UpdateXMLOnCancelation(orderID);

                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'UpdateXMLOnCancelation'. orderID:{0}", orderID);
                    Logger.Exception(ex, message);
                }

                //Continue to save
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    clientID = od.ClientID;

                    //Update OrderDetails
                    od.OrderDetails.ToList().ForEach(odd => odd.UserDesignOnline = false);

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.CancelAOPJobs;
                    string sub = "Cancel Online Design Job, Let Abc Photosigns design it";

                    string approvalNotes = "-";

                    if (comment.IndexOf("** Make Changes and APPROVE to print **") > 0)
                    {
                        approvalNotes = "** Make Changes and APPROVE to print **";

                    }
                    else if (comment.IndexOf("** Make Changes and Send Proof **") > 0)
                    {
                        approvalNotes = "** Make Changes and Send Proof **";
                    }

                    string xmlData = @"
									<EVENT>
									<OrderId>" + orderID + @"</OrderId>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
									<RequestedBy>" + byWhom + @"</RequestedBy>
									<Notes>" + comment.Replace("&", "&amp;") + @"</Notes>
                                    <FurtherInstruction>" + approvalNotes + @"</FurtherInstruction>
									<ReceivedOn>" + DateTime.Today.ToString("dd/MM/yyyy") + @"</ReceivedOn>
									</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Cancel Online Design Job, Let Abc Photosigns design it." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Requested By: " + byWhom +
                                            Environment.NewLine +
                                            @"Notes: " + comment +
                                            @"Further Instruction: " + approvalNotes;

                    string source = "OnlineBL_OrderService_CancelOnlineDesignJob";

                    od.Notes = "---Previous DIY---\r\n" + od.Notes;

                    if (string.IsNullOrEmpty(od.Caption) || od.Caption.StartsWith("DIY"))
                    {
                        //temp empty
                    }

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CancelOnlineDesignJob'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #endregion

        #region CancelOnlineDesignExpressJob
        public void CancelOnlineDesignExpressJob(int orderID, string comment, string byWhom)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(comment))
            {
                throw new ArgumentNullException("comment");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                int clientID;
                bool hasAOPJob = false;
                try
                {
                    // check if order has AOP job
                    hasAOPJob = IsDesignNowApplicable(orderID);
                    if (hasAOPJob)
                    {
                        UpdateXMLOnCancelation(orderID);

                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'UpdateXMLOnCancelation'. orderID:{0}", orderID);
                    Logger.Exception(ex, message);
                }

                //Continue to save
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Order_To_AOP_JobDocuments);


                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    clientID = od.ClientID;

                    //Update OrderDetails
                    od.OrderDetails.ToList().ForEach(odd => odd.UserDesignOnline = false);

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.CancelAOPJobs;
                    string sub = "Cancel Online Design Job, Let Abc Photosigns design it";

                    string approvalNotes = "-";

                    if (comment.IndexOf("** Make Changes and APPROVE to print **") > 0)
                    {
                        approvalNotes = "** Make Changes and APPROVE to print **";

                    }
                    else if (comment.IndexOf("** Make Changes and Send Proof **") > 0)
                    {
                        approvalNotes = "** Make Changes and Send Proof **";
                    }

                    string xmlData = @"
									<EVENT>
									<OrderId>" + orderID + @"</OrderId>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
									<RequestedBy>" + byWhom + @"</RequestedBy>
									<Notes>" + comment.Replace("&", "&amp;") + @"</Notes>
                                    <FurtherInstruction>" + approvalNotes + @"</FurtherInstruction>
									<ReceivedOn>" + DateTime.Today.ToString("dd/MM/yyyy") + @"</ReceivedOn>
									</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Cancel Online Design Job, Let Abc Photosigns design it." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Requested By: " + byWhom +
                                            Environment.NewLine +
                                            @"Notes: " + comment +
                                            @"Further Instruction: " + approvalNotes;

                    string source = "OnlineBL_OrderService_CancelOnlineDesignJob";

                    od.Notes = "---Previous DIY---\r\n" + od.Notes;

                    if (string.IsNullOrEmpty(od.Caption) || od.Caption.StartsWith("DIY"))
                    {
                        foreach (AOP_JobDocument aopJob in od.AOP_JobDocuments)
                        {
                            if (aopJob.TempDocumentModel != null)
                            {
                                Abc.OnlinePublication.Common.DOM.Document doc = Dom.Document.Deserialize(aopJob.TempDocumentModel.ToString());

                                Abc.OnlinePublication.Common.DOM.Tag captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "Heading"; });
                                if (captionTag != null)
                                {
                                    if (captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                }
                                captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BodyCopy"; });
                                if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                {
                                    od.Caption = captionTag.FormElement.Value;
                                    if (captionTag.FormElement.Value.Length > 20)
                                    {
                                        od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                    }
                                    break;
                                }
                                captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureHeading"; });
                                if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                {
                                    od.Caption = captionTag.FormElement.Value;
                                    if (captionTag.FormElement.Value.Length > 20)
                                    {
                                        od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                    }
                                    break;
                                }
                                captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureBodyCopy"; });
                                if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                {
                                    od.Caption = captionTag.FormElement.Value;
                                    if (captionTag.FormElement.Value.Length > 20)
                                    {
                                        od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'CancelOnlineDesignExpressJob'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #endregion

        #region MakeOrderAsConjunctionOffer
        public void MakeOrderAsConjunctionOffer(int orderID, string comment, string byWhom, string contactNumber, string filePath)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(comment))
            {
                throw new ArgumentNullException("comment");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                int clientID;
                try
                {
                    UpdateXMLOnCancelation(orderID);
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'UpdateXMLOnCancelation'. orderID:{0}", orderID);
                    Logger.Exception(ex, message);
                }

                //Continue to save
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Order_To_AOP_JobDocuments);


                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    clientID = od.ClientID;

                    //Update OrderDetails
                    od.OrderDetails.ToList().ForEach(odd => odd.UserDesignOnline = false);

                    //Update Notes
                    od.Notes = "---Previous DIY---\r\n" + od.Notes;

                    //Update Caption
                    if (string.IsNullOrEmpty(od.Caption) || od.Caption.ToUpper().StartsWith("DIY"))
                    {
                        foreach (AOP_JobDocument aopJob in od.AOP_JobDocuments)
                        {
                            if (aopJob.TempDocumentModel != null)
                            {
                                Abc.OnlinePublication.Common.DOM.Document doc = Dom.Document.Deserialize(aopJob.TempDocumentModel.ToString());

                                Abc.OnlinePublication.Common.DOM.Tag captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "Heading"; });
                                if (captionTag != null)
                                {
                                    if (captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                }
                                captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BodyCopy"; });
                                if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                {
                                    od.Caption = captionTag.FormElement.Value;
                                    if (captionTag.FormElement.Value.Length > 20)
                                    {
                                        od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                    }
                                    break;
                                }
                                captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureHeading"; });
                                if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                {
                                    od.Caption = captionTag.FormElement.Value;
                                    if (captionTag.FormElement.Value.Length > 20)
                                    {
                                        od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                    }
                                    break;
                                }
                                captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureBodyCopy"; });
                                if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                {
                                    od.Caption = captionTag.FormElement.Value;
                                    if (captionTag.FormElement.Value.Length > 20)
                                    {
                                        od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    string orderEmail = od.SendProofTo;

                    if (string.IsNullOrEmpty(orderEmail))
                    {
                        orderEmail = od.Client.Email;
                    }

                    //ROnlineBLe an Event to send email notification to Admin

                    int eventID = EventSettings.ChangeToConjunctionalOrder;
                    string sub = "Change To Conjunctional Order";
                    string xmlData = @"
									<EVENT>
									<OrderID>" + orderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                    <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
									<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
									<ContactName>" + byWhom + @"</ContactName>
									<ContactNumber>" + contactNumber + @"</ContactNumber>
									<RequestDetails>" + comment.Replace("&", "&amp;") + @"</RequestDetails>
									</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Make Order As Conjunction Offer." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Requested By: " + byWhom +
                                            Environment.NewLine +
                                            @"Requested Number: " + contactNumber +
                                            Environment.NewLine +
                                            @"Reason: " + comment;

                    string source = "OnlineBL_OrderService_MakeOrderAsConjunctionOffer";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(filePath) ? filePath : String.Empty);

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'MakeOrderAsConjunctionOffer'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region ApproveExpressJob
        public void ApproveExpressJob(int orderID, string byWhom, string purchaseOrderNumber)
        {
            bool clientHasStockboardDIY = false;
            int clientID;

            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);
                    clientID = od.ClientID;

                    var paygClient = ctx.ClientsPrefs.Where(x => x.ClientId == od.ClientID && x.PrefID == ClientsPref.PayAsYouGo).FirstOrDefault();

                    var cpHasSBDIY = ctx.ClientsPrefs.Where(x => x.ClientId == od.ClientID && x.PrefID == Abc.OnlineBL.Entities.Utility.Preferences.StockboardDIY).FirstOrDefault();

                    if (cpHasSBDIY != null && cpHasSBDIY.BitValue.HasValue && cpHasSBDIY.BitValue == true)
                        clientHasStockboardDIY = true;

                    if (paygClient != null && paygClient.BitValue == true && od.OrderStatus > 0)
                    {
                        throw new Exception(string.Format("Your job can not be approved at this time. Your invoice is ready. Click {0} to pay.", "<a href='/Abc.Web.Express/Accounts/PaymentSelection' target='_parent'>here</a>"));
                    }
                    else if (od.OrderStatus > 0)
                    {
                        throw new Exception("Your job can not be approved at this time, please contact 03 9313 0953 for further assistance in this matter");
                    }
                    else if (od != null && od.OnHold.HasValue && od.Notes.Contains("On Hold – awaiting photography to be completed"))
                    {
                        string msg = @"Apologies for the inconvenience, this order is currently on hold awaiting the photography to be completed first. 
                                       Once the photography shoot has been completed, you will be able to approve your stockboard for installation and complete your brochure design.
                                       If you require further assistance or information, please contact our Head Office on (03) 9313 0999.";
                        Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), msg));
                        throw new Exception(msg);
                    }
                    else if (od != null && od.OnHold.HasValue)
                    {
                        string msg = "Your job on hold and can not be approved at this time, please contact 03 9313 0953 for further assistance in this matter.";
                        Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), msg));
                        throw new Exception(msg);
                    }
                }

                // check if order has AOP job
                bool hasAOPJob = IsDesignNowApplicable(orderID);
                bool hasOnlineListing = OrderHasOnlineListing(orderID);
                bool hasStockboard = OrderHasStockboard(orderID);
                bool IsStockboardApproved = IsStockBoardApproved(orderID);

                if (hasAOPJob)
                {
                    if (!ServiceConfig.IS_NZ && hasOnlineListing)
                    {
                        UpdateXMLOnApproval(orderID);
                    }
                    //move document and preview
                    MoveDocumentsAndPreviewFiles(orderID);
                }

                //Continue to save
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (clientHasStockboardDIY)
                    {
                        List<EntityRelations> loadOptions = new List<EntityRelations>();
                        loadOptions.Add(EntityRelations.Order_To_Client);
                        loadOptions.Add(EntityRelations.Order_To_Location);
                        loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                        loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                        loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                        loadOptions.Add(EntityRelations.OrderDetail_To_AOP_JobDocuments);

                        ctx.DeferredLoadingEnabled = false;
                        ctx.SetDataLoadOptions(loadOptions);

                        Order order = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                        if (!hasStockboard || IsStockboardApproved)
                        {
                            ctx.CDAS_ApproveJob(order.ClientID, order.OrderID, byWhom);
                        }
                        else
                        {
                            //dont need to check if OrderHasOverlayExcludeUnitStickerAndNamePlates
                            if (orderID < 99000000)
                            {
                                try
                                {
                                    Logger.Warn("Approve Order has stockboard");

                                    if (order.OrderStatus <= 0)
                                    {
                                        var result = ctx.ACC_GetStockOrderDetails(orderID);

                                        string prop = order.PropertyAddress + ", " + order.Location.Location1;

                                        if (result != null)
                                        {
                                            bool usePrint = false;
                                            string xmlDataItems = string.Empty;
                                            var stockBoardOrderDetailIds = new List<int>();
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

                                                stockBoardOrderDetailIds.Add(item.OrderDetailsID);
                                            }

                                            //send email notification to manager
                                            int eventID = EventSettings.StockboardProgressToManagers;
                                            string sub = "Stockboard Order Progress";
                                            string xmlData = string.Empty;

                                            xmlData = @"<EVENT>
								                    <ORDERID>" + orderID + @"</ORDERID>
                                                    <AGENT>" + order.Client.ClientName.Replace("&", "&amp;") + "/" + order.Client.Office + @"</AGENT>
								                    <PADDRESS>" + prop.Replace("&", "&amp;") + @"</PADDRESS>
                                                    <CAPTION>Workshop Stockboard Jobs for Printing Decision</CAPTION>
                                                    <DATERECEIVED>" + DateTime.Now.ToString() + @"</DATERECEIVED>
                                                    <ITEMS>" + xmlDataItems + @"</ITEMS>
								                    </EVENT>";

                                            string textData = xmlData;
                                            string source = "ProcessOrder";

                                            ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, orderID, null, order.ManagerID, null, source, String.Empty);
                                            ctx.SubmitChanges();

                                            if (usePrint)
                                            {
                                                ctx.ACC_ApproveOnly(orderID);
                                            }
                                            else
                                            {
                                                bool hasAnyOtherProductsLikeOverlay = order.OrderDetails.Any(x => x.Product.TypeID != ProductTypes.Stockboard);
                                                bool hasAnyOtherProductsOtherThanStockboardOrBoarOverlay = order.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.Brochure || x.Product.TypeID == ProductTypes.WindowCard || x.Product.TypeID == ProductTypes.BillBoard);
                                                string path = DirectoryPath.GetOutputFolder(orderID, ServiceConfig.APPROVED_DOC_DIR, Abc.OnlinePublication.Common.FolderStructureType.Hundred);

                                                Logger.Warn("STOCKBOARD - USE FROM");
                                                try
                                                {
                                                    foreach (OrderDetail detail in order.OrderDetails)
                                                    {
                                                        if (detail.Product.TypeID == ProductTypes.Stockboard)
                                                        {
                                                            detail.ItemNote += "DONT PRINT STOCKBOARD - USE FROM STOCK";

                                                            //need to rename or remove the SB file, as we dont want to put SB to print
                                                            string inddPattern = string.Format("{0}_{1}_{2}.indd", orderID, detail.AOP_JobDocuments[0].JobDocumentId, Path.GetFileNameWithoutExtension(detail.AOP_JobDocuments[0].TemplateOriginalFileName));
                                                            string sourceFile = Path.Combine(path, inddPattern);
                                                            string destFile = Path.Combine(path, inddPattern.Replace("_B", "_SB"));
                                                            Logger.Warn("destFile: " + destFile);
                                                            File.Move(sourceFile, destFile);
                                                        }
                                                    }
                                                    ctx.SubmitChanges();
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Exception(ex, "Rename Stockboard file has an issue");
                                                }

                                                //Approve only
                                                if (order.ManagerID == ManagerSettings.WorkshopVictoria)
                                                {
                                                    //This will approve everything 
                                                    ctx.ABCWRKFLOW_ApproveOrder(orderID, DateTime.Now, null, null, null, hasAnyOtherProductsOtherThanStockboardOrBoarOverlay);

                                                    //refactor this
                                                    bool sendJobsheet = false;
                                                    if (order.DespatchDetail != null && order.DespatchDetail.PreferredErectionDate.HasValue)
                                                    {

                                                        DateTime now = DateTime.Now;
                                                        DateTime later = order.DespatchDetail.PreferredErectionDate.Value;

                                                        int days = (later - now).Days;

                                                        if (days > 4)
                                                            sendJobsheet = false;
                                                        else
                                                            sendJobsheet = true;
                                                    }
                                                    else
                                                    {
                                                        sendJobsheet = true;
                                                    }

                                                    if (sendJobsheet)
                                                    {
                                                        ctx.FrontOffice_ServiceQueueAdd("PrintJobsheet", "OrderID=" + orderID, 0, 2, null, @"\\adbb\stockboarddepot", null, null, "Online Order Processor", 2);
                                                    }
                                                    else
                                                    {
                                                        Logger.Warn("Stockboard DIY Order: " + order.OrderID);
                                                        try
                                                        {
                                                            BoardApproval ba = new BoardApproval();
                                                            ba.OrderID = order.OrderID;
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
                                                    if (hasAnyOtherProductsLikeOverlay)
                                                    {
                                                        //approve send to print only if order has overlay
                                                        ctx.ABCWRKFLOW_ApproveOrder(orderID, DateTime.Now, null, null, null, hasAnyOtherProductsLikeOverlay);
                                                    }
                                                    else
                                                    {
                                                        ctx.ACC_ApproveAndDespatch(orderID);
                                                    }
                                                }
                                            }

                                            ctx.SubmitChanges();

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = string.Format("Error occured in 'validating SB Inventory'. orderID:{0}", orderID);
                                    Logger.Exception(ex, message);
                                }

                                ctx.SP_EventGen_ApprovedByClient(orderID, byWhom, DateTime.Now, "ApproveExpressJob");

                            }
                        }
                    }
                    else
                    {
                        Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);
                        clientID = od.ClientID;
                        ctx.CDAS_ApproveJob(od.ClientID, od.OrderID, byWhom);
                    }

                    ctx.SubmitChanges();

                    if (!string.IsNullOrEmpty(purchaseOrderNumber))
                    {
                        try
                        {
                            OrderOtherInfo of = ctx.OrderOtherInfos.SingleOrDefault(o => o.OrderID == orderID);
                            if (of != null)
                            {
                                of.PurchaseOrder = purchaseOrderNumber;
                            }
                            else
                            {
                                of = new OrderOtherInfo();
                                of.OrderID = orderID;
                                of.PurchaseOrder = purchaseOrderNumber;
                                ctx.OrderOtherInfos.InsertOnSubmit(of);
                            }
                            ctx.SubmitChanges();

                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, "Error occured in 'ApproveExpressJob'. Saving Purchase Order Number");
                        }
                    }

                }

                //begin new code to update the caption
                if (hasAOPJob)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        List<EntityRelations> loadOptions = new List<EntityRelations>();
                        loadOptions.Add(EntityRelations.Order_To_AOP_JobDocuments);
                        loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                        loadOptions.Add(EntityRelations.AOP_JobDocument_To_OrderDetail);
                        loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                        loadOptions.Add(EntityRelations.Order_To_OrderOtherDetail);


                        ctx.DeferredLoadingEnabled = false;
                        ctx.SetDataLoadOptions(loadOptions);

                        Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                        bool orderHasStockboard = od.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.Stockboard);
                        bool orderHasBoard = od.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.BillBoard);

                        try
                        {
                            if (string.IsNullOrEmpty(od.Caption) || od.Caption.ToUpper().StartsWith("DIY"))
                            {
                                foreach (AOP_JobDocument aopJob in od.AOP_JobDocuments.OrderByDescending(aop => aop.TemplateOriginalFileName.Contains("_B.indt")))
                                {
                                    if (aopJob.TempDocumentModel != null)
                                    {
                                        Abc.OnlinePublication.Common.DOM.Document doc = Dom.Document.Deserialize(aopJob.TempDocumentModel.ToString());

                                        Abc.OnlinePublication.Common.DOM.Tag captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "Heading"; });
                                        if (captionTag != null)
                                        {
                                            if (captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                            {
                                                od.Caption = GetCaption(captionTag);
                                                break;
                                            }
                                        }
                                        captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BodyCopy"; });
                                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                        {
                                            od.Caption = GetCaption(captionTag);
                                            break;
                                        }
                                        captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureHeading"; });
                                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                        {
                                            od.Caption = GetCaption(captionTag);
                                            break;
                                        }
                                        captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureBodyCopy"; });
                                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                        {
                                            od.Caption = GetCaption(captionTag);
                                            break;
                                        }
                                    }
                                }
                            }
                            if (clientHasStockboardDIY)
                            {
                                //clear the caption if order does not have Board
                                if (orderHasStockboard && !orderHasBoard)
                                {
                                    od.Caption = string.Empty;
                                }

                                var stockBoardCaption = GetStockboardName(od);

                                if (!string.IsNullOrEmpty(stockBoardCaption))
                                {
                                    if (string.IsNullOrEmpty(od.Caption))
                                        od.Caption = string.Format("{0}", stockBoardCaption);
                                    else if (!od.Caption.Contains(stockBoardCaption))
                                    {
                                        if (od.Caption.ToUpper() != "DIY")
                                            od.Caption = stockBoardCaption + "-" + od.Caption;
                                        else
                                            od.Caption = stockBoardCaption;
                                    }

                                }

                                var caption = GetNamePlateAndUnitStickerCaption(od);

                                if (!string.IsNullOrEmpty(caption))
                                {
                                    if (string.IsNullOrEmpty(od.Caption))
                                        od.Caption = string.Format("{0}", caption);
                                    else
                                        if (!od.Caption.Contains(caption))
                                        {
                                            od.Caption += string.Format(" - {0}", caption);
                                        }
                                }

                                var brochureCaption = GetBrochureCaption(od);
                                if (!string.IsNullOrEmpty(brochureCaption))
                                {
                                    if (od.OrderOtherDetail == null)
                                    {
                                        od.OrderOtherDetail = new OrderOtherDetail();
                                    }
                                    od.OrderOtherDetail.BrochureCaption = brochureCaption;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            Logger.Exception(ex, "Error updating order caption" + orderID.ToString());
                        }


                        ctx.SubmitChanges();
                    }
                }

                try
                {
                    if (hasAOPJob)
                    {
                        //Move working DIY folder
                        if (!ServiceConfig.IS_NZ)
                            MoveWorkingDIYFolder(orderID, clientID);
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'ApproveJob'. orderID:{0}", orderID);
                    Logger.Exception(ex, message);
                }

                try
                {
                    if (!ServiceConfig.IS_NZ && hasOnlineListing)
                    {
                        //Move xml file
                        string xmlSourceFile = Path.Combine(ServiceConfig.XML_ORDER_DIR, orderID + ".xml");
                        if(File.Exists(xmlSourceFile))
                        {
                            string destFile = Path.Combine(ServiceConfig.ABC_LISTING_DIR, orderID + ".xml");
                            File.Copy(xmlSourceFile, destFile, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'ApproveJob - Online Listing'. orderID:{0}", orderID);
                    Logger.Exception(ex, message);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ApproveJob'. orderID:{0}", orderID);
                Logger.Exception(ex, message);

                if (byWhom == "B2B Auto Approve" && ex.Message != "Job is already approved")
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        Order or = ctx.Orders.FirstOrDefault(o => o.OrderID == orderID);
                        //ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.B2BAutoApprovalFailed;
                        string sub = "Abc Notification: B2B Auto Approval Failed (Order no: " + orderID + ")";
                        string xmlData = @"<EVENT>
								<OrderID>" + orderID + @"</OrderID>
								<ClientID>" + or.ClientID + @"</ClientID>
								<Description>" + ex.Message.Replace("&", "&amp;") + @"</Description>
								<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
								</EVENT>";


                        string source = "OnlineBL_OrderService_ApproveExpressJob";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, xmlData, null, null, null, null, source, String.Empty);

                        ctx.SubmitChanges();
                    }
                }

                throw ex;
            }
        }

        #endregion

        #region ApproveJob
        public void ApproveJob(int orderID, string byWhom)
        {
            ApproveExpressJob(orderID, byWhom, string.Empty);
        }

        private void MoveWorkingDIYFolder(int orderID, int clientID)
        {
            try
            {
                if (orderID < 1)
                    return;

                string workingPath = String.Format(@"{0}\{1}\{2}", ServiceConfig.AOP_TEMPLATE_ROOT_DIR, clientID, orderID);

                string destPath = String.Format(@"{0}\{1}\{2}", ServiceConfig.AOP_WORKING_DIR, clientID, orderID);

                //Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(workingPath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(workingPath, destPath));

                //Move all the files
                foreach (string newPath in Directory.GetFiles(workingPath, "*.*",
                    SearchOption.AllDirectories))
                    File.Move(newPath, newPath.Replace(workingPath, destPath));

                // After moving the files, delete the folder.
                try
                {
                    if (Directory.Exists(workingPath))
                    {
                        Directory.Delete(workingPath, true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, workingPath);
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'MoveWorkingDIYFolder'. orderID:{0}", orderID);
                Logger.Exception(ex, message);

                try
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        ctx.OnlineServiceQueue_AddMoveWorkingDIYFolderToQueue(orderID);

                        ctx.SubmitChanges();
                    }
                }
                catch (Exception exc)
                {
                    string mesg = string.Format("Error occured in 'MoveWorkingDIYFolder' add to queue. orderID:{0}", orderID);
                    Logger.Exception(ex, mesg);
                }
            }
        }

        private void MoveExpressDIYWorkingFolder(int orderID, int clientID, List<int> jobDocIds)
        {
            try
            {
                if (orderID < 1)
                    return;

                //move the working job document folder
                foreach (int jobDocumentID in jobDocIds)
                {
                    try
                    {
                        string workingPath = String.Format(@"{0}\{1}\{2}\{3}", ServiceConfig.AOP_TEMPLATE_ROOT_DIR, clientID, orderID, jobDocumentID);

                        //New AOP Modify DIY folder
                        string destPath = String.Format(@"{0}\{1}\{2}\{3}", ServiceConfig.AOP_WORKING_DIR, clientID, orderID, jobDocumentID);

                        if (!Directory.Exists(destPath))
                        {
                            Directory.CreateDirectory(destPath);
                        }

                        //Move all the files
                        foreach (string newPath in Directory.GetFiles(workingPath, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(workingPath, destPath), true);

                        // After moving the files, delete the folder.
                        try
                        {
                            if (Directory.Exists(workingPath))
                            {
                                Directory.Delete(workingPath, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, workingPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format("Error occured in moving the working job document folder on function 'ModifyDIYTemplate '. orderID:{0}", orderID);
                        Logger.Exception(ex, message);
                    }
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'MoveExpressDIYWorkingFolder'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
            }
        }

        #endregion

        #region RequestBoardRemoval
        public string RequestBoardRemoval(int orderID, string rNotes, string byWhom, int prefType, DateTime? prefDate)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            if (prefType < -1 || prefType > 1)
            {
                throw new ArgumentNullException("prefType");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);


                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od.DespatchDetail.DateRemovalRequested != null)
                        return "This job has been already marked for Removal";

                    else
                    {
                        //Update Request ByWhom and Preferred Datetime
                        if (string.IsNullOrEmpty(od.RemovalNotes))
                            od.RemovalNotes = rNotes;
                        else
                            od.RemovalNotes = od.RemovalNotes + ". " + rNotes;
                        od.DespatchDetail.RBy = byWhom.Length > 50 ? byWhom.Substring(0, 48) : byWhom;
                        od.DespatchDetail.PreferredRemovalDate = prefDate;
                        od.DespatchDetail.PreferredRemovalType = prefType;
                        
                        ctx.SubmitChanges();

                        ctx.ABCWRKFLOW_BoardsRemReqSet(od.OrderID, DateTime.Now, true);

                        string prefRemovalDate = string.Empty;
                        if (prefDate != null)
                        {
                            if (prefType == -1)
                                prefRemovalDate = "Before or On " + prefDate.Value.ToString("dd/MM/yyyy");
                            else if (prefType == 0)
                                prefRemovalDate = "On " + prefDate.Value.ToString("dd/MM/yyyy");
                            else if (prefType == 1)
                                prefRemovalDate = "On or After" + prefDate.Value.ToString("dd/MM/yyyy");
                        }

                        ctx.SubmitChanges();
                        return "Success";
                    }

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RequestBoardRemoval'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Updates the content of the XML file with the information from the document model
        /// so we can generate online listing.
        /// </summary>
        private void UpdateXMLOnApproval(int orderID)
        {
            // Get design completed documents
            AOPService aopService = new AOPService();
            List<EntityRelations> enRelations = new List<EntityRelations>();
            enRelations.Add(EntityRelations.AOP_JobDocument_To_AOP_TemplateProduct);

            List<AOP_JobDocument> jobDocs = aopService.GetJobDocumentsByDesignStatus(orderID, 1, enRelations);

            if (jobDocs == null)
                return;
            if (jobDocs.Count < 1)
                return;

            UpdateXMLForDocuments(jobDocs, true);
        }

        private bool MoveDocumentsAndPreviewFiles(int orderID)
        {
            try
            {
                if (orderID < 1)
                    return false;
                // Get design completed documents
                AOPService aopService = new AOPService();

                List<AOP_JobDocument> jobDocs = aopService.GetJobDocumentsByDesignStatus(orderID, 1, null);

                if (jobDocs != null && jobDocs.Count > 0)
                    MoveFiles(orderID, jobDocs);

                return true;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'MoveDocumentsAndPreviewFiles'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                return false;
            }
        }

        private bool MoveDIYDocumentsAndPreviewFiles(int orderID, List<int> jobDocIds)
        {
            try
            {
                if (orderID < 1)
                    return false;
                // Get design completed documents
                AOPService aopService = new AOPService();

                List<AOP_JobDocument> jobDocs = aopService.GetJobDocumentsByDesignStatus(orderID, 1, null);

                jobDocs = jobDocs.FindAll(jd => jobDocIds.Contains(jd.JobDocumentId)).ToList();

                if (jobDocs != null && jobDocs.Count > 0)
                    MoveFiles(orderID, jobDocs);

                return true;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'MoveDocumentsAndPreviewFiles'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                return false;
            }
        }

        private void MoveFiles(int orderID, List<AOP_JobDocument> jobDocs)
        {
            if (orderID < 1)
                return;
            if (jobDocs != null && jobDocs.Count > 0)
            {
                IFile file = VirtualFileSystemFactory.GetFile();

                string path = GetDocumentOutputDir(orderID);

                if (string.IsNullOrEmpty(path))
                    throw new Exception("AOP_DOC_OUTPUT_DIR_STRUCTURE is not correct");

                if (!file.ExistsDir(path))
                    throw new Exception("AOP CompletedJobs path doesn't exist:" + path);

                foreach (AOP_JobDocument jobDoc in jobDocs)
                {
                    string inddPattern = string.Format("{0}_{1}_{2}*.indd", orderID, jobDoc.JobDocumentId, Path.GetFileNameWithoutExtension(jobDoc.TemplateOriginalFileName));
                    string[] artworkFiles = file.GetFiles(path, inddPattern);

                    // Moving Indd documents
                    if (artworkFiles != null && artworkFiles.Length > 0)
                    {
                        MoveFiles(orderID, artworkFiles, ServiceConfig.APPROVED_DOC_DIR, ServiceConfig.APPROVED_DOC_DIR_STRUCTURE, "APPROVED_DOC_DIR_STRUCTURE");
                    }
                    else
                    {
                        Logger.Warn("MoveFiles::Directory.GetFiles returned zero documents for {0} in path {1}", inddPattern, path);
                    }
                }

                foreach (AOP_JobDocument jobDoc in jobDocs)
                {
                    string jpgPattern = string.Format("{0}_{1}_{2}*.jpg", orderID, jobDoc.JobDocumentId, Path.GetFileNameWithoutExtension(jobDoc.TemplateOriginalFileName));
                    string[] previewFiles = file.GetFiles(path, jpgPattern);

                    // Moving preview images
                    if (previewFiles != null && previewFiles.Length > 0)
                    {
                        MoveFiles(orderID, previewFiles, ServiceConfig.PROOF_DIR, ServiceConfig.PROOF_DIR_STRUCTURE, "PROOF_DIR_STRUCTURE");
                    }
                }

                // After moving the files, delete the folder.
                try
                {
                    if (file.ExistsDir(path))
                    {

                        //file.DeleteDir(path, true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, path);
                }
            }
        }

        private string GetDocumentOutputDir(int orderID)
        {
            string path = string.Empty;
            if (ServiceConfig.AOP_DOC_OUTPUT_DIR_STRUCTURE.ToLower() == "hundred")
                path = DirectoryPath.GetOutputFolder(orderID, ServiceConfig.AOP_DOC_OUTPUT_DIR, Abc.OnlinePublication.Common.FolderStructureType.Hundred);
            else if (ServiceConfig.AOP_DOC_OUTPUT_DIR_STRUCTURE.ToLower() == "flat")
                path = DirectoryPath.GetOutputFolder(orderID, ServiceConfig.AOP_DOC_OUTPUT_DIR, Abc.OnlinePublication.Common.FolderStructureType.Flat);
            else if (ServiceConfig.AOP_DOC_OUTPUT_DIR_STRUCTURE.ToLower() == "jobid")
                path = DirectoryPath.GetOutputFolder(orderID, ServiceConfig.AOP_DOC_OUTPUT_DIR, Abc.OnlinePublication.Common.FolderStructureType.JobId);

            return path;
        }

        private void MoveFiles(int orderID, string[] files, string rootPath, string structure, string pathStructureConfigName)
        {
            if (orderID < 1)
                return;

            IFile vfile = VirtualFileSystemFactory.GetFile();

            if (files.Length > 0)
            {
                string destPath = null;
                // This is the go arround way.
                if (structure.ToLower() == "flat")
                {
                    destPath = DirectoryPath.GetOutputFolder(orderID, rootPath, Abc.OnlinePublication.Common.FolderStructureType.Flat);
                }
                else if (structure.ToLower() == "hundred")
                {
                    destPath = DirectoryPath.GetOutputFolder(orderID, rootPath, Abc.OnlinePublication.Common.FolderStructureType.Hundred);
                }

                if (destPath == null)
                    throw new Exception(string.Format("{0} is not correct", pathStructureConfigName));

                try
                {
                    if (!vfile.ExistsDir(destPath))
                        vfile.CreateDir(destPath);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, destPath);
                }

                foreach (string file in files)
                {
                    string destFile = Path.Combine(destPath, Path.GetFileName(file));
                    try
                    {
                        if (vfile.Exists(destFile))
                            vfile.Delete(destFile);
                        vfile.Copy(file, destFile, true);
                        //vfile.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, file, destFile);
                    }
                }
            }
        }

        private void UpdateXMLForDocuments(List<AOP_JobDocument> jobDocs, bool approvalOperation)
        {
            XmlOrder.Config.ImageFolderStructure = ServiceConfig.AOP_IMAGE_OUTPUT_DIR_STRUCTURE;
            XmlOrder.Config.ImageRootFolder = ServiceConfig.AOP_IMAGE_OUTPUT_DIR;
            XmlOrder.Config.XPathMappingFile = ServiceConfig.XML_XPATH_MAPPING_FILE;

            int jobId = -1;
            List<XmlOrder.DocumentWrapper> documentWrappers = new List<XmlOrder.DocumentWrapper>();
            foreach (AOP_JobDocument jobDoc in jobDocs)
            {
                if (jobId == -1)
                    jobId = jobDoc.JobId.Value;

                Abc.OnlinePublication.Common.DOM.Document doc = null;
                if (approvalOperation)
                {
                    doc = Dom.Document.Deserialize(jobDoc.WorkingDocumentModel.ToString());
                }
                else
                {
                    if (jobDoc.TempDocumentModel != null && !string.IsNullOrEmpty(jobDoc.TempDocumentModel.ToString()))
                        doc = Dom.Document.Deserialize(jobDoc.TempDocumentModel.ToString());
                    else
                        doc = Dom.Document.Deserialize(jobDoc.WorkingDocumentModel.ToString());
                }

                if (doc == null)
                    continue;

                XmlOrder.DocumentWrapper documentWrapper = new XmlOrder.DocumentWrapper();
                documentWrapper.Document = doc;

                if (jobDoc.AOP_TemplateProduct != null)
                {
                    documentWrapper.IsBoard = (jobDoc.AOP_TemplateProduct.Type == "Billboard");
                }
                documentWrapper.JobDocumentId = jobDoc.JobDocumentId;

                documentWrappers.Add(documentWrapper);
            }

            if (documentWrappers.Count > 0)
            {

                string xmlFile = GetXmlFile(jobId);
                XmlOrder.XmlContentUpdater updater = new XmlOrder.XmlContentUpdater(jobId, documentWrappers, xmlFile);
                updater.Update();
            }
        }

        private string GetXmlFile(int jobId)
        {
            string path;
            if (ServiceConfig.XML_ORDER_DIR_IS_STRUCTURED)
                path = DirectoryPath.GetOutputFolder(jobId, ServiceConfig.XML_ORDER_DIR, Abc.OnlinePublication.Common.FolderStructureType.Hundred);
            else
                path = DirectoryPath.GetOutputFolder(jobId, ServiceConfig.XML_ORDER_DIR, Abc.OnlinePublication.Common.FolderStructureType.Flat);

            return Path.Combine(path, string.Format("{0}.xml", jobId));
        }

        private void UpdateXMLOnCancelation(int orderID)
        {
            // To copy images over we need to issue a SaveAsInDesign request just like
            // the client click OK in AOP. But before doing that we only need to make request
            // when client has done something which means a job document has been in the job queue b4.
            AOPService aopService = new AOPService();
            List<EntityRelations> enRelations = new List<EntityRelations>();
            enRelations.Add(EntityRelations.AOP_JobDocument_To_AOP_TemplateProduct);

            List<AOP_JobDocument> allJobDocs = aopService.GetJobDocumentsByJobId(orderID, enRelations);

            if (allJobDocs == null || allJobDocs.Count < 1)
                return;

            List<int> jobDocIds = aopService.GetJobDocumentIdsInQueueByJobId(orderID);

            List<AOP_JobDocument> jobDocsToUpdate = new List<AOP_JobDocument>();
            foreach (AOP_JobDocument jobDoc in allJobDocs)
            {
                if (jobDocIds.Contains(jobDoc.JobDocumentId))
                    jobDocsToUpdate.Add(jobDoc);
            }

            foreach (AOP_JobDocument jobDoc in jobDocsToUpdate)
            {
                AOP_JobQueue queue = new AOP_JobQueue();
                queue.SetAsChangeTrackingRoot(EntityState.New);
                queue.JobDocumentId = jobDoc.JobDocumentId;
                queue.StatusId = 0;
                queue.Action = "SaveAsIndesignToAltFolder";
                queue.DateCreated = DateTime.Now;
                // Setting EntityState.New alone might be enough to insert on submit.
                queue.SetAsInsertOnSubmit();
                aopService.UpdateJobQueue(queue);
            }

            if (jobDocsToUpdate.Count > 0 && !ServiceConfig.IS_NZ)
                UpdateXMLForDocuments(jobDocsToUpdate, false);
        }

        private string GetCaption(Abc.OnlinePublication.Common.DOM.Tag captionTag)
        {
            var caption = captionTag.FormElement.Value;
            if (captionTag.FormElement.Value.Length > 20)
            {
                caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
            }
            return caption;
        }

        private string GetCopyCaption(Abc.OnlinePublication.Common.DOM.Tag captionTag)
        {
            var caption = captionTag.FormElement.Value;
            if (captionTag.FormElement.Value.Length > 80)
            {
                caption = captionTag.FormElement.Value.Substring(0, 80) + "...";
            }
            return caption;
        }

        private string GetNamePlateAndUnitStickerCaption(Order od)
        {
            var sbContact = new StringBuilder();
            var sbUnitStick = new StringBuilder();
            var sbBulletPoints = new StringBuilder();
            var sbIcons = new StringBuilder();
            var sbOverlayInfo = new StringBuilder();
            var sbAuctionDetail = new StringBuilder();
            var sbFixedDateSaleDetails = new StringBuilder();
            var sbForthcomingAuction = new StringBuilder();
            var sbPropertyAddress = new StringBuilder();
            var sbSMSDetails = new StringBuilder();

            foreach (AOP_JobDocument aopJob in od.AOP_JobDocuments.Where(aop => aop.TemplateOriginalFileName.Contains("_OL.indt")))
            {
                if (aopJob.TempDocumentModel == null) continue;

                Abc.OnlinePublication.Common.DOM.Document doc = Dom.Document.Deserialize(aopJob.TempDocumentModel.ToString());

                var orderDetail = od.OrderDetails.Where(x => x.OrderDetailsID == aopJob.OrderDetailsID).FirstOrDefault();
                if (orderDetail == null)
                    continue;

                if (ServiceConfig.NAMEPLATES_PRODUCT_IDS.Contains(orderDetail.ProductID.ToString()))
                {
                    var visibleLayers = doc.Layers.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Layer t) { return t.Visible == true; });

                    var visibleLayerIds = visibleLayers.Select(x => x.Id);

                    var contactGroups = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t)
                    {
                        return t.TagName == "ContactsGroup" && visibleLayerIds.Contains(t.ItemLayerId);
                    });

                    foreach (var contactGroup in contactGroups)
                    {
                        var captionTag = contactGroup.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("ContactFirstName"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbContact.AppendFormat("{0}", GetCaption(captionTag));
                        }
                        captionTag = contactGroup.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("ContactLastName"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbContact.AppendFormat(" {0}", GetCaption(captionTag));
                        }

                        captionTag = contactGroup.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("ContactMobile"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbContact.AppendFormat(" {0}", GetCaption(captionTag));
                        }
                        sbContact.Append(" -- ");
                    }
                    continue;
                }

                if (ServiceConfig.UNITSTICKER_PRODUCT_IDS.Contains(orderDetail.ProductID.ToString()))
                {
                    var unitStickers = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "UnitNo"; });

                    foreach (var unit in unitStickers)
                    {
                        var captionTag = unit.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("UnitNo"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbUnitStick.AppendFormat("{0}", GetCaption(captionTag));
                        }
                        sbUnitStick.Append(" -- ");
                    }
                }

                if (ServiceConfig.OVERLAY_POINTS_PRODUCT_IDS.Contains(orderDetail.ProductID.ToString()))
                {
                    var points = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "OverlayBulletPoints"; });

                    foreach (var point in points)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("OverlayBulletPoints"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbBulletPoints.AppendFormat("{0}", GetCopyCaption(captionTag));
                        }
                        sbBulletPoints.Append(" -- ");
                    }
                }

                if (ServiceConfig.OVERLAY_THREE_ICONS_PRODUCT_IDS.Contains(orderDetail.ProductID.ToString()))
                {
                    var bedPoints = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "IconBed"; });

                    foreach (var point in bedPoints)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("IconBed"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbIcons.AppendFormat("Bed {0}", GetCaption(captionTag));
                        }
                        sbIcons.Append(", ");
                    }

                    var bathPoints = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "IconBath"; });

                    foreach (var point in bathPoints)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("IconBath"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbIcons.AppendFormat("Bath {0}", GetCaption(captionTag));
                        }
                        sbIcons.Append(", ");
                    }

                    var carPoints = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "IconCar"; });

                    foreach (var point in carPoints)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("IconCar"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbIcons.AppendFormat("Car {0}", GetCaption(captionTag));
                        }
                        sbIcons.Append(", ");
                    }

                    var studyPoints = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "IconStudy"; });

                    foreach (var point in studyPoints)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("IconStudy"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbIcons.AppendFormat("Study {0}", GetCaption(captionTag));
                        }
                        sbIcons.Append(", ");
                    }
                }

                if (ServiceConfig.OVERLAY_INFO_PRODUCT_IDS.Contains(orderDetail.ProductID.ToString()))
                {
                    var overlayInfos = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return (t.TagName == "OverlayInfo" && t.FormElement != null && t.FormElement.Visible == true); });

                    foreach (var point in overlayInfos)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("OverlayInfo"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbOverlayInfo.AppendFormat("{0}", GetCopyCaption(captionTag));
                        }
                        sbOverlayInfo.Append(" -- ");
                    }

                    var auctionDetails = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return (t.TagName == "AuctionDetails" && t.FormElement != null && t.FormElement.Visible == true); });
                    
                    foreach (var point in auctionDetails)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("AuctionDetails"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbAuctionDetail.AppendFormat("Auction: {0}", GetCopyCaption(captionTag));
                        }
                        sbAuctionDetail.Append(" -- ");
                    }

                    var fixedDateSaleDetails = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return (t.TagName == "FixedDateSaleDetails" && t.FormElement != null && t.FormElement.Visible == true); });

                    foreach (var point in fixedDateSaleDetails)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("FixedDateSaleDetails"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbFixedDateSaleDetails.AppendFormat("Fixed Date Sale: {0}", GetCopyCaption(captionTag));
                        }
                        sbFixedDateSaleDetails.Append(" -- ");
                    }

                    var forthcomingAuction = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return (t.TagName == "ForthcomingAuction" && t.FormElement != null && t.FormElement.Visible == true); });

                    foreach (var point in forthcomingAuction)
                    {
                        var captionTag = point.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("ForthcomingAuction"); });
                        if (captionTag != null && captionTag.FormElement != null)
                        {
                            sbForthcomingAuction.AppendFormat("{0}", GetCopyCaption(captionTag));
                        }
                        sbForthcomingAuction.Append(" -- ");
                    }
                }

                if (ServiceConfig.OVERLAY_PROPERTY_ADDRESS_IDS.Contains(orderDetail.ProductID.ToString()))
                {
                    var propertyAddresses = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "SBPropertyStreet"; });

                    foreach (var pAddress in propertyAddresses)
                    {
                        var captionTag = pAddress.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("SBPropertyStreet"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbPropertyAddress.AppendFormat("{0}", GetCaption(captionTag));
                        }
                        sbPropertyAddress.Append(" ");
                    }

                    var suburbs = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "SBPropertySuburb"; });

                    foreach (var suburb in suburbs)
                    {
                        var captionTag = suburb.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("SBPropertySuburb"); });
                        if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                        {
                            sbPropertyAddress.AppendFormat(" {0}", GetCaption(captionTag));
                        }
                        sbPropertyAddress.Append(" ");
                    }
                }

                var sMSDetails = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return (t.TagName == "SMSDetails" && t.FormElement != null && t.FormElement.Visible == true); });

                foreach (var sMSDetail in sMSDetails)
                {
                    var captionTag = sMSDetail.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("SMSDetails"); });
                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                    {
                        sbSMSDetails.AppendFormat("SMSDetails: {0}", GetCaption(captionTag));
                    }
                    sbSMSDetails.Append(" -- ");
                }

                var sMSMobiles = doc.Root.FindAll(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return (t.TagName == "SMSMobile" && t.FormElement != null && t.FormElement.Visible == true); });

                foreach (var sMSMobile in sMSMobiles)
                {
                    var captionTag = sMSMobile.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName.Contains("SMSMobile"); });
                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                    {
                        sbSMSDetails.AppendFormat("SMSMobile: {0}", GetCaption(captionTag));
                    }
                    sbSMSDetails.Append(" -- ");
                }
            }

            StringBuilder result = new StringBuilder();

            var contactCaption = sbContact.ToString();
            if (!string.IsNullOrEmpty(contactCaption))
            {
                if (contactCaption.EndsWith(" -- "))
                    contactCaption = contactCaption.Substring(0, contactCaption.Length - 4);

                result.AppendFormat("{0} - ", contactCaption);
            }

            var unitStickCaption = sbUnitStick.ToString();
            if (!string.IsNullOrEmpty(unitStickCaption))
            {
                if (unitStickCaption.EndsWith(" -- "))
                    unitStickCaption = unitStickCaption.Substring(0, unitStickCaption.Length - 4);

                result.AppendFormat("{0} - ", unitStickCaption);
            }

            var bulletPointsCaption = sbBulletPoints.ToString();
            if (!string.IsNullOrEmpty(bulletPointsCaption))
            {
                if (bulletPointsCaption.EndsWith(" -- "))
                    bulletPointsCaption = bulletPointsCaption.Substring(0, bulletPointsCaption.Length - 4);

                result.AppendFormat("{0} - ", bulletPointsCaption);
            }

            var iconsCaption = sbIcons.ToString();
            if (!string.IsNullOrEmpty(iconsCaption))
            {
                if (iconsCaption.EndsWith(", "))
                    iconsCaption = iconsCaption.Substring(0, iconsCaption.Length - 2);

                result.AppendFormat("{0} - ", iconsCaption);
            }

            var overlayInfoCaption = sbOverlayInfo.ToString();
            if (!string.IsNullOrEmpty(overlayInfoCaption))
            {
                if (overlayInfoCaption.EndsWith(" -- "))
                    overlayInfoCaption = overlayInfoCaption.Substring(0, overlayInfoCaption.Length - 4);

                result.AppendFormat("{0} - ", overlayInfoCaption);
            }

            var auctionDetailCaption = sbAuctionDetail.ToString();
            if (!string.IsNullOrEmpty(auctionDetailCaption))
            {
                if (auctionDetailCaption.EndsWith(" -- "))
                    auctionDetailCaption = auctionDetailCaption.Substring(0, auctionDetailCaption.Length - 4);

                result.AppendFormat("{0} - ", auctionDetailCaption);
            }

            var fixedDateSaleDetailsCaption = sbFixedDateSaleDetails.ToString();
            if (!string.IsNullOrEmpty(fixedDateSaleDetailsCaption))
            {
                if (fixedDateSaleDetailsCaption.EndsWith(" -- "))
                    fixedDateSaleDetailsCaption = fixedDateSaleDetailsCaption.Substring(0, fixedDateSaleDetailsCaption.Length - 4);

                result.AppendFormat("{0} - ", fixedDateSaleDetailsCaption);
            }

            var forthcomingAuctionCaption = sbForthcomingAuction.ToString();
            if (!string.IsNullOrEmpty(forthcomingAuctionCaption))
            {
                if (forthcomingAuctionCaption.EndsWith(" -- "))
                    forthcomingAuctionCaption = forthcomingAuctionCaption.Substring(0, forthcomingAuctionCaption.Length - 4);

                result.AppendFormat("{0} - ", forthcomingAuctionCaption);
            }

            var propertyAddressCaption = sbPropertyAddress.ToString();
            if (!string.IsNullOrEmpty(propertyAddressCaption))
            {
                result.AppendFormat("{0} - ", propertyAddressCaption);
            }

            var smsDetailCaption = sbSMSDetails.ToString();
            if (!string.IsNullOrEmpty(smsDetailCaption))
            {
                if (smsDetailCaption.EndsWith(" -- "))
                    smsDetailCaption = smsDetailCaption.Substring(0, smsDetailCaption.Length - 4);

                result.AppendFormat("{0} - ", smsDetailCaption);
            }

            string ret = result.ToString();
            if (ret.EndsWith(" - "))
                ret = ret.Substring(0, ret.Length - 3);

            Logger.Warn(od.OrderID.ToString() + ": " + ret);

            
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            ret = ret.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);

            return ret;
        }

        private string GetBrochureCaption(Order od)
        {
            var brochureJobDoc = od.AOP_JobDocuments
                            .Where(x => x.OrderDetail.Product.CategoryId == CategoryTypes.Brochure || x.OrderDetail.Product.CategoryId == CategoryTypes.WindowCard)
                            .OrderBy(x => x.OrderDetail.OrderDetailsID)
                            .FirstOrDefault();

            if (brochureJobDoc != null)
            {
                if (brochureJobDoc.TempDocumentModel != null)
                {
                    Abc.OnlinePublication.Common.DOM.Document doc = Dom.Document.Deserialize(brochureJobDoc.TempDocumentModel.ToString());

                    Abc.OnlinePublication.Common.DOM.Tag captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "Heading"; });
                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                    {
                        return GetCaption(captionTag);
                    }
                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BodyCopy"; });
                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                    {
                        return GetCaption(captionTag);
                    }
                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureHeading"; });
                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                    {
                        return GetCaption(captionTag);
                    }
                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureBodyCopy"; });
                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                    {
                        return GetCaption(captionTag);
                    }
                }
            }
            return string.Empty;
        }

        private string GetStockboardName(Order od)
        {
            var stockboardOD = od.OrderDetails
                            .Where(x => x.Product.CategoryId == CategoryTypes.Stockboard)
                            .FirstOrDefault();

            if (stockboardOD != null)
            {
                return stockboardOD.Product.Name;
            }
            return string.Empty;
        }
        #endregion

        #region ChangeRequest
        public void ChangeRequest(int orderID, string notes, string byWhom, bool reProof)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            if (string.IsNullOrEmpty(notes))
            {
                throw new ArgumentNullException("notes");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);


                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (reProof)
                    {
                        //ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.ChangeAndReproof;
                        string sub = "Abc Notification: Change and REPROOF Request (Job No " + od.OrderID + ")";

                        string pAddress = "";
                        if (!string.IsNullOrEmpty(od.PropertyAddress))
                        {
                            pAddress = od.PropertyAddress.Replace("&", "&amp;");
                        }

                        string xmlData = @"<EVENT>
									<OrderID>" + od.OrderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
                                    <PAddress>" + pAddress + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
									<Change><![CDATA[" + notes.Replace("&", "&amp;") + @"]]></Change>
									<ReqBy>" + byWhom + @"</ReqBy>
									<Actions>Make Changes and RE-PROOF</Actions>
									<ReceivedOn>" + DateTime.Today.ToString("dd/MM/yyyy") + @"</ReceivedOn>
									</EVENT>";

                        string textData = "Change and REPROOF Request (Job No " + od.OrderID + ")";
                        string source = "OnlineBL_OrderService_ChangeRequest";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);
                    }
                    else
                    {
                        //ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.ChangeAndApprove;
                        string sub = "Abc Notification: Change and APPROVE Request (Job No " + od.OrderID + ")";

                        string pAddress = "";
                        if (!string.IsNullOrEmpty(od.PropertyAddress))
                        {
                            pAddress = od.PropertyAddress.Replace("&", "&amp;");
                        }

                        string xmlData = @"<EVENT>
									<OrderID>" + od.OrderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + pAddress + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
									<Change><![CDATA[" + notes.Replace("&", "&amp;") + @"]]></Change>
									<ReqBy>" + byWhom + @"</ReqBy>
									<Actions>Make Changes and APPROVE Job</Actions>
									<ReceivedOn>" + DateTime.Today.ToString("dd/MM/yyyy") + @"</ReceivedOn>
									</EVENT>";

                        string textData = "Change and APPROVE Request (Job No " + od.OrderID + ")";
                        string source = "OnlineBL_OrderService_ChangeRequest";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);
                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ChangeRequest'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region UpdateSmsOrderDetails
        public void UpdateSmsOrderDetails(int orderID, string smsUserText, string smsAgentMobileNo, bool smsNotifyAgent, bool smsSendEmail, string smsAgentEmailAddress, bool mmsAllowed, bool smsActive)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    OrderOtherDetail od = ctx.OrderOtherDetails.SingleOrDefault(o => o.OrderId == orderID);

                    od.SMS_UserText = smsUserText;
                    od.SMS_AgentMobileNo = smsAgentMobileNo;
                    od.SMS_NotifyAgent = smsNotifyAgent;
                    od.SMS_SendEmail = smsSendEmail;
                    od.SMS_AgentEmailAddress = smsAgentEmailAddress;
                    od.MMS_Allowed = mmsAllowed;
                    od.SMS_Active = smsActive;

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateSmsOrderDetails'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetSMSQueueByOrderId
        public List<SMS_Queue> GetSMSQueueByOrderId(int orderID, List<EntityRelations> loadOptions)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    List<SMS_Queue> sq = ctx.SMS_Queues.Where(o => o.OrderId == orderID).ToList();
                    return sq;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetSMSQueueByOrderId'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region UpdateMMSPhoto
        public void UpdateMMSPhoto(int orderID, byte[] photoBytes)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (photoBytes == null)
            {
                throw new ArgumentNullException("photoBytes");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    OrderOtherDetail od = ctx.OrderOtherDetails.SingleOrDefault(o => o.OrderId == orderID);

                    od.MMS_Photo = photoBytes;

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateMMSPhoto'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetAllArPropertyType
        public List<AR_PropertyType> GetAllArPropertyType(List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    return ctx.AR_PropertyTypes.OrderBy(a => a.PTypeName).ToList();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetAllArPropertyType'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetLocationIdByStateAndSuburb
        public Location GetLocationIdByStateAndSuburb(string state, string suburb, List<EntityRelations> loadOptions)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException("state");
            }
            if (string.IsNullOrEmpty(suburb))
            {
                throw new ArgumentNullException("suburb");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Location lo = ctx.Locations.SingleOrDefault(l => l.State.ToUpper() == state.Trim().ToUpper()
                                                                              && l.Location1.ToUpper() == suburb.Trim().ToUpper());
                    return lo;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetLocationIdByStateAndSuburb'. state:{0}, suburb{1}", state, suburb);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region InsertProperty
        public Property InsertProperty(Property property)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.Properties.InsertOnSubmit(property);

                    ctx.SubmitChanges();
                    return property;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'InsertProperty'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetPropertyById
        public Property GetPropertyById(int propertyID, List<EntityRelations> loadOptions)
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

                    Property pro = ctx.Properties.SingleOrDefault(p => p.PropertyId == propertyID);

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

        #region SaveOrderSession
        public void SaveOrderSession(int clientId, string data)
        {
            if (clientId <= 0)
            {
                throw new ArgumentNullException("clientId");
            }
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == clientId);

                    cl.OnlineOrderData = data;

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'SaveOrderSession'. clientId:{0}", clientId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetOrderFromSession
        public string GetOrderFromSession(int clientId)
        {
            if (clientId <= 0)
            {
                throw new ArgumentNullException("clientId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == clientId);

                    return cl.OnlineOrderData;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetOrderFromSession'. clientId:{0}", clientId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region SavePropertyOrderData
        public void SavePropertyOrderData(int propertyID, string data)
        {
            if (propertyID <= 0)
            {
                throw new ArgumentNullException("propertyID");
            }
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Property pr = ctx.Properties.SingleOrDefault(p => p.PropertyId == propertyID);

                    pr.OrderData = data;

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'SavePropertyOrderData'. propertyID:{0}", propertyID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetOnlineOrderCategory
        public List<OnlineOrderCategory> GetOnlineOrderCategory(int clientId, bool propertyRelatedOrder, bool regularOrder, List<EntityRelations> loadOptions)
        {
            if (clientId <= 0)
            {
                throw new ArgumentNullException("clientId");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    if (propertyRelatedOrder)
                    {
                        var ret = from oo in ctx.OnlineOrderCategories
                                  where oo.IncludeInPropertyOrder == propertyRelatedOrder
                                  orderby oo.SortOrder
                                  select oo;

                        return ret.ToList();
                    }
                    if (regularOrder)
                    {

                        var ret = from oo in ctx.OnlineOrderCategories
                                  where oo.IncludeInRegularOrder == regularOrder
                                  orderby oo.SortOrder
                                  select oo;

                        return ret.ToList();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetOnlineOrderCategory'. clientId:{0}", clientId);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region IOrderService Members
        public void UpdateOrderXMLFile(Abc.OnlineBL.Entities.Model.OrderFile.UploadText model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("UploadText");
            }
            if (model.orderID <= 0)
            {
                throw new ArgumentNullException("model.orderID");
            }

            #region Make the Caption 1st Letter captial and rest small
            string caption = model.Heading;
            if (!string.IsNullOrEmpty(caption))
                caption = caption.Substring(0, 1).ToUpper() + caption.Substring(1).ToLower();
            model.Heading = caption;
            #endregion

            try
            {
                //work with xml at the moment
                StreamWriter sText = null;
                StreamWriter sXml = null;

                string txtFilePath = string.Format(@"{0}\{1}\{2}.txt",
                                                            OnlineBLConfig.UPLOAD_TEXT_DETAILS_DIR.TrimEnd('\\'),
                                                            StructuredDirectoryName(model.orderID.ToString()),
                                                            model.orderID.ToString());

                string xmlFilePath = JobFilePath(model.orderID);

                #region Make sure the directories exists ...

                IFile file = VirtualFileSystemFactory.GetFile();

                if (!Directory.Exists(Path.GetDirectoryName(txtFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(txtFilePath));

                if (!file.ExistsDir(Path.GetDirectoryName(xmlFilePath)))
                    file.CreateDir(Path.GetDirectoryName(xmlFilePath));

                #endregion

                try
                {
                    string txtData = UploadTextToTextFile(model);
                    string xmlData = UploadTextToXml(model);

                    // Save Text file
                    sText = new StreamWriter(txtFilePath, false, System.Text.Encoding.Default);
                    sText.Write(txtData);

                    // Save xml file -- Old AU Code
                    //sXml = new StreamWriter(xmlFilePath, false);
                    //sXml.Write(xmlData);

                    file.WriteAllText(xmlFilePath, xmlData);

                    //update Material Details
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        MaterialDetail md = (from m in ctx.MaterialDetails
                                             where m.OrderID == model.orderID
                                             select m).FirstOrDefault();
                        if (md != null)
                        {
                            md.TextReceived = DateTime.Now;
                        }

                        List<EntityRelations> options = new List<EntityRelations>();
                        options.Add(EntityRelations.Order_To_Client);
                        options.Add(EntityRelations.Order_To_Location);

                        Order od = (from o in ctx.Orders
                                    where o.OrderID == model.orderID
                                    select o).FirstOrDefault();
                        if (od != null)
                        {
                            //ROnlineBLe an Event to send email notification to Admin
                            int eventID = EventSettings.TextUpload;
                            string sub = "Abc Notification: Text Uploaded for Order " + model.orderID;
                            string xmlEventData = @"<EVENT>
												<OrderID>" + model.orderID + @"</OrderID>
												<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
												<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
												<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
												<ReceivedOn>" + DateTime.Today.ToString("dd/MM/yyyy") + @"</ReceivedOn>
												</EVENT>";

                            string textData = "Text Uploaded for Order " + model.orderID;
                            string source = "OnlineBL_OrderService_UpdateOrderXMLFile";

                            ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, model.orderID, null, null, null, source, txtFilePath);
                            od.Caption = model.Heading;
                        }
                        ctx.SubmitChanges();
                    }
                }
                catch (System.IO.IOException ex)
                {
                    Logger.Exception(ex, model.orderID.ToString());
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    Logger.Exception(ex, model.orderID.ToString());
                    throw ex;
                }
                finally
                {
                    if (sText != null)
                        sText.Close();

                    if (sXml != null)
                        sXml.Close();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateOrderXMLFile'. orderID:{0}", model.orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        private string UploadTextToTextFile(Abc.OnlineBL.Entities.Model.OrderFile.UploadText model)
        {
            StringBuilder sb = new StringBuilder();

            #region Append Text ...

            AppendText(ref sb, "<Inspection Details>", model.InspectionDetails);
            AppendText(ref sb, "<Bath>", model.Bathrooms);
            AppendText(ref sb, "<Bed>", model.Bedrooms);
            AppendText(ref sb, "<Car>", model.CarportsOrGarages);
            AppendText(ref sb, "<Pool>", model.HasPool == true ? "1" : "0");
            AppendText(ref sb, "<Study>", model.Studyrooms);
            AppendText(ref sb, "<Toilet>", model.Toilet);
            AppendText(ref sb, "<Terms & Conditions>", model.TermAndConditions);
            AppendText(ref sb, "<Heading>", string.Format("{0}{1}", (!string.IsNullOrEmpty(model.Heading)) ? model.Heading.Trim('\n') : string.Empty, (!string.IsNullOrEmpty(model.BrochureHeading) && model.BrochureHeading.Trim().Length > 0) ? "\r\n***BROCHURE HEADING***\r\n" + model.BrochureHeading : string.Empty));
            AppendText(ref sb, "<Sub Heading>", string.Format("{0}{1}", (!string.IsNullOrEmpty(model.SubHeading)) ? model.SubHeading.Trim('\n') : string.Empty, (!string.IsNullOrEmpty(model.BrochureSubHeading) && model.BrochureSubHeading.Trim().Length > 0) ? "\r\n***BROCHURE SUB HEADING***\r\n" + model.BrochureSubHeading : string.Empty));
            AppendText(ref sb, "<Body Copy>", string.Format("{0}{1}", (!string.IsNullOrEmpty(model.BodyCopy)) ? model.BodyCopy.Trim('\n') : string.Empty, (!string.IsNullOrEmpty(model.BrochureBodyCopy) && model.BrochureBodyCopy.Trim().Length > 0) ? "\r\n***BROCHURE BODY COPY***\r\n" + model.BrochureBodyCopy : string.Empty));

            StringBuilder sbd = new StringBuilder();
            int firstIDTemp = 0, secondIDtemp = 0, thirdIDTemp = 0;
            if (model.FirstContactID != null && int.TryParse(model.FirstContactID, out firstIDTemp))
            {
                sbd.AppendFormat("{0}{1}", GetContactDetails(firstIDTemp), "\r\n");
            }
            if (model.SecondContactID != null && int.TryParse(model.SecondContactID, out secondIDtemp))
            {
                sbd.AppendFormat("{0}{1}", GetContactDetails(secondIDtemp), "\r\n");
            }
            if (model.ThirdContactID != null && int.TryParse(model.ThirdContactID, out thirdIDTemp))
            {
                sbd.AppendFormat("{0}{1}", GetContactDetails(thirdIDTemp), "\r\n");
            }
            if (!string.IsNullOrEmpty(sbd.ToString()))
                AppendText(ref sb, "<A/H Details>", sbd.ToString());

            using (AbcDataContext ctx = new AbcDataContext())
            {
                List<EntityRelations> options = new List<EntityRelations>();
                options.Add(EntityRelations.Order_To_Client);
                options.Add(EntityRelations.Order_To_Location);

                Order od = (from o in ctx.Orders
                            where o.OrderID == model.orderID
                            select o).FirstOrDefault();
                if (od != null)
                {
                    AppendText(ref sb, "<Agent Name>", od.Client.ClientName);
                    AppendText(ref sb, "<Agent Office>", od.Client.Office);
                    AppendText(ref sb, "<Office Address>", od.Client.Address);
                    AppendText(ref sb, "<Office Phone>", od.Client.Phone);
                }
            }

            AppendText(ref sb, "<Conjunctional Details>", model.ConjunctionDetails);

            #endregion
            return sb.ToString();
        }

        public string GetContactDetails(int contactID)
        {
            StringBuilder sb = new StringBuilder();
            using (AbcDataContext ctx = new AbcDataContext())
            {
                ClientContact cc = (from ccc in ctx.ClientContacts
                                    where ccc.ContactId == contactID
                                    select ccc).FirstOrDefault();
                if (cc != null)
                {
                    if (!OnlineBLConfig.IS_NZ)
                        return string.Format("{0} {1} {2}", cc.FirstName, cc.LastName, (!string.IsNullOrEmpty(cc.AhDetails)) ? cc.AhDetails :
                                                                                          (!string.IsNullOrEmpty(cc.Mobile)) ? cc.Mobile : cc.Phone);
                    else
                    {

                        sb.AppendFormat("{0} {1}", cc.FirstName, cc.LastName);

                        if (!string.IsNullOrEmpty(cc.Mobile))
                        {
                            if (string.IsNullOrEmpty(cc.AhDetails) && string.IsNullOrEmpty(cc.Phone))
                                sb.AppendFormat(" {0}", cc.Mobile);
                            else
                                sb.AppendFormat(" Mob: {0}", cc.Mobile);
                        }

                        if (!string.IsNullOrEmpty(cc.AhDetails))
                        {
                            if (string.IsNullOrEmpty(cc.Mobile) && string.IsNullOrEmpty(cc.Phone))
                                sb.AppendFormat(" {0}", cc.AhDetails);
                            else
                                sb.AppendFormat(" A/h: {0}", cc.AhDetails);
                        }

                        if (!string.IsNullOrEmpty(cc.Phone))
                        {
                            if (string.IsNullOrEmpty(cc.Mobile) && string.IsNullOrEmpty(cc.AhDetails))
                                sb.AppendFormat(" {0}", cc.Phone);
                            else
                                sb.AppendFormat(" Ph: {0}", cc.Phone);
                        }

                    }
                }
            }
            return sb.ToString();
        }

        private void AppendText(ref StringBuilder sb, string tag, string text)
        {
            if (!string.IsNullOrEmpty(text))
                sb.Append(tag + text.Trim('\n').Replace("\n", "\r\n") + "\r\n");
        }

        private string StructuredDirectoryName(string jobID)
        {
            return (jobID.ToCharArray().Reverse().ToArray())[2] + "00";
        }

        private string JobFilePath(int jobID)
        {
            if (ServiceConfig.XML_ORDER_DIR_IS_STRUCTURED)
                return string.Format(@"{0}\{1}\{2}.xml",
                                                 ServiceConfig.XML_ORDER_DIR.TrimEnd('\\'),
                                              StructuredDirectoryName(jobID.ToString()),
                                              jobID);
            else
                return string.Format(@"{0}\{1}.xml",
                                          ServiceConfig.XML_ORDER_DIR.TrimEnd('\\'),
                                            jobID);
        }


        private string UploadTextToXml(Abc.OnlineBL.Entities.Model.OrderFile.UploadText model)
        {
            XmlDocument xmlDoc = new XmlDocument();

            string FilePath = JobFilePath(model.orderID);

            IFile file = VirtualFileSystemFactory.GetFile();

            if (file.Exists(FilePath))
            {
                //xmlDoc.Load(FilePath); //old AU code
                xmlDoc.LoadXml(file.ReadAllText(FilePath));
                string logEntry = "Client Made Entry";

                if (logEntry != string.Empty)
                {
                    XmlNode logEntries = GetNode(ref xmlDoc, "OnlineOrder/LogHistory/LogEntries");

                    XmlNode logEntryNode = xmlDoc.CreateElement("LogEntry");
                    logEntryNode.InnerXml = logEntry;

                    logEntries.InsertBefore(logEntryNode, logEntries.FirstChild);
                }
            }

            #region Update Text details ...

            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/InspectionDetails").InnerText = model.InspectionDetails;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/Bath").InnerText = model.Bathrooms;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/Bed").InnerText = model.Bedrooms;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/Car").InnerText = model.CarportsOrGarages;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/Pool").InnerText = model.HasPool == true ? "1" : "0";
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/Study").InnerText = model.Studyrooms;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/Toilet").InnerText = model.Toilet;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/DisplayIcons").InnerText = model.IsDisplayIcon == true ? "Yes" : "No";
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/TermsConditions").InnerText = model.TermAndConditions;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/Heading").InnerText = model.Heading;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/BrochureHeading").InnerText = model.BrochureHeading;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/SubHeading").InnerText = model.SubHeading;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/BrochureSubHeading").InnerText = model.BrochureSubHeading;
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/BodyCopy").InnerXml = string.Format("<![CDATA[\r\n{0}\r\n]]>", model.BodyCopy);
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/BrochureBodyCopy").InnerXml = string.Format("<![CDATA[\r\n{0}\r\n]]>", model.BrochureBodyCopy);
            GetNode(ref xmlDoc, "OnlineOrder/TextDetails/ConjunctionalDetails").InnerText = model.ConjunctionDetails;

            StringBuilder sb = new StringBuilder();
            int i = 1;
            int firstIDTemp = 0, secondIDtemp = 0, thirdIDTemp = 0;
            if (model.FirstContactID != null && int.TryParse(model.FirstContactID, out firstIDTemp))
            {
                sb.Append("<Contact>");
                sb.AppendFormat("<Id>{0}</Id>", i);
                sb.AppendFormat("<ContactId>{0}</ContactId>", model.FirstContactID);
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ClientContact clientContact = (from cc in ctx.ClientContacts
                                                   where cc.ContactId == firstIDTemp
                                                   select cc).FirstOrDefault();
                    if (clientContact != null)
                    {
                        sb.AppendFormat("<FirstName>{0}</FirstName>", HttpUtility.HtmlEncode(clientContact.FirstName));
                        sb.AppendFormat("<LastName>{0}</LastName>", HttpUtility.HtmlEncode(clientContact.LastName));
                        sb.AppendFormat("<Phone>{0}</Phone>", HttpUtility.HtmlEncode(clientContact.Phone));
                        sb.AppendFormat("<Mobile>{0}</Mobile>", HttpUtility.HtmlEncode(clientContact.Mobile));
                        sb.AppendFormat("<Email>{0}</Email>", HttpUtility.HtmlEncode(clientContact.Email));
                    }
                }
                sb.Append("</Contact>");
            }
            if (model.SecondContactID != null && int.TryParse(model.SecondContactID, out secondIDtemp))
            {
                sb.Append("<Contact>");
                sb.AppendFormat("<Id>{0}</Id>", i + 1);
                sb.AppendFormat("<ContactId>{0}</ContactId>", model.SecondContactID);
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ClientContact clientContact = (from cc in ctx.ClientContacts
                                                   where cc.ContactId == secondIDtemp
                                                   select cc).FirstOrDefault();
                    if (clientContact != null)
                    {
                        sb.AppendFormat("<FirstName>{0}</FirstName>", HttpUtility.HtmlEncode(clientContact.FirstName));
                        sb.AppendFormat("<LastName>{0}</LastName>", HttpUtility.HtmlEncode(clientContact.LastName));
                        sb.AppendFormat("<Phone>{0}</Phone>", HttpUtility.HtmlEncode(clientContact.Phone));
                        sb.AppendFormat("<Mobile>{0}</Mobile>", HttpUtility.HtmlEncode(clientContact.Mobile));
                        sb.AppendFormat("<Email>{0}</Email>", HttpUtility.HtmlEncode(clientContact.Email));
                    }
                }
                sb.Append("</Contact>");
            }
            if (model.ThirdContactID != null && int.TryParse(model.ThirdContactID, out thirdIDTemp))
            {
                sb.Append("<Contact>");
                sb.AppendFormat("<Id>{0}</Id>", i + 2);
                sb.AppendFormat("<ContactId>{0}</ContactId>", model.ThirdContactID);
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ClientContact clientContact = (from cc in ctx.ClientContacts
                                                   where cc.ContactId == thirdIDTemp
                                                   select cc).FirstOrDefault();
                    if (clientContact != null)
                    {
                        sb.AppendFormat("<FirstName>{0}</FirstName>", HttpUtility.HtmlEncode(clientContact.FirstName));
                        sb.AppendFormat("<LastName>{0}</LastName>", HttpUtility.HtmlEncode(clientContact.LastName));
                        sb.AppendFormat("<Phone>{0}</Phone>", HttpUtility.HtmlEncode(clientContact.Phone));
                        sb.AppendFormat("<Mobile>{0}</Mobile>", HttpUtility.HtmlEncode(clientContact.Mobile));
                        sb.AppendFormat("<Email>{0}</Email>", HttpUtility.HtmlEncode(clientContact.Email));
                    }
                }
                sb.Append("</Contact>");
            }
            string xpath = "OnlineOrder/TextDetails/AgentContacts";
            GetNode(ref xmlDoc, xpath).InnerXml = sb.ToString();
            #endregion


            #region Append LogHistory ...

            // Incase of the OnlineOrder file being created
            if (!File.Exists(FilePath))
            {
                XmlNode logNode = xmlDoc.CreateElement("LogHistory");
                logNode.InnerXml = CreateLogCreation();

                xmlDoc.SelectSingleNode("OnlineOrder").InsertAfter(logNode,
                                                xmlDoc.SelectSingleNode("OnlineOrder").LastChild);
            }

            #endregion

            return xmlDoc.InnerXml;
        }

        private XmlNode GetNode(ref XmlDocument xmlDoc, string xPathExpr)
        {
            XmlNode node = xmlDoc.SelectSingleNode(xPathExpr);

            if (node == null)
            {
                string[] nodes = xPathExpr.Split('/');

                for (int i = 0; i <= nodes.Length - 1; i++)
                {
                    node = xmlDoc.SelectSingleNode(string.Join("/", nodes, 0, i + 1));

                    if (node == null)
                    {
                        node = xmlDoc.CreateElement(nodes[i]);
                        xmlDoc.SelectSingleNode(string.Join("/", nodes, 0, i)).AppendChild(node);
                    }
                }

                return node;
            }
            else
                return node;
        }

        private string CreateLogCreation()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<LogCreation>");

            sb.AppendFormat("<LogInfo appName=\"{0}\" computerName=\"{1}\" userName=\"{2}\" logDateTime=\"{3}\" />",
                                      "OnlineBL Service", Environment.MachineName,
                                      Environment.UserName, DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"));

            sb.Append("</LogCreation>");

            return sb.ToString();
        }
        #endregion

        public Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse RemoveNonPayingOrder(int clientID, int orderID)
        {
            Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ret = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse();
            try
            {
                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order order = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);
                    order.Notes = "ORDER CANCELLED";
                    order.OrderDetails.Clear();
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                ret.OrderHasError = true;
                Logger.Exception(ex, string.Format("An Error occured in 'RemoveNonPayingOrder'. ClientID: {0}\r\nOrderID: {1}", clientID, orderID));
            }
            ret.OrderId = orderID;

            return ret;
        }

        #region ProcessOrder
        public Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessOrder(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder)
        {
            int orderId = 0, photoOrderId = 0, stockId = 0, listingId = 0;
            Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ret = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse();

            if (propertyOrder == null)
            {
                throw new ArgumentNullException("propertyOrder");
            }

            if (string.IsNullOrEmpty(propertyOrder.ContactName) || string.IsNullOrEmpty(propertyOrder.ContactDetailName) || string.IsNullOrEmpty(propertyOrder.SendProofTo))
            {
                Logger.Warn("Error ContactDetailName: {0}, ClientID: {1}", propertyOrder.PropertyId, propertyOrder.ClientId);
                SendMail("notifications@photosigns.com.au", OnlineBLConfig.SEND_ERROR_MESSAGE_TO, null, "Error Processing Order", "Order Details:\r\n\r\n" + propertyOrder.GetHTMLString() + "\r\n");
                throw new ArgumentNullException("ContactDetailName");
            }

            if(propertyOrder.PropertyId > 0)
            {
                DateTime today = DateTime.Now;
                DateTime startDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 1);
                DateTime endDate = new DateTime(today.Year, today.Month, today.Day, 23, 23, 59);

                startDate = DateTime.Now.AddMinutes(-6);
                endDate = DateTime.Now.AddMinutes(6);

                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.PropertyOrder_To_Order);
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    PropertyOrder proOrder = ctx.PropertyOrders.Where(p => p.PropertyId == propertyOrder.PropertyId).OrderByDescending(p => p.OrderId).FirstOrDefault();

                    if (proOrder != null && proOrder.Order != null && proOrder.Order.DateReceived >= startDate && proOrder.Order.CreatedOn <= endDate)
                    {
                        //Logger.Warn(startDate.ToString() + " - " + endDate.ToString());
                        Logger.Warn("Order double up Error: {0}, ClientID: {1}", propertyOrder.PropertyId, propertyOrder.ClientId);
                        throw new ArgumentNullException("Order double up Error");

                    }
                }
            }

            if (propertyOrder.ClientId != 22812 && propertyOrder.OrderHasCommunityBoard() && !propertyOrder.OrderHasCommunityBoardOnly())
            {
                Logger.Warn("Error: {0}, ClientID: {1}", propertyOrder.PropertyId, propertyOrder.ClientId);
                SendMail("webdesign@photosigns.com.au", OnlineBLConfig.SEND_ERROR_MESSAGE_TO, null, "Error Processing Order - Community Board", "PropertyID:\r\n" + propertyOrder.PropertyId + "\r\n");
                throw new ArgumentNullException("OrderHasCommunityBoardOnly");
            }                                                           

            //if (propertyOrder.OrderHasCommunityBoard())
            //{
            //    bool allowToOrderCommunityBoardOnline = false;
            //    using (AbcDataContext ctx = new AbcDataContext())
            //    {
            //        ClientsPref cpAllowCommunityBoard = (from c in ctx.ClientsPrefs
            //                                            where c.ClientId == propertyOrder.ClientId && c.PrefID == ClientsPref.AllowToOrderCommunityBoardOnline
            //                                             select c).FirstOrDefault();

            //        if (cpAllowCommunityBoard != null && cpAllowCommunityBoard.BitValue.HasValue && cpAllowCommunityBoard.BitValue == true)
            //            allowToOrderCommunityBoardOnline = true;
            //    }

            //    if(!allowToOrderCommunityBoardOnline)
            //    {
            //        Logger.Warn("Error: {0}, ClientID: {1}", propertyOrder.PropertyId, propertyOrder.ClientId);
            //        SendMail("webdesign@photosigns.com.au", OnlineBLConfig.SEND_ERROR_MESSAGE_TO, null, "Error Processing Order - Community Board", "PropertyID:\r\n" + propertyOrder.PropertyId + "\r\n");
            //        throw new ArgumentNullException("OrderHasCommunityBoard");
            //    }
            //}

            if (propertyOrder.IsArtworkUpload)
            {
                propertyOrder.IsDIYOrder = false;
                propertyOrder.OrderType = OrderType.ProvideArtwork;
            }

            if (propertyOrder.PropertyId > 0)
            {
                DateTime today = DateTime.Now;
                DateTime startDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 1);
                DateTime endDate = new DateTime(today.Year, today.Month, today.Day, 23, 23, 59);

                startDate = DateTime.Now.AddMinutes(-6);
                endDate = DateTime.Now.AddMinutes(6);

                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.PropertyOrder_To_Order);
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    PropertyOrder proOrder = ctx.PropertyOrders.Where(p => p.PropertyId == propertyOrder.PropertyId).OrderByDescending(p => p.OrderId).FirstOrDefault();

                    if (proOrder != null && proOrder.Order != null && proOrder.Order.DateReceived >= startDate && proOrder.Order.CreatedOn <= endDate)
                    {
                        Logger.Warn(startDate.ToString() + " - " + endDate.ToString());
                        Logger.Warn("Order double up Error: {0}, ClientID: {1}", propertyOrder.PropertyId, propertyOrder.ClientId);
                        throw new ArgumentNullException("Order double up Error");

                    }
                }
            }

            try
            {
                WFRuntimeHelper runtime = new WFRuntimeHelper(propertyOrder);
                Abc.OnlineBL.Orders.Workflow.OrderDataExchange orderData = runtime.ExecuteWorkflow();

                listingId = orderData.ListingId;
                photoOrderId = orderData.PhotoOrderId;
                stockId = orderData.StockId;
                orderId = orderData.OrderId;
            }
            catch (Exception ex)
            {
                ret.OrderHasError = true;
                Logger.Exception(ex, string.Format("ClientID: {0}\r\nPropertyID: {1}\r\n{2}", propertyOrder.ClientId, propertyOrder.PropertyId, propertyOrder.GetHTMLString()));
                SendMail("notifications@photosigns.com.au", OnlineBLConfig.SEND_ERROR_MESSAGE_TO, null, "Error Processing Order", "Order Details:\r\n\r\n" + propertyOrder.GetHTMLString() + "XML:\r\n" + propertyOrder.GetXml() + "\r\n");
            }

            ret.PropertyId = propertyOrder.PropertyId;
            ret.OrderId = orderId;
            ret.PhotoOrderId = photoOrderId;
            ret.StockId = stockId;
            ret.ListingId = listingId;

            return ret;

        }

        public Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessOnlinePaymentExpressOrder(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, int TemporderdetailsId, int ClientId, int AccountId, string PaymentResponse, string CardType)
        {
            int orderId = 0, photoOrderId = 0, stockId = 0, listingId = 0, PropertyId = 0;
            Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ret = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse();

            using (AbcDataContext ctx = new AbcDataContext())
            {
                try
                {
                    ctx.Connection.Open();
                    ctx.Transaction = ctx.Connection.BeginTransaction();
                    ctx.DeferredLoadingEnabled = false;
                    OnlinePaymentExpressOrder Pgorder = ctx.OnlinePaymentExpressOrders.SingleOrDefault(o => o.Id == TemporderdetailsId);
                    //Update PayGPaymentDetails  and process Order
                    if (Pgorder != null)
                    {

                        //Process Order (New order will be generated)
                        if (!string.IsNullOrEmpty(Pgorder.OrderData))
                        {
                            // OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder = ObjectUtility.Deserialize<OnlinePropertyOrder>(Pgorder.OrderData);
                            if (propertyOrder == null)
                            {
                                throw new ArgumentNullException("propertyOrder");
                            }

                            if (propertyOrder.IsArtworkUpload)
                            {
                                propertyOrder.IsDIYOrder = false;
                                propertyOrder.OrderType = OrderType.ProvideArtwork;
                            }

                            try
                            {
                                WFRuntimeHelper runtime = new WFRuntimeHelper(propertyOrder);
                                Abc.OnlineBL.Orders.Workflow.OrderDataExchange orderData = runtime.ExecuteWorkflow();
                                listingId = orderData.ListingId;
                                photoOrderId = orderData.PhotoOrderId;
                                stockId = orderData.StockId;
                                orderId = orderData.OrderId;
                                PropertyId = propertyOrder.PropertyId;
                                //If Order is generated successully add data in Online payment and Payment Details table
                                if (orderId > 0 || stockId > 0 || photoOrderId > 0)
                                {
                                    Pgorder.IsPaymentDone = true;
                                    Pgorder.PaymentResponse = PaymentResponse;
                                    Pgorder.CardType = CardType;
                                    if(orderId > 0)
                                    Pgorder.OrderId = orderId;
                                    if (stockId > 0)
                                        Pgorder.StockboardOrderId = stockId;
                                    if (photoOrderId > 0)
                                        Pgorder.PhotoOrderId = photoOrderId;
                                    ctx.SubmitChanges();

                                    //Insert in Parents Table (OnlinePayment,payments)
                                    OnlinePayment onlinePaymentObj = new OnlinePayment();
                                    onlinePaymentObj.ClientId = ClientId;
                                    onlinePaymentObj.AccountId = AccountId;
                                    onlinePaymentObj.CardType = Pgorder.CardType;
                                    onlinePaymentObj.Surcharge = Convert.ToDecimal(Pgorder.Surcharge);
                                    onlinePaymentObj.Total = Convert.ToDecimal(Pgorder.PaymentAmount);
                                    onlinePaymentObj.CreateOn = DateTime.Now;
                                    onlinePaymentObj.TransactionComplete = false;
                                    onlinePaymentObj.CBATransactionRef = string.Empty;
                                    onlinePaymentObj.PaymentId = 0;

                                    ctx.OnlinePayments.InsertOnSubmit(onlinePaymentObj);
                                    ctx.SubmitChanges();

                                    int onlinePaymentId = onlinePaymentObj.OnlinePaymentId;

                                    if (onlinePaymentId <= 0)
                                    {
                                        return null;
                                    }

                                    Payment payment = new Payment();
                                    payment.AccountID = AccountId;
                                    payment.PaymentDate = DateTime.Now;
                                    payment.PayedBy = "Online";
                                    payment.Notes = PaymentResponse;
                                    payment.Amount = Convert.ToDecimal(Pgorder.PaymentAmount);
                                    payment.CreationDate = DateTime.Now;
                                    payment.AccountCreditId = null;
                                    payment.Bank = null;
                                    payment.BankBranch = null;
                                    payment.Drawer = null;

                                    ctx.Payments.InsertOnSubmit(payment);
                                    ctx.SubmitChanges();
                                    if (payment.PaymentID <= 0)
                                    {
                                        return null;
                                    }
                                    int PaymentId = payment.PaymentID;
                                    ///insert PaymentDetails and Online Payment Details Table (We will check for all 3 types of order and add each record)
                                    if (orderId > 0)
                                    {
                                        var OrderInvoice = ctx.Invoices.SingleOrDefault(o => o.OrderID == orderId);
                                        OnlinePaymentDetail oDObj = new OnlinePaymentDetail();
                                        oDObj.OnlinePaymentId = onlinePaymentId;
                                        oDObj.OrderId = orderId;
                                        oDObj.AmountPaid = OrderInvoice.AmountDue ;
                                        ctx.OnlinePaymentDetails.InsertOnSubmit(oDObj);
                                        ctx.SubmitChanges();

                                        PaymentDetail paymentDetail = new PaymentDetail();
                                        paymentDetail.AmountPaid = OrderInvoice.AmountDue;
                                        paymentDetail.OrderID = orderId;
                                        paymentDetail.PaymentID = PaymentId;
                                        ctx.PaymentDetails.InsertOnSubmit(paymentDetail);
                                        ctx.SubmitChanges();
                                       
                                    }
                                    if (stockId > 0)
                                    {
                                        var stockInvoice = ctx.Invoices.SingleOrDefault(o => o.OrderID == stockId);
                                        OnlinePaymentDetail oDObjs = new OnlinePaymentDetail();
                                        oDObjs.OnlinePaymentId = onlinePaymentId;
                                        oDObjs.OrderId = stockId;
                                        oDObjs.AmountPaid = stockInvoice.AmountDue;
                                        ctx.OnlinePaymentDetails.InsertOnSubmit(oDObjs);
                                        ctx.SubmitChanges();

                                        PaymentDetail paymentDetails= new PaymentDetail();
                                        paymentDetails.AmountPaid = stockInvoice.AmountDue;
                                        paymentDetails.OrderID = stockId;
                                        paymentDetails.PaymentID = PaymentId;
                                        ctx.PaymentDetails.InsertOnSubmit(paymentDetails);
                                        ctx.SubmitChanges();
                                    }
                                    if (photoOrderId > 0)
                                    {
                                        var photoInvoice = ctx.Invoices.SingleOrDefault(o => o.OrderID == photoOrderId);
                                        OnlinePaymentDetail oDObjp = new OnlinePaymentDetail();
                                        oDObjp.OnlinePaymentId = onlinePaymentId;
                                        oDObjp.OrderId = photoOrderId;
                                        oDObjp.AmountPaid = photoInvoice.AmountDue;
                                        ctx.OnlinePaymentDetails.InsertOnSubmit(oDObjp);
                                        ctx.SubmitChanges();

                                        PaymentDetail paymentDetailp = new PaymentDetail();
                                        paymentDetailp.AmountPaid = photoInvoice.AmountDue;
                                        paymentDetailp.OrderID = photoOrderId;
                                        paymentDetailp.PaymentID = PaymentId;
                                        ctx.PaymentDetails.InsertOnSubmit(paymentDetailp);
                                        ctx.SubmitChanges();
                                    }
                                  
                                    /// update OnlinePayment table
                                    OnlinePayment onPayment = (from o in ctx.OnlinePayments
                                                               where o.OnlinePaymentId == onlinePaymentId
                                                               select o).SingleOrDefault();

                                    onPayment.PaymentId = PaymentId;
                                    onPayment.TransactionComplete = true;
                                    onPayment.CBATransactionRef = PaymentResponse;

                                    ctx.SubmitChanges();
                                    ctx.Transaction.Commit();


                                }

                            }
                            catch (Exception ex)
                            {
                                ctx.Transaction.Rollback();
                                ret.OrderHasError = true;
                                Logger.Exception(ex, string.Format("ClientID: {0}\r\nPropertyID: {1}\r\n{2}", propertyOrder.ClientId, propertyOrder.PropertyId, propertyOrder.GetHTMLString()));
                                SendMail("notifications@photosigns.com.au", OnlineBLConfig.SEND_ERROR_MESSAGE_TO, null, "Error Processing Order", "Order Details:\r\n\r\n" + propertyOrder.GetHTMLString() + "XML:\r\n" + propertyOrder.GetXml() + "\r\n");
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    ctx.Transaction.Rollback();
                }
            }

            ret.PropertyId = PropertyId;
            ret.OrderId = orderId;
            ret.PhotoOrderId = photoOrderId;
            ret.StockId = stockId;
            ret.ListingId = listingId;

            return ret;

        }

        public bool IsWeekend(DateTime date)
        {
            return ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday));
        }

        public bool SendMail(string mailFrom, string mailTo, string mailTo2, string sub, string body)
        {
            try
            {
                SmtpClient smtp = new SmtpClient(OnlineBLConfig.SMTP_SERVER);
                MailMessage myMail = new MailMessage();

                myMail.From = new MailAddress(mailFrom);
                myMail.To.Add(new MailAddress(mailTo));
                myMail.Subject = sub;
                myMail.IsBodyHtml = true;
                myMail.Body = body;
                if (mailTo2 != null)
                {
                    myMail.CC.Add(new MailAddress(mailTo2));
                }
                smtp.Send(myMail);
                return true;
            }
            catch (Exception exx)
            {
                string sEx = exx.Message;
                return false;
            }
        }

        #endregion

        #region GetProofFileList
        public string GetProofFileList(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                string text = string.Empty;
                List<string> fileList = new List<string>();

                IFile vfile = VirtualFileSystemFactory.GetFile();

                //check proofing_01 server first
                if (vfile.ExistsDir(ServiceConfig.PROOF_DIR))
                {
                    string[] files = vfile.GetFiles(ServiceConfig.PROOF_DIR, orderID + "*.pdf");
                    string[] jpgFiles = vfile.GetFiles(ServiceConfig.PROOF_DIR, orderID + "*.jpg");

                    foreach (string file in files)
                    {
                        if (Path.GetFileName(file).StartsWith(orderID.ToString()))
                        {
                            fileList.Add(Path.GetFileName(file));
                        }
                    }

                    foreach (string file in jpgFiles)
                    {
                        if (Path.GetFileName(file).StartsWith(orderID.ToString()))
                        {
                            fileList.Add(Path.GetFileName(file));
                        }
                    }
                }

                if (fileList != null && fileList.Count > 0)
                {
                    Logger.Info("Number of file: " + fileList.Count);

                    string extraSalt = "";
                    int clientID = 0;
                    try
                    {
                        using (AbcDataContext ctx = new AbcDataContext())
                        {
                            Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                            if (od == null)
                                return string.Empty;

                            clientID = od.ClientID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, "Error occured in 'GetProofFileList - ClientID'. orderID:{0}", orderID);
                    }

                    foreach (string fileName in fileList)
                    {
                        if (clientID == 19202 || clientID == 19812 || clientID == 20379)
                        {
                            if (!string.IsNullOrEmpty(fileName) && fileName.ToLower().Contains("_br"))
                            {
                                extraSalt = "#1";
                            }
                        }

                        string fileNameWithSalt = fileName + extraSalt;

                        string encFileName = HttpUtility.UrlEncode(SimpleEncrypt.Encrypt(fileNameWithSalt, ServiceConfig.ENCRYPT_KEY));
                        string url = string.Format("{0}?P={1}", ServiceConfig.PROOF_WEB_SITE_URL, encFileName);
                        text += WritePreviewLine(fileName, url);
                    }
                }

                text += GetAOPPreviews(orderID);
                string fullText = "<table class=\"tblProofs\" cellpadding=\"1\" cellspacing=\"1\">";
                string endText = "</table>";

                if (text.IndexOf(".ashx", StringComparison.OrdinalIgnoreCase) > 0 ||
                     text.IndexOf(".aspx", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    text = fullText + text + endText;
                }

                return text;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProofFileList'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetAOPPreviews
        public string GetAOPPreviews(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                string fileList = string.Empty;

                //if (!IsApproveJobApplicable(orderID))
                //    return fileList;

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return string.Empty;

                    if (od.ProofDetail.DateApproved.HasValue || od.OrderDetails.All(odd => (!odd.UserDesignOnline.HasValue || odd.UserDesignOnline.Value == false)))
                    {
                        return string.Empty;
                    }

                }

                return fileList;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetAOPPreviews'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #region WritePreviewLine
        private string WritePreviewLine(string fileName, string url)
        {
            string ltPreviewList = string.Empty;
            ltPreviewList += string.Format("<tr><td>{0}</td>", fileName);
            ltPreviewList += string.Format("<td style=\"padding-left:15px;\"><a href=\"{0}\" target=\"_blank\">View</a></td></tr>", url);
            return ltPreviewList;
        }
        #endregion

        #endregion

        #region OrderHas
        public bool OrderHas(int orderID, int typeID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            bool ret = false;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (od.OrderDetails != null && od.OrderDetails.Count > 0)
                    {
                        foreach (OrderDetail anItem in od.OrderDetails)
                        {
                            if (anItem.Product.TypeID == typeID)
                            {
                                ret = true;
                                break;
                            }
                            //else if (anItem.Product.TypeID == ProductTypes.Packages || anItem.Product.TypeID == ProductTypes.BoardPackages || anItem.Product.TypeID == ProductTypes.OtherPackages)
                            //{
                            //    foreach (PackageContentGroup item in anItem.Product.PackageContentGroups)
                            //    {
                            //        foreach (PackageContentGroupProduct contentProductItem in item.PackageContentGroupProducts)
                            //        {
                            //            Product pro = ctx.Products.SingleOrDefault(p => p.ProductID == contentProductItem.ProductId);
                            //            if (pro != null && pro.TypeID == typeID)
                            //            {
                            //                ret = true;
                            //                break;
                            //            }

                            //        }
                            //        if (ret == true)
                            //            break;
                            //    }

                            //}
                        }
                    }
                    return ret;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'OrderHas'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #endregion

        #region GetSBOrderById
        public SB_Order GetSBOrderById(int orderId, List<EntityRelations> loadOptions)
        {
            if (orderId <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    SB_Order order = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == orderId);

                    return order;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetSBOrderById'. orderID:{0}", orderId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsSBErectionDetailsApplicable
        public bool IsSBErectionDetailsApplicable(int stockboardOrderID)
        {
            if (stockboardOrderID <= 0)
            {
                throw new ArgumentNullException("stockboardOrderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    ctx.DeferredLoadingEnabled = false;

                    SB_Order od = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == stockboardOrderID);

                    if (od == null)
                        return false;

                    if (!od.DateBoardErected.HasValue)
                    {
                        return true;
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsSBErectionDetailsApplicable'. stockboardOrderID:{0}", stockboardOrderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region AddStockBoardErectionDetails
        public void AddStockBoardErectionDetails(int orderID, string name, string message, int prefType, DateTime? prefDate)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (prefType < -1 || prefType > 1)
            {
                throw new ArgumentNullException("prefType");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.SB_Order_To_Client);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(options);

                    SB_Order od = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string note = string.Empty;

                    if (!string.IsNullOrEmpty(message))
                        note = "Installation Note by " + name + ": " + message;
                    else
                        note = "Installation Note by " + name;


                    if (string.IsNullOrEmpty(od.Notes))
                        od.Notes = note;
                    else
                        od.Notes = od.Notes + " -- " + note;

                    od.PreferredErectionDate = prefDate;
                    od.PreferredErectionType = prefType;

                    string prefErectionDate = string.Empty;
                    if (prefDate != null)
                    {
                        if (prefType == -1)
                            prefErectionDate = "Before or On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 0)
                            prefErectionDate = "On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 1)
                            prefErectionDate = "On or After " + prefDate.Value.ToString("dd/MM/yyyy");
                    }

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.ErectionNotesChanged;
                    string sub = "Abc Notification: Erection Notes Changed for Job No " + od.OrderID;
                    string xmlData = @"<EVENT>
										<OrderID>" + od.OrderID + @"</OrderID>
										<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
										<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
										<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Replace("&", "&amp;") + @"</PAddress>
										<Notes>" + od.Notes.Replace("&", "&amp;") + @"</Notes>
										<PreferredErectionDate>" + prefErectionDate + @"</PreferredErectionDate>
										<ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
										</EVENT>";

                    string textData = "Erection Notes Changed for Job No " + od.OrderID;
                    string source = "OnlineBL_OrderService_AddStockBoardErectionDetails";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string messages = string.Format("Error occured in 'AddStockBoardErectionDetails'. orderID:{0}", orderID);
                Logger.Exception(ex, messages);
                throw;
            }
        }

        #endregion

        #region IsStockBoardRemovalDetailsApplicable
        public bool IsStockBoardRemovalDetailsApplicable(int stockboardOrderID)
        {
            if (stockboardOrderID <= 0)
            {
                throw new ArgumentNullException("stockboardOrderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    SB_Order od = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == stockboardOrderID);

                    if (od == null)
                        return false;

                    if (!od.DateBoardRemoved.HasValue)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsStockBoardRemovalDetailsApplicable'. stockboardOrderID:{0}", stockboardOrderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region AddStockBoardRemovalDetails
        public void AddStockBoardRemovalDetails(int orderID, string name, string message, int prefType, DateTime? prefDate)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (prefType < -1 || prefType > 1)
            {
                throw new ArgumentNullException("prefType");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.SB_Order_To_Client);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(options);

                    SB_Order od = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string note = string.Empty;

                    if (!string.IsNullOrEmpty(message))
                        note = "Removal Note by " + name + ": " + message;
                    else
                        note = "Removal Note by " + name;


                    if (string.IsNullOrEmpty(od.Notes))
                        od.Notes = note;
                    else
                        od.Notes = od.Notes + " -- " + note;

                    od.PreferredRemovalDate = prefDate;
                    od.PreferredRemovalType = prefType;

                    string prefRemovalDate = string.Empty;
                    if (prefDate != null)
                    {
                        if (prefType == -1)
                            prefRemovalDate = "Before or On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 0)
                            prefRemovalDate = "On " + prefDate.Value.ToString("dd/MM/yyyy");
                        else if (prefType == 1)
                            prefRemovalDate = "On or After " + prefDate.Value.ToString("dd/MM/yyyy");
                    }

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string messages = string.Format("Error occured in 'AddStockBoardRemovalDetails'. orderID:{0}", orderID);
                Logger.Exception(ex, messages);
                throw;
            }
        }

        #endregion

        #region IsRequestForStockBoardRemovalApplicable
        public bool IsRequestForStockBoardRemovalApplicable(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    SB_Order od = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (od.DateDespatched.HasValue && od.DateBoardErected.HasValue && !od.DateRemovalRequested.HasValue)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsRequestForStockBoardRemovalApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region RequestStockBoardRemoval
        public void RequestStockBoardRemoval(int orderID, string name, string message)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.SB_Order_To_Client);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(options);

                    SB_Order od = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string note = string.Empty;

                    if (!string.IsNullOrEmpty(message))
                        note = "Date Removal Requested by " + name + ": " + message;
                    else
                        note = "Date Removal Requested by " + name;


                    if (string.IsNullOrEmpty(od.Notes))
                        od.Notes = note;
                    else
                        od.Notes = od.Notes + ". " + note;

                    od.DateRemovalRequested = DateTime.Now;

                    ctx.SP_EventGen_SBRemovalRequested(orderID, name, "OnlineBL_OrderService_RequestStockBoardRemoval");

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string messages = string.Format("Error occured in 'RequestStockBoardRemoval'. orderID:{0}", orderID);
                Logger.Exception(ex, messages);
                throw;
            }
        }

        #endregion

        #region GetAllLocations
        public List<Location> GetAllLocations(List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    if (loadOptions != null)
                        ctx.SetDataLoadOptions(loadOptions);

                    var los = (from l in ctx.Locations
                               orderby l.Location1
                               select l).ToList();

                    return los;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetAllLocations'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetAllLocationsBySearchTerm
        public List<Location> GetAllLocationsBySearchTerm(string term, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    if (loadOptions != null)
                        ctx.SetDataLoadOptions(loadOptions);

                    var los = (from l in ctx.Locations
                               where l.Location1.Contains(term)
                               orderby l.Location1
                               select l).ToList();

                    return los;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetAllLocationsBySearchTerm'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region PropertyOrdersHasBoardOrStockBoard
        public bool PropertyOrdersHasBoardOrStockBoard(int propertyID)
        {
            if (propertyID <= 0)
            {
                throw new ArgumentNullException("propertyID");
            }
            bool ret = false;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var os = (from o in ctx.Orders
                              join po in ctx.PropertyOrders on o.OrderID equals po.OrderId
                              join p in ctx.Properties on po.PropertyId equals p.PropertyId
                              where p.PropertyId == propertyID
                              select o).ToList();

                    if (os == null)
                        return false;

                    foreach (Order item in os)
                    {
                        if (OrderHas(item.OrderID, ProductTypes.BillBoard) || OrderHas(item.OrderID, ProductTypes.Stockboard) || OrderHas(item.OrderID, ProductTypes.BoardPackages))
                        {
                            ret = true;
                            break;
                        }
                    }
                    return ret;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'PropertyOrdersHasBoardOrStockBoard'. propertyID:{0}", propertyID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #endregion

        #region GetAllNotesByOrderID
        public string GetAllNotesByOrderID(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var od = ctx.Orders.FirstOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return string.Empty;

                    StringBuilder sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(od.Notes))
                    {
                        sb.Append(od.Notes.Trim());
                        sb.Append("<br />");
                    }
                    if (!string.IsNullOrEmpty(od.ErectionNotes))
                    {
                        sb.Append("Installation Notes: ");
                        sb.Append(od.ErectionNotes.Trim());
                        sb.Append("<br />");
                    }
                    if (!string.IsNullOrEmpty(od.RemovalNotes))
                    {
                        sb.Append("Removal Notes: ");
                        sb.Append(od.RemovalNotes.Trim());
                    }
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetAllNotesByOrderID'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetPrefereredErectionTypeAndDate
        public string GetPrefereredErectionTypeAndDate(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var dd = ctx.DespatchDetails.FirstOrDefault(o => o.OrderID == orderID);

                    if (dd == null)
                        return string.Empty;

                    StringBuilder sb = new StringBuilder();
                    if (dd.PreferredErectionType.HasValue && dd.PreferredErectionDate.HasValue)
                    {
                        if (dd.PreferredErectionType.Value == -1)
                            sb.Append("Before or On " + dd.PreferredErectionDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredErectionType.Value == 0)
                            sb.Append("On " + dd.PreferredErectionDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredErectionType.Value == 1)
                            sb.Append("On or After " + dd.PreferredErectionDate.Value.ToString("dd-MMM-yyyy"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetPrefereredErectionTypeAndDate'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetPrefereredRemovalTypeAndDate
        public string GetPrefereredRemovalTypeAndDate(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var dd = ctx.DespatchDetails.FirstOrDefault(o => o.OrderID == orderID);

                    if (dd == null)
                        return string.Empty;

                    StringBuilder sb = new StringBuilder();
                    if (dd.PreferredRemovalType.HasValue && dd.PreferredRemovalDate.HasValue)
                    {
                        if (dd.PreferredRemovalType.Value == -1)
                            sb.Append("Before or On " + dd.PreferredRemovalDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredRemovalType.Value == 0)
                            sb.Append("On " + dd.PreferredRemovalDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredRemovalType.Value == 1)
                            sb.Append("On or After " + dd.PreferredRemovalDate.Value.ToString("dd-MMM-yyyy"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetPrefereredRemovalTypeAndDate'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsBoardErectionApplicable
        public bool IsBoardErectionApplicable(int orderID)
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
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (!od.DespatchDetail.InTransitDt.HasValue && !od.DespatchDetail.DateBoardErected.HasValue && od.ProofDetail != null
                         && od.ProofDetail.DateApproved.HasValue)
                    {
                        if (od.OrderDetails != null && od.OrderDetails.Count > 0)
                        {
                            foreach (OrderDetail item in od.OrderDetails)
                            {
                                int id = item.Product.TypeID;
                                if (id == 1 || id == 4 || id == 9 || id == 10 ||
                                     id == 14 || id == 16)
                                {
                                    return true;
                                }
                                else if (id == 5 && (od.Client.ManagerID == ManagerSettings.WorkshopVictoria || od.Client.ManagerID == ManagerSettings.SignshopVictoria))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsBoardErectionDetailsApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsBoardRemovalApplicable
        public bool IsBoardRemovalApplicable(int orderID)
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
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (!od.DespatchDetail.DateRemovalRequested.HasValue && !od.DespatchDetail.DateBoardRemoved.HasValue
                         && od.DespatchDetail.DateBoardErected.HasValue)
                    {
                        if (od.OrderDetails != null && od.OrderDetails.Count > 0)
                        {
                            foreach (OrderDetail item in od.OrderDetails)
                            {
                                int id = item.Product.TypeID;
                                if (id == 1 || id == 4 || id == 9 || id == 10 ||
                                     id == 14 || id == 16)
                                {
                                    return true;
                                }
                                else if (id == 5 && (od.Client.ManagerID == ManagerSettings.WorkshopVictoria || od.Client.ManagerID == ManagerSettings.SignshopVictoria))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsBoardRemovalApplicable'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region NotifyUploadImages
        public void NotifyUploadImages(int orderID, List<UploadPhotoResponse> uploadPhotoResponses)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                StringBuilder sb = new StringBuilder();
                string colorCode = string.Empty;

                sb.Append("<TABLE style=\"font-family:Tahoma;font-size:9pt;border:1px solid gray;margin:5px;\">");

                foreach (UploadPhotoResponse item in uploadPhotoResponses)
                {
                    if (item.Quality == UploadedFileQualityType.RED)
                    {
                        colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.RED);
                        if (!string.IsNullOrEmpty(item.FileName))
                        {
                            sb.AppendFormat("<TR><TD width=\"20%\"><B>File Name</B> :</TD><TD width=\"80%\"><A href=\"file:{0}\"><font color=\"{2}\">{1}</font></A> - Quality: <font color=\"{2}\">{2}</font></TD></TR>", item.FileName.Replace("\\", "/"), item.FileName, colorCode);
                        }
                    }
                    else if (item.Quality == UploadedFileQualityType.IMAGING
                                                                    || item.Quality == UploadedFileQualityType.GRAY)
                    {
                        colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.GRAY);
                        if (!string.IsNullOrEmpty(item.FileName))
                        {
                            sb.AppendFormat("<TR><TD width=\"20%\"><B>File Name</B> :</TD><TD width=\"80%\"><A href=\"file:{0}\"><font color=\"{2}\">{1}</font></A> - Quality: <font color=\"{2}\">{2}</font></TD></TR>", item.FileName.Replace("\\", "/"), item.FileName, colorCode);
                        }
                    }
                    else if (item.Quality == UploadedFileQualityType.GREEN)
                    {
                        colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.GREEN);
                        if (!string.IsNullOrEmpty(item.FileName))
                        {
                            sb.AppendFormat("<TR><TD width=\"20%\"><B>File Name</B> :</TD><TD width=\"80%\"><A href=\"file:{0}\"><font color=\"{2}\">{1}</font></A> - Quality: <font color=\"{2}\">{2}</font></TD></TR>", item.FileName.Replace("\\", "/"), item.FileName, colorCode);
                        }
                    }
                    else if (item.Quality == UploadedFileQualityType.GRAPHICS)
                    {
                        colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.GRAPHICS);
                        if (!string.IsNullOrEmpty(item.FileName))
                        {
                            sb.AppendFormat("<TR><TD width=\"20%\"><B>File Name</B> :</TD><TD width=\"80%\"><A href=\"file:{0}\"><font color=\"{2}\">{1}</font></A> - Quality: <font color=\"{2}\">{2}</font></TD></TR>", item.FileName.Replace("\\", "/"), item.FileName, colorCode);
                        }
                    }
                    else if (item.Quality == UploadedFileQualityType.OTHER_DOCS)
                    {
                        colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.OTHER_DOCS);
                        if (!string.IsNullOrEmpty(item.FileName))
                        {
                            sb.AppendFormat("<TR><TD width=\"20%\"><B>File Name</B> :</TD><TD width=\"80%\"><A href=\"file:{0}\"><font color=\"{2}\">{1}</font></A> - Quality: <font color=\"{2}\">{2}</font></TD></TR>", item.FileName.Replace("\\", "/"), item.FileName, colorCode);
                        }
                    }
                    if (item.ForBoards)
                    {
                        sb.AppendFormat("<TR><TD width=\"20%\"><B>Usage</B> :</TD><TD width=\"80%\">For Boards</TD></TR>");
                    }
                    if (item.ForBrochures)
                    {
                        sb.AppendFormat("<TR><TD width=\"20%\"><B>Usage</B> :</TD><TD width=\"80%\">For Brochures</TD></TR>");
                    }
                    if (!string.IsNullOrEmpty(item.Notes))
                    {
                        sb.AppendFormat("<TR><TD width=\"20%\"><B>NOTE</B> :</TD><TD width=\"80%\">" + item.Notes.ToUpper() + "</TD></TR>");
                    }
                    sb.AppendFormat("<TD colspan=\"2\" style=\"text-align:center\">------------------------------------------------------------------</TD></TR>");

                }

                sb.Append("</TABLE>");


                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.Order_To_Client);
                    options.Add(EntityRelations.Order_To_Location);

                    Order od = (from o in ctx.Orders
                                where o.OrderID == orderID
                                select o).FirstOrDefault();
                    if (od != null)
                    {
                        //ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.FileUpload;
                        string sub = "File Uploaded for Order " + orderID;
                        string xmlEventData = @"<HTML><head></head><body>
												<p>
													An Agent has uploaded File in our system.
												</p>
												<p>
													<b><u>Details:</u></b>
												</p>
												<p>
													Agent Name: " + od.Client.ClientName + "/" + od.Client.Office + @"<br />
													Job No    : " + orderID + @"<br />
												</p>
												<p>
													" + sb.ToString() + @"</p>
												</body>
												</html>";

                        string textData = xmlEventData;
                        string source = "OnlineBL_OrderService_NotifyUploadImages";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, orderID, od.ClientID, null, null, source, "");
                    }
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'NotifyUploadImages'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #endregion

        #region IOrderService Members

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>
        /// Returns A list Of JobSearch Obj.
        /// </returns>
        public List<JobSearch> GetJobs(int clientId)
        {
            try
            {
                if (clientId <= 0)
                {
                    return null;
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Client_To_Manager);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.SB_Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                    loadOptions.Add(EntityRelations.SB_Order_To_SB_OrderDetails);
                    loadOptions.Add(EntityRelations.SB_OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    var cl = ctx.Clients.FirstOrDefault(c => c.ClientID == clientId);

                    if (cl != null && cl.Manager != null && cl.Manager.IsWorkshop)
                    {
                        List<Order> jobList = (from o in ctx.Orders
                                               join c in ctx.Clients on o.ClientID equals c.ClientID
                                               join dd in ctx.DespatchDetails on o.OrderID equals dd.OrderID
                                               join od in ctx.OrderDetails on o.OrderID equals od.OrderID
                                               join p in ctx.Products on od.ProductID equals p.ProductID
                                               //join w in ctx.ViewBoardOrdersRemovals on o.OrderID equals w.OrderID
                                               where o.ClientID == clientId
                                               && dd.DateBoardErected != null
                                               && dd.DateRemovalRequested == null
                                               && dd.DateBoardRemoved == null
                                               && (p.TypeID == ProductTypes.BillBoard || p.TypeID == ProductTypes.Stockboard)
                                               && p.ProductID != ProductSettings.FourByThreeFlatpack
                                               && p.ProductID != ProductSettings.SixByFourFlatpack
                                               orderby o.OrderID ascending
                                               select o).ToList().Distinct().OrderBy(od => od.OrderID).ToList();

                        List<JobSearch> js = GetJobSearchList(jobList);

                        return js;
                    }
                    else
                    {
                        List<Order> jobList = (from o in ctx.Orders
                                               join c in ctx.Clients on o.ClientID equals c.ClientID
                                               join dd in ctx.DespatchDetails on o.OrderID equals dd.OrderID
                                               join od in ctx.OrderDetails on o.OrderID equals od.OrderID
                                               join p in ctx.Products on od.ProductID equals p.ProductID
                                               //join w in ctx.ViewBoardOrdersRemovals on o.OrderID equals w.OrderID
                                               where o.ClientID == clientId
                                               && dd.DateBoardErected != null
                                               && dd.DateRemovalRequested == null
                                               && dd.DateBoardRemoved == null
                                               && ((p.TypeID == ProductTypes.BillBoard || p.TypeID == ProductTypes.Stockboard || p.TypeID == ProductTypes.Overlay || p.TypeID == ProductTypes.BoardAccessory
                                                   || p.TypeID == ProductTypes.BoardPackages || p.TypeID == ProductTypes.ForPrinting) || (p.TypeID == ProductTypes.SignShop && (o.ManagerID == ManagerSettings.WorkshopVictoria || o.ManagerID == ManagerSettings.SignshopVictoria)))
                                               orderby o.OrderID
                                               select o).ToList().Distinct().ToList();

                        List<JobSearch> js = GetJobSearchList(jobList);
                        foreach (JobSearch job in js)
                        {
                            List<Order> lightBOrSpotlightB = (from o in ctx.Orders
                                                              join od in ctx.OrderDetails on o.OrderID equals od.OrderID
                                                              join p in ctx.Products on od.ProductID equals p.ProductID
                                                              where (o.OrderID == job.OrderID && (p.FrameType == "Light Board" || p.CategoryId == CategoryTypes.Spotlight))
                                                              select o).ToList<Order>();

                            if (lightBOrSpotlightB != null && lightBOrSpotlightB.Count > 0)
                            {
                                job.IsSpotorLightBoard = true;
                            }
                            else
                            {
                                job.IsSpotorLightBoard = false;
                            }
                        }
                        List<SB_Order> jobListSB = (from o in ctx.SB_Orders
                                                    join c in ctx.Clients on o.ClientID equals c.ClientID
                                                    where o.ClientID == clientId
                                                    && o.DateBoardErected != null
                                                    && o.DateDespatched != null
                                                    && o.DateRemovalRequested == null
                                                    && o.DateBoardRemoved == null
                                                    orderby o.OrderID
                                                    select o).ToList().Distinct().ToList();

                        List<JobSearch> jsSB = GetJobSearchListSB(jobListSB);
                        foreach (JobSearch job in js)
                        {
                            List<SB_Order> lightBOrSpotlightB = (from o in ctx.SB_Orders
                                                                 join od in ctx.SB_OrderDetails on o.OrderID equals od.OrderID
                                                                 join p in ctx.Products on od.ProductID equals p.ProductID
                                                                 where (o.OrderID == job.OrderID && (p.FrameType == "Light Board" || p.CategoryId == CategoryTypes.Spotlight))
                                                                 select o).ToList<SB_Order>();

                            if (lightBOrSpotlightB != null && lightBOrSpotlightB.Count > 0)
                            {
                                job.IsSpotorLightBoard = true;
                            }
                            else
                            {
                                job.IsSpotorLightBoard = false;
                            }
                        }
                        List<JobSearch> all = js.Concat(jsSB).ToList().OrderBy(j => j.OrderID).ToList();

                        return all;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, string.Format("clientId:{0}", clientId));
                return null;
            }
        }
        private List<JobSearch> GetJobSearchList(List<Order> orderListObj)
        {
            if (orderListObj == null || orderListObj.Count <= 0)
            {
                return new List<JobSearch>();
            }

            List<JobSearch> jobSearchList = new List<JobSearch>();
            
            foreach (Order job in orderListObj)
            {
                JobSearch jobSearchObj = new JobSearch();
                jobSearchObj.OrderID = job.OrderID;
                jobSearchObj.DateReceived = job.DateReceived;
                jobSearchObj.ClientID = job.ClientID;
                jobSearchObj.ClientName = job.Client.ClientName;
                jobSearchObj.PropertyAddress = job.PropertyAddress;
                jobSearchObj.Location1 = job.Location.Location1;
                jobSearchObj.State = job.Location.State;
                jobSearchObj.Caption = job.Caption;
                jobSearchObj.DateBoardErected = job.DespatchDetail.DateBoardErected;
                bool light = false;
                try
                {
                    light = job.OrderDetails.Any(od => od.Product.FrameType == "Light Board" || od.Product.CategoryId == CategoryTypes.Spotlight);
                }
                catch (Exception)
                {
                    Logger.Warn("Can not assign the value: " + job.OrderID.ToString());
                }
                jobSearchObj.IsSpotorLightBoard = light;
                jobSearchList.Add(jobSearchObj);
            }

            return jobSearchList;
        }

        private List<JobSearch> GetJobSearchListSB(List<SB_Order> orderListObj)
        {
            if (orderListObj == null || orderListObj.Count <= 0)
            {
                return new List<JobSearch>();
            }

            List<JobSearch> jobSearchList = new List<JobSearch>();

            foreach (SB_Order job in orderListObj)
            {
                JobSearch jobSearchObj = new JobSearch();
                jobSearchObj.OrderID = job.OrderID;
                jobSearchObj.DateReceived = job.DateReceived;
                jobSearchObj.ClientID = job.ClientID;
                jobSearchObj.ClientName = job.Client.ClientName;
                jobSearchObj.PropertyAddress = job.PropertyAddress;
                jobSearchObj.Location1 = job.Location;
                jobSearchObj.State = job.State;
                jobSearchObj.Caption = job.Caption;
                jobSearchObj.DateBoardErected = job.DateBoardErected;
                jobSearchList.Add(jobSearchObj);
            }

            return jobSearchList;
        }


        /// <summary>
        /// Batches the request removal.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="jobs">The jobs.</param>
        /// <param name="reqBy">The req by.</param>
        /// <returns>Returns the string value of the Request which  is processed or not.</returns>
        public string BatchRequestRemoval(int clientId, List<int> jobs, string reqBy, int? removalType, DateTime removalDate)
        {
            string ret = "OK";

            try
            {
                if (jobs.Count <= 0)
                {
                    return string.Empty;
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        foreach (int orderID in jobs)
                        {
                            /// Normal Order
                            if (orderID < 900000 || (orderID > 950000 && orderID < 99000000))
                            {
                                var oldDate = ctx.DespatchDetails.SingleOrDefault(o => o.OrderID == orderID).DateRemovalRequested;

                                int totalOrders = (from o in ctx.Orders
                                                   where o.OrderID == orderID && o.ClientID == clientId
                                                   select o).Count();

                                if (totalOrders == 1 && oldDate == null)
                                {
                                    var obj = ctx.DespatchDetails.Single(o => o.OrderID == orderID);
                                    obj.RBy = reqBy;

                                    //string dateStr = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");

                                    if (removalType == null)
                                    {
                                        removalType = 1;
                                    }

                                    obj.PreferredRemovalDate = removalDate;
                                    obj.PreferredRemovalType = removalType;
                                    //if (removalDate != null)
                                    //{
                                    //    //Logger.Warn("Request removal" + removalDate.ToString());
                                    //    obj.UrgentRemoval = true;
                                    //}
                                    ctx.SubmitChanges();

                                    int? val = ctx.ABCWRKFLOW_BoardsRemReqSet(orderID, DateTime.Now, true);
                                    ctx.SubmitChanges();

                                }
                            }
                            else ///Stockboard Order
                            {
                                var obj = ctx.SB_Orders.Single(o => o.OrderID == orderID);

                                if (obj.ClientID == clientId && obj.DateRemovalRequested == null)
                                {
                                    var objSbOrders = ctx.SB_Orders.Single(o => o.OrderID == orderID);

                                    //string dateStr = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
                                    objSbOrders.DateRemovalRequested = DateTime.Now;
                                    objSbOrders.PreferredRemovalDate = removalDate;
                                    objSbOrders.PreferredRemovalType = removalType;

                                    ctx.SubmitChanges();

                                    int? val = ctx.SP_EventGen_SBRemovalRequested(orderID, reqBy, "BatchRequestRemoval");
                                    //retBuilder.Append("OK");
                                }
                                //else
                                //{
                                //    retBuilder.Append("Board Removal is already requested");
                                //}
                            }
                        }
                        ret = "OK";
                    }
                    catch (SqlException sqlEx)
                    {
                        ret = "An Error Occured in Database while Processing your Request, Please Try again";

                        /// General Error Occured, Rollback the Changes
                        ctx.GetChangeSet().Updates.Clear();
                        Logger.Exception(sqlEx, string.Format("clientId:{0}, Jobs:{1}, reqBy:{2}", clientId, jobs, reqBy));
                    }
                    catch (Exception ex)
                    {
                        /// General Error Occured, Rollback the Changes
                        ctx.GetChangeSet().Updates.Clear();
                        ret = "An Error Occured while Processing your Request, Please Try again. ";
                        ret += "This Error has been logged.";
                        Logger.Exception(ex, string.Format("clientId:{0}, Jobs:{1}, reqBy:{2}", clientId, jobs, reqBy));
                    }
                }
            }
            catch (Exception ex)
            {
                ret = "An Error Occured while Processing your Request, Please Try again. ";
                ret += "This Error has been logged.";
                Logger.Exception(ex, string.Format("clientId:{0}, Jobs:{1}, reqBy:{2}", clientId, jobs, reqBy));
            }

            return ret;
        }

        /// <summary>
        /// Gets the SMS order details bu client ID.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <returns>
        /// Returns List of the SmsOrderDetail object.
        /// </returns>
        public List<Order> GetSMSOrderDetailsByClientID(int clientID)
        {
            try
            {
                if (clientID <= 0)
                {
                    return null;
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderOtherDetail);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Order_To_PropertyOrder);
                    loadOptions.Add(EntityRelations.PropertyOrder_To_Property);
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);
                    List<Order> smsOrderDetailList = (from o in ctx.Orders

                                                      join ood in ctx.OrderOtherDetails on o.OrderID equals ood.OrderId
                                                      join l in ctx.Locations on o.LocationID equals l.LocationID
                                                      join po in ctx.PropertyOrders on o.OrderID equals po.OrderId
                                                      join p in ctx.Properties on po.PropertyId equals p.PropertyId
                                                      join sq in ctx.SMS_Queues on o.OrderID equals sq.OrderId into sqo
                                                      where ood.SMS_Allowed == true && o.ClientID == clientID
                                                      from sq in sqo.DefaultIfEmpty()
                                                      orderby o.OrderID descending
                                                      select o).ToList();
                    return smsOrderDetailList;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets the SMS order details for A month.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="clientId">The client id.</param>
        /// <returns>
        /// Returns List of the Order List object.
        /// </returns>
        public List<Order> GetSmsOrderDetailsForAMonth(int year, int month, int clientId)
        {
            try
            {
                if (clientId <= 0)
                {
                    return null;
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string fromDate = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
                    string toDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd");

                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderOtherDetail);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Order_To_PropertyOrder);
                    loadOptions.Add(EntityRelations.PropertyOrder_To_Property);
                    loadOptions.Add(EntityRelations.SMS_Queue_To_OrderOtherDetail);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    List<Order> orderDetailsList = (from o in ctx.Orders
                                                    where (from sq in ctx.SMS_Queues
                                                           where sq.RcvdOn >= DateTime.Parse(fromDate) && sq.RcvdOn <= DateTime.Parse(toDate)
                                                           select sq.OrderId).Contains(o.OrderID)

                                                    join ood in ctx.OrderOtherDetails on o.OrderID equals ood.OrderId
                                                    join l in ctx.Locations on o.LocationID equals l.LocationID
                                                    join po in ctx.PropertyOrders on o.OrderID equals po.OrderId
                                                    join p in ctx.Properties on po.PropertyId equals p.PropertyId
                                                    join smsq in ctx.SMS_Queues on o.OrderID equals smsq.OrderId
                                                    where ood.SMS_Allowed == true && o.ClientID == clientId
                                                    orderby o.OrderID descending
                                                    select o).ToList();
                    return orderDetailsList;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, ex.Message);
                return null;
            }
        }

        #endregion

        #region GetARPropertyTypeByID
        public AR_PropertyType GetARPropertyTypeByID(int pTypeID)
        {
            if (pTypeID <= 0)
            {
                throw new ArgumentNullException("pTypeID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    AR_PropertyType pType = ctx.AR_PropertyTypes.SingleOrDefault(o => o.PType == pTypeID);

                    return pType;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetARPropertyTypeByID'. pTypeID:{0}", pTypeID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region UploadSinglePhoto
        public UploadPhotoResponse UploadSinglePhoto(UploadPhotoRequest requests, OrderTrackingEventParameter orderTracking)
        {
            List<string> outFiles = new List<string>();
            #region Sanity Check
            int clientId;
            GetOrderImageRequirementsResult imageRequirements = null;

            if (orderTracking == null)
            {
                throw new ArgumentNullException("OrderTrackingEventParameter");
            }
            if (orderTracking.OrderId <= 0)
            {
                throw new ArgumentOutOfRangeException("OrderId");
            }

            using (AbcDataContext ctx = new AbcDataContext())
            {
                Order od = (from o in ctx.Orders
                            where o.OrderID == orderTracking.OrderId
                            select o).FirstOrDefault();
                if (od == null)
                {
                    orderTracking.OrderId = 0;
                    throw new System.Exception("Job number not found in server");
                }
                clientId = od.ClientID;
                imageRequirements = ctx.GetOrderImageRequirements(orderTracking.OrderId).SingleOrDefault();
            }

            #endregion

            //Mark order to short state Image Extracting
            orderTracking.Message = "Extracting Images";
            UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.ImageExtracting, false);

            #region Sanity Check
            if (string.IsNullOrEmpty(requests.UncFilePath))
            {
                throw new ArgumentNullException("uncFilePath");
            }
            //if (!requests.UncFilePath.StartsWith("\\\\"))
            //{
            //    throw new ArgumentException("uncFilePath doesn't start with \\\\");
            //}
            if (!File.Exists(requests.UncFilePath))
            {
                throw new ApplicationException("uncFilePath doesn't exist");
            }
            if (requests.FileSelectMode == UploadedFileType.AgentPhoto && String.IsNullOrEmpty(requests.AgentContactName))
            {
                throw new ArgumentNullException("contactAgentName");
            }
            #endregion

            string outPath = OnlineBLConfig.RED_PROPERTY_PHOTO_OUTPUT_FOLDER;
            string outFile = string.Format("{0}_{1}", orderTracking.OrderId, requests.FileName);
            UploadPhotoResponse uploadPhotoResponse = new UploadPhotoResponse();
            uploadPhotoResponse.FileSelectMode = requests.FileSelectMode;
            uploadPhotoResponse.Quality = UploadedFileQualityType.RED;
            uploadPhotoResponse.Notes = requests.Notes;

            try
            {
                if (requests.FileSelectMode == UploadedFileType.ImagingPhoto)
                {
                    outPath = OnlineBLConfig.IMAGING_PROPERTY_PHOTO_OUTPUT_FOLDER;

                    outFile = string.Format("{0}_{1}", orderTracking.OrderId, requests.FileName);
                    uploadPhotoResponse.Quality = UploadedFileQualityType.IMAGING;
                }
                else if (requests.FileSelectMode == UploadedFileType.GraphicsPhoto)
                {
                    outPath = OnlineBLConfig.GRAPHICS_PROPERTY_PHOTO_OUTPUT_FOLDER;

                    outFile = string.Format("{0}_{1}", orderTracking.OrderId, requests.FileName);
                    uploadPhotoResponse.Quality = UploadedFileQualityType.GRAPHICS;
                }
                else if (requests.FileSelectMode == UploadedFileType.AgentPhoto)
                {
                    outFile = string.Format("{0}_AGENTPHOTO_{1}_{2}{3}", orderTracking.OrderId, requests.AgentContactName.Replace("'", ""), clientId, Path.GetExtension(requests.UncFilePath).ToLower());
                    uploadPhotoResponse.Quality = UploadedFileQualityType.IMAGING;
                }
                else if (requests.FileSelectMode == UploadedFileType.OtherDoc)
                {
                    outPath = OnlineBLConfig.OTHERS_DOCUMENT_OUTPUT_FOLDER;

                    outFile = string.Format("{0}_{1}", orderTracking.OrderId, requests.FileName);
                    uploadPhotoResponse.Quality = UploadedFileQualityType.OTHER_DOCS;
                }
                else
                {

                    if (imageRequirements != null && imageRequirements.ImageRequirements_MinimumMegaPixels > 0)
                    {
                        string ext = Path.GetExtension(requests.UncFilePath).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp" || ext == ".tiff")
                        {
                            //build up image quality and decide right folder image should go to
                            ImageQuality quality = ImageQuality.GetImageQuality(requests.UncFilePath);
                            decimal inMegaPixel = quality.GetInMegaPixel();

                            if (inMegaPixel >= (decimal)imageRequirements.ImageRequirements_RecommendedMegaPixels)
                            {
                                uploadPhotoResponse.Quality = UploadedFileQualityType.GREEN;
                                outPath = OnlineBLConfig.GREEN_PROPERTY_PHOTO_OUTPUT_FOLDER;

                            }
                            else if (inMegaPixel >= (decimal)imageRequirements.ImageRequirements_MinimumMegaPixels)
                            {
                                uploadPhotoResponse.Quality = UploadedFileQualityType.GRAY;
                                outPath = OnlineBLConfig.GRAY_PROPERTY_PHOTO_OUTPUT_FOLDER;
                            }

                            outFile = string.Format("{0}_{1}", orderTracking.OrderId, requests.FileName);
                        }
                    }
                }

                outFile = Path.Combine(outPath, outFile);
                outFiles.Add(outFile);

                //Put image into right folder
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                    File.Delete(outFile);
                }
                IFile file = VirtualFileSystemFactory.GetFile();
                file.WriteAllBytes(outFile, File.ReadAllBytes(requests.UncFilePath));
                //File.Copy(requests.UncFilePath, outFile, true);

                //Delete the temp file
                if (requests.UncFilePath != null && File.Exists(requests.UncFilePath))
                {
                    File.SetAttributes(requests.UncFilePath, FileAttributes.Normal);
                    File.Delete(requests.UncFilePath);
                }

                //return the value back to the client
                uploadPhotoResponse.FileName = outFile;
                uploadPhotoResponse.FileProcesed = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GradeImageQuality");
                uploadPhotoResponse.FileProcesed = false;
                uploadPhotoResponse.ErrorMessage = String.Format("System Error - {0}", ex.Message);
            }


            //If success
            //TODO: Check file type as other
            if (uploadPhotoResponse.Quality == UploadedFileQualityType.GRAPHICS)
            {
                orderTracking.Message = "Waiting for Graphic";
                UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.WaitingForGraphic, false);
            }
            else if (uploadPhotoResponse.Quality == UploadedFileQualityType.RED)
            {
                orderTracking.Message = "Waiting for Image to be processed";
                UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.WaitingForImageToBeProcessed, false);
            }
            else if (uploadPhotoResponse.Quality == UploadedFileQualityType.IMAGING || uploadPhotoResponse.Quality == UploadedFileQualityType.GRAY)
            {
                orderTracking.Message = "Waiting for Image to be processed";
                UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.WaitingForImageToBeProcessed, false);
            }
            else
            {
                orderTracking.Message = "Image Processed";
                UpdateOrderStatus(orderTracking, Abc.OnlineBL.Entities.Enums.WorkflowStates.ImageProcessed, false);
            }

            return uploadPhotoResponse;
        }

        #endregion

        public List<PickingSlipOrderInfo> GetOrdersForPickingSlip(int clientID)
        {

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var orders = (from o in ctx.Orders
                                  where o.ClientID == clientID
                                  join prd in ctx.ProofDetails on o.OrderID equals prd.OrderID
                                  //where prd.DateApproved.HasValue == false
                                  join po in ctx.PropertyOrders on o.OrderID equals po.OrderId
                                  join p in ctx.Properties on po.PropertyId equals p.PropertyId
                                  join lo in ctx.Locations on p.LocationId equals lo.LocationID
                                  join od in ctx.OrderDetails on o.OrderID equals od.OrderID
                                  join pd in ctx.Products on od.ProductID equals pd.ProductID
                                  where (pd.TypeID == ProductTypes.BillBoard || pd.TypeID == ProductTypes.Brochure || pd.TypeID == ProductTypes.WindowCard)
                                  join pt in ctx.ProductTypes on pd.TypeID equals pt.TypeID
                                  where prd.DateApproved.HasValue == false
                                  select new
                                  {
                                      o.PropertyAddress,
                                      p.PropertyId,
                                      o.OrderID,
                                      lo.State,
                                      lo.PostCode,
                                      p.StreetName,
                                      p.StreetNo,
                                      p.UnitNo,
                                      pd.ProductID,
                                      pd.Name,
                                      pt.Type
                                  });

                    List<PickingSlipOrderInfo> list = (from n in orders
                                                       orderby n.OrderID descending
                                                       group n by n.OrderID + " - " + n.PropertyAddress + "--" + n.PostCode into g
                                                       select new PickingSlipOrderInfo { Property = g.Key, Products = (from o in g select new ProductInfo { OrderID = o.OrderID, ProductID = o.ProductID, ProductName = o.Name, ProductTypes = o.Type }).ToList() }).ToList();
                    list.Reverse();
                    return list;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, ex.Message);
                throw;
            }

        }

        public String EmailPropertyPhotoLink(int clientId, string property, string emailTo, string link)
        {
            string ret = "";
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.SP_EventGen_EmailPhotoLink(clientId, property, link, emailTo, "OnlineBL_OrderService_EmailPropertyPhotoLink");
                }

            }
            catch (Exception ex)
            {
                Logger.Exception(ex, ex.Message);
                ret = "Unknown Fatal Error Occured. This error has been logged";
            }
            return ret;
        }

        public String AddNewPickingSlip(int clientId, string property, int orderid, List<PickingSlipModel> ps)
        {
            String res = "";
            List<string> files = new List<string>();
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderid);

                    ctx.NewPickingSlips.InsertOnSubmit(new NewPickingSlip
                    {
                        ClientId = clientId,
                        PropertyAddress = property,
                        OrderId = orderid,
                        ProductXml = System.Xml.Linq.XElement.Parse(getProductXml(ps))
                    });

                    //Send an event
                    int eventID = EventSettings.NewPickingSlip;
                    string sub = "Abc Notification: Picking Slip For - " + orderid.ToString() + " - " + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;");

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<TABLE style=\"font-family:Tahoma;font-size:9pt;border:1px solid gray;margin:5px;\">");
                    foreach (var item in ps)
                    {
                        sb.Append("<tr><td><b>" + item.ProductName.Replace("&", "With") + " -- " + item.ProductID.ToString() + "</b></td></tr>");
                        sb.Append("<tr><td>");
                        sb.Append("<TABLE style=\"font-family:Tahoma;font-size:9pt;border:1px solid gray;margin:5px;\">");
                        sb.Append("<tr><td>Pref</td><td>Path</td><td>Notes</td></tr>");

                        foreach (var im in item.Images)
                        {
                            sb.AppendFormat("<tr><td width=\"10%\"><B>{0}</B></td><td width=\"50%\">{1}</td><td width=\"40%\">{2}</td></tr>", im.OrderNo, im.Path, im.Note);
                            if (!files.Contains(im.Path) && !string.IsNullOrEmpty(im.Path))
                            {
                                //files.Add(im.Path);
                                files.Add(im.Path.ToLower());
                            }
                        }

                        sb.Append("</TABLE>");
                        sb.Append("</td></tr>");
                    }

                    sb.Append("</TABLE>");

                    string phone = string.Empty;

                    if (!string.IsNullOrEmpty(od.Client.Phone))
                    {
                        phone = od.Client.Phone.Replace("&", "&amp;");
                    }

                    string xmlEventData = "<EVENT>" +
                                            "<ClientID>" + od.ClientID.ToString() + "</ClientID>" +
                                            "<OrderID>" + orderid.ToString() + "</OrderID>" +
                                            "<ClientName>" + od.Client.ClientName.Replace("&", "&amp;") + "</ClientName>" +
                                            "<Office>" + od.Client.Office.Replace("&", "&amp;") + "</Office>" +
                                            "<Phone>" + phone + "</Phone>" +
                                            "<Property>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + "</Property>" +
                                            "<ReqOn>" + DateTime.Today.ToString("dd/MM/yyyy") + "</ReqOn>" +
                                            "<PSlip>" + sb.ToString() + "</PSlip>" +
                                            "</EVENT>";

                    string textData = "Abc Notification: Picking Slip For - Job No " + od.OrderID.ToString();
                    string source = "OnlineBL_ManagerService_AddNewPickingSlip";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, od.OrderID, od.ClientID, null, null, source, "");
                    ctx.SubmitChanges();

                    //Move File
                    List<string> jpgFiles = GetJpgFiles(property, clientId);

                    foreach (string file in jpgFiles)
                    {
                        if (!files.Contains(Path.GetFileName(file).ToLower()))
                            continue;

                        string srcFile = file; // Path.ChangeExtension(file, ".eps");					

                        //if floorplans then copy eps
                        if (file.Contains("_000_001") || file.Contains("_001_001"))
                        {
                            //we always copy from 001 folder for eps
                            srcFile = srcFile.Replace("HiRes-", "").Replace("000_0", "001_0");
                            srcFile = Path.ChangeExtension(srcFile, ".eps");
                        }

                        string destFile = Path.Combine(ServiceConfig.PICKING_SLIP_SORT_DIR, string.Format("{0}_{1}", orderid, Path.GetFileName(srcFile)));

                        if (!Directory.Exists(ServiceConfig.PICKING_SLIP_SORT_DIR))
                            Directory.CreateDirectory(ServiceConfig.PICKING_SLIP_SORT_DIR);

                        if (!File.Exists(srcFile)) continue;
                        File.Copy(srcFile, destFile, true);

                    }

                    res = "Picking slip saved succeeded";
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'AddNewPickingSlip'. orderId:{0}, property:{1}",
                                            orderid.ToString(), property);
                Logger.Exception(ex, message);
                //Abc.Util2.Log.Logger.LogException(ex, new object[] { clientId, property, orderid, ps });
                res = "Unknown Fatal Error Occured. This error has been logged";

            }
            return res;
        }

        private string getProductXml(List<PickingSlipModel> psList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<products>");
            foreach (var item in psList)
            {
                sb.AppendFormat("<product id=\"{0}\">", item.ProductID);
                sb.Append("<images>");
                foreach (var item2 in item.Images)
                {
                    sb.Append("<image>");
                    sb.AppendFormat("<data orderno=\"{0}\" path=\"{1}\" note=\"{2}\" />", item2.OrderNo, item2.Path, item2.Note);
                    sb.Append("</image>");
                }
                sb.Append("</images>");
                sb.Append("</product>");
            }
            sb.Append("</products>");
            return sb.ToString();
        }

        private List<string> GetJpgFiles(string pro, int clientId)
        {
            List<string> ret = new List<string>();
            string dirToLook = ServiceConfig.PHOTO_DIR + PadClientId(clientId) + "\\" + pro;

            string[] files = Directory.GetFiles(dirToLook, "HiRes-" + PadClientId(clientId) + "_" + pro + "*.jpg");

            foreach (string file in files)
            {
                ret.Add(file);
            }
            return ret;
        }

        private string PadClientId(int clientId)
        {
            string sId = clientId.ToString();
            if (sId.Length < 4)
                return clientId.ToString().PadLeft(4, '0');
            else
                return clientId.ToString();
        }

        public NewPickSlipInfo GetNewPickingSlip(int pickingSlipId)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    return (from u in ctx.NewPickingSlips
                            where u.NewPickingSlipID == pickingSlipId
                            select new NewPickSlipInfo { ClientID = u.ClientId, PropertyAddress = u.PropertyAddress, OrderId = u.OrderId, ProductXml = u.ProductXml.Value }
                                    ).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Abc.Util2.Log.Logger.LogException(ex, new object[] { pickingSlipId });

            }
            return null;
        }

        #region GetOrderImageRequirement
        public GetOrderImageRequirementsResult GetOrderImageRequirement(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentOutOfRangeException("OrderId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    return ctx.GetOrderImageRequirements(orderId).SingleOrDefault();
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetOrderImageRequirement'. orderId:{0}", orderId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Gets the install file by order id.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>
        /// Returns the full fileName.
        /// </returns>
        public string GetInstallFileByOrderId(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentOutOfRangeException("OrderId");
            }

            string installFile = string.Empty;

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    installFile = ctx.OrderOtherDetails.SingleOrDefault(x => x.OrderId == orderId).InstallFile;
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetInstallFileByOrderId'. orderId:{0}", orderId);
                Logger.Exception(ex, message);
                throw;
            }
            return installFile;
        }

        #region InsertInDesignPrintQueue
        public RND_InDesignPrintQueue InsertInDesignPrintQueue(RND_InDesignPrintQueue printQueue)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.RND_InDesignPrintQueues.InsertOnSubmit(printQueue);

                    ctx.SubmitChanges();
                    return printQueue;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'InsertInDesignPrintQueue'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region UploadSingleArtwork
        public UploadPhotoResponse UploadSingleArtwork(UploadPhotoRequest requests, OrderTrackingEventParameter orderTracking)
        {
            #region Sanity Check
            if (orderTracking == null)
            {
                throw new ArgumentNullException("OrderTrackingEventParameter");
            }
            if (orderTracking.OrderId <= 0)
            {
                throw new ArgumentOutOfRangeException("OrderId");
            }

            using (AbcDataContext ctx = new AbcDataContext())
            {
                Order od = (from o in ctx.Orders
                            where o.OrderID == orderTracking.OrderId
                            select o).FirstOrDefault();
                if (od == null)
                {
                    orderTracking.OrderId = 0;
                    throw new System.Exception("Job number not found in server");
                }
            }

            if (string.IsNullOrEmpty(requests.UncFilePath))
            {
                throw new ArgumentNullException("uncFilePath");
            }
            //if (!requests.UncFilePath.StartsWith("\\\\"))
            //{
            //    throw new ArgumentException("uncFilePath doesn't start with \\\\");
            //}
            if (!File.Exists(requests.UncFilePath))
            {
                throw new ApplicationException("uncFilePath doesn't exist");
            }
            #endregion

            string outPath = OnlineBLConfig.ARTWORK_PDF_UPLOAD_FOLDER;
            string outFile = string.Format("{0}_{1}", orderTracking.OrderId, requests.FileName);
            UploadPhotoResponse uploadPhotoResponse = new UploadPhotoResponse();
            uploadPhotoResponse.FileSelectMode = requests.FileSelectMode;
            uploadPhotoResponse.Quality = UploadedFileQualityType.ARTWORK;

            try
            {
                outFile = Path.Combine(outPath, outFile);

                //Put image into right folder
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                    File.Delete(outFile);
                }
                IFile file = VirtualFileSystemFactory.GetFile();
                file.WriteAllBytes(outFile, File.ReadAllBytes(requests.UncFilePath));
                //File.Copy(requests.UncFilePath, outFile, true);

                //Delete the temp file
                if (requests.UncFilePath != null && File.Exists(requests.UncFilePath))
                {
                    File.SetAttributes(requests.UncFilePath, FileAttributes.Normal);
                    File.Delete(requests.UncFilePath);
                }

                //return the value back to the client
                uploadPhotoResponse.FileName = outFile;
                uploadPhotoResponse.FileProcesed = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "UploadSingleArtwork");
                uploadPhotoResponse.FileProcesed = false;
                uploadPhotoResponse.ErrorMessage = String.Format("System Error - {0}", ex.Message);
            }

            return uploadPhotoResponse;
        }

        #endregion

        #region NotifyUploadArtworks
        public void NotifyUploadArtworks(int orderID, List<UploadPhotoResponse> uploadPhotoResponses)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                StringBuilder sb = new StringBuilder();
                string colorCode = string.Empty;

                sb.Append("<TABLE style=\"font-family:Tahoma;font-size:9pt;border:1px solid gray;margin:5px;\">");

                foreach (UploadPhotoResponse item in uploadPhotoResponses)
                {

                    if (item.Quality == UploadedFileQualityType.ARTWORK)
                    {
                        colorCode = Enum.GetName(typeof(UploadedFileQualityType), UploadedFileQualityType.GREEN);
                        sb.AppendFormat("<TR><TD width=\"20%\"><B>File Name</B> :</TD><TD width=\"80%\"><A href=\"file:{0}\"><font color=\"{2}\">{1}</font></A> - Quality: <font color=\"{2}\">{2}</font></TD></TR>", item.FileName.Replace("\\", "/"), item.FileName, colorCode);
                    }

                    sb.AppendFormat("<TD colspan=\"2\" style=\"text-align:center\">------------------------------------------------------------------</TD></TR>");
                }

                sb.Append("</TABLE>");

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.Order_To_Client);
                    options.Add(EntityRelations.Order_To_Location);

                    Order od = (from o in ctx.Orders
                                where o.OrderID == orderID
                                select o).FirstOrDefault();
                    if (od != null)
                    {
                        //ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.ArtworkUploaded;
                        string sub = "Artwork PDFs Uploaded for Order " + orderID;
                        string xmlEventData = @"<HTML><head></head><body>
												<p>
													An Agent has uploaded Artwork PDFs in our system.
												</p>
												<p>
													<b><u>Details:</u></b>
												</p>
												<p>
													Agent Name: " + od.Client.ClientName + "/" + od.Client.Office + @"<br />
													Job No    : " + orderID + @"<br />
												</p>
												<p>
													" + sb.ToString() + @"</p>
												</body>
												</html>";

                        string textData = xmlEventData;
                        string source = "OnlineBL_OrderService_NotifyUploadArtworks";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, orderID, od.ClientID, null, null, source, "");
                    }
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'NotifyUploadArtworks'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GetStockBoardPrefereredErectionTypeAndDate
        public string GetStockBoardPrefereredErectionTypeAndDate(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var dd = ctx.SB_Orders.FirstOrDefault(o => o.OrderID == orderID);

                    if (dd == null)
                        return string.Empty;

                    StringBuilder sb = new StringBuilder();
                    if (dd.PreferredErectionType.HasValue && dd.PreferredErectionDate.HasValue)
                    {
                        if (dd.PreferredErectionType.Value == -1)
                            sb.Append("Before or On " + dd.PreferredErectionDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredErectionType.Value == 0)
                            sb.Append("On " + dd.PreferredErectionDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredErectionType.Value == 1)
                            sb.Append("On or After " + dd.PreferredErectionDate.Value.ToString("dd-MMM-yyyy"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetStockBoardPrefereredErectionTypeAndDate'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetStockBoardPrefereredRemovalTypeAndDate
        public string GetStockBoardPrefereredRemovalTypeAndDate(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var dd = ctx.SB_Orders.FirstOrDefault(o => o.OrderID == orderID);

                    if (dd == null)
                        return string.Empty;

                    StringBuilder sb = new StringBuilder();
                    if (dd.PreferredRemovalType.HasValue && dd.PreferredRemovalDate.HasValue)
                    {
                        if (dd.PreferredRemovalType.Value == -1)
                            sb.Append("Before or On " + dd.PreferredRemovalDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredRemovalType.Value == 0)
                            sb.Append("On " + dd.PreferredRemovalDate.Value.ToString("dd-MMM-yyyy"));
                        else if (dd.PreferredRemovalType.Value == 1)
                            sb.Append("On or After " + dd.PreferredRemovalDate.Value.ToString("dd-MMM-yyyy"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetStockBoardPrefereredRemovalTypeAndDate'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region PropertySBOrdersHasBoardOrStockBoard
        public bool PropertySBOrdersHasBoardOrStockBoard(int propertyID)
        {
            if (propertyID <= 0)
            {
                throw new ArgumentNullException("propertyID");
            }
            bool ret = false;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var os = (from o in ctx.SB_Orders
                              join po in ctx.PropertySBOrders on o.OrderID equals po.OrderId
                              where po.PropertyId == propertyID
                              select o).ToList();

                    if (os == null)
                        return false;

                    foreach (SB_Order item in os)
                    {
                        if (SBOrderHas(item.OrderID, ProductTypes.BillBoard) || SBOrderHas(item.OrderID, ProductTypes.Stockboard) || SBOrderHas(item.OrderID, ProductTypes.BoardPackages))
                        {
                            ret = true;
                            break;
                        }
                    }
                    return ret;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'PropertySBOrdersHasBoardOrStockBoard'. propertyID:{0}", propertyID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region SBOrderHas
        public bool SBOrderHas(int orderID, int typeID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            bool ret = false;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.SB_Order_To_SB_OrderDetails);
                    loadOptions.Add(EntityRelations.SB_OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    SB_Order od = ctx.SB_Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (od.SB_OrderDetails != null && od.SB_OrderDetails.Count > 0)
                    {
                        foreach (SB_OrderDetail anItem in od.SB_OrderDetails)
                        {
                            if (anItem.Product.TypeID == typeID)
                            {
                                ret = true;
                                break;
                            }
                            else if (anItem.Product.TypeID == ProductTypes.Packages || anItem.Product.TypeID == ProductTypes.BoardPackages || anItem.Product.TypeID == ProductTypes.OtherPackages)
                            {
                                foreach (PackageContentGroup item in anItem.Product.PackageContentGroups)
                                {
                                    foreach (PackageContentGroupProduct contentProductItem in item.PackageContentGroupProducts)
                                    {
                                        Product pro = ctx.Products.SingleOrDefault(p => p.ProductID == contentProductItem.ProductId);
                                        if (pro != null && pro.TypeID == typeID)
                                        {
                                            ret = true;
                                            break;
                                        }

                                    }
                                    if (ret == true)
                                        break;
                                }

                            }
                        }
                    }
                    return ret;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'OrderHas'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #endregion

        #region AddOverlayInstallationFile
        public void AddOverlayInstallationFile(int orderID, string fileName)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderOtherDetail);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);
                    string diagramUploaded = string.Empty;

                    if (System.IO.File.Exists(fileName))
                    {
                        diagramUploaded = " -- " + DateTime.Now.ToString() + " Installation File has been uploaded";

                        if (string.IsNullOrEmpty(od.ErectionNotes))
                            od.ErectionNotes = diagramUploaded;
                        else
                            od.ErectionNotes = od.ErectionNotes + ".\r\n" + diagramUploaded;

                        if (od.OrderOtherDetail != null)
                        {
                            if (string.IsNullOrEmpty(od.OrderOtherDetail.InstallFile))
                                od.OrderOtherDetail.InstallFile = fileName;
                            else
                                od.OrderOtherDetail.InstallFile = od.OrderOtherDetail.InstallFile + ";" + fileName;

                            od.OrderOtherDetail.DateFinishedOverlay = DateTime.Now;
                        }
                        else
                        {
                            od.OrderOtherDetail = new OrderOtherDetail();
                            od.OrderOtherDetail.OrderId = orderID;
                            od.OrderOtherDetail.InstallFile = fileName;
                            od.OrderOtherDetail.DateFinishedOverlay = DateTime.Now;
                            ctx.OrderOtherDetails.InsertOnSubmit(od.OrderOtherDetail);
                        }

                    }

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'AddOverlayInstallationFile'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetProductsSizeCodeFromOrder
        public List<string> GetProductsSizeCodeFromOrder(int orderID, int productType)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (productType <= 0)
            {
                throw new ArgumentNullException("productType");
            }
            try
            {

                List<string> sizeCodes = new List<string>();
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    var ods = (from o in ctx.OrderDetails
                               where o.OrderID == orderID
                               select o).ToList();

                    if (ods != null && ods.Count > 0)
                    {
                        foreach (OrderDetail item in ods)
                        {
                            if (item.Product != null && item.Product.TypeID == productType)
                            {
                                if (!string.IsNullOrEmpty(item.Product.SizeCode))
                                {
                                    sizeCodes.Add(item.Product.SizeCode);
                                }
                            }
                        }

                    }
                    return sizeCodes;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProductsSizeCodeFromOrder'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetRelatedOrdersByPropertyId
        public List<int> GetRelatedOrdersByPropertyId(int propertyId)
        {
            if (propertyId <= 0)
            {
                throw new ArgumentNullException("propertyId");
            }
            try
            {

                List<int> relOrders = new List<int>();
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var os = (from o in ctx.Orders
                              join po in ctx.PropertyOrders on o.OrderID equals po.OrderId
                              join p in ctx.Properties on po.PropertyId equals p.PropertyId
                              join m in ctx.MaterialDetails on o.OrderID equals m.OrderID
                              where p.PropertyId == propertyId && m.TextReceived != null
                              select o).ToList();

                    if (os == null)
                        return relOrders;

                    foreach (Order item in os)
                    {
                        if (NonDIYOrderHas(item.OrderID, ProductTypes.BillBoard) || NonDIYOrderHas(item.OrderID, ProductTypes.Brochure) || NonDIYOrderHas(item.OrderID, ProductTypes.WindowCard))
                        {
                            string xmlFilePath = GetXmlFile(item.OrderID);
                            if (File.Exists(xmlFilePath))
                            {
                                relOrders.Add(item.OrderID);
                            }
                        }
                    }
                    return relOrders;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetRelatedOrdersByPropertyId'. propertyId:{0}", propertyId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region NonDIYOrderHas
        public bool NonDIYOrderHas(int orderID, int typeID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            bool ret = false;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        return false;

                    if (od.OrderDetails != null && od.OrderDetails.Count > 0)
                    {
                        foreach (OrderDetail anItem in od.OrderDetails)
                        {
                            if (anItem.Product.TypeID == typeID && (anItem.UserDesignOnline == null || anItem.UserDesignOnline.Value == false))
                            {
                                ret = true;
                                break;
                            }
                        }
                    }
                    return ret;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'NonDIYOrderHas'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }

        #endregion

        #region ModifyDIYOrder
        public OnlineOrderResponse ModifyDIYTemplate(int orderID, OnlinePropertyOrder propertyOrder)
        {
            var ret = new OnlineOrderResponse();
            try
            {
                if (orderID <= 0)
                {
                    throw new ArgumentNullException("orderID");
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_PropertyOrder);
                    loadOptions.Add(EntityRelations.PropertyOrder_To_Property);
                    loadOptions.Add(EntityRelations.Property_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    var jobDocs = ctx.AOP_JobDocuments.Where(x => x.JobId == orderID).ToList();

                    string templateChanges = string.Empty;

                    bool clientHasStockboardDIY = false;
                    ClientsPref cpHasSBDIY = (from c in ctx.ClientsPrefs
                                              where c.ClientId == propertyOrder.ClientId && c.PrefID == ClientsPref.StockboardDIY
                                              select c).FirstOrDefault();

                    if (cpHasSBDIY != null && cpHasSBDIY.BitValue.HasValue && cpHasSBDIY.BitValue == true)
                        clientHasStockboardDIY = true;

                    foreach (CartItem item in propertyOrder.Cart)
                    {
                        if (item.TypeId == ProductTypes.BillBoard || (item.TypeId == ProductTypes.Stockboard && clientHasStockboardDIY) || (item.TypeId == ProductTypes.StockboardOverlay && clientHasStockboardDIY) || item.TypeId == ProductTypes.Brochure || item.TypeId == ProductTypes.WindowCard || item.TypeId == ProductTypes.Corflute || item.TypeId == ProductTypes.DIYStickers)
                        {
                            int templateProductId = item.SelectedDIYTemplateId;
                            if (templateProductId <= 0)
                                continue;

                            var existingJobDoc = jobDocs.Where(x => x.JobDocumentId == item.JobDocumentID).FirstOrDefault();
                            if (existingJobDoc != null)
                            {
                                if (existingJobDoc.TemplateProductId != item.SelectedDIYTemplateId)
                                {
                                    //move the working job document folder
                                    try
                                    {
                                        string workingPath = String.Format(@"{0}\{1}\{2}\{3}", ServiceConfig.AOP_TEMPLATE_ROOT_DIR, propertyOrder.ClientId, orderID, existingJobDoc.JobDocumentId);

                                        //New AOP Modify DIY folder
                                        string destPath = String.Format(@"{0}\{1}\{2}\{3}", ServiceConfig.AOP_WORKING_DIR_MODIFY_ORDER, propertyOrder.ClientId, orderID, existingJobDoc.JobDocumentId);

                                        if (!Directory.Exists(destPath))
                                        {
                                            Directory.CreateDirectory(destPath);
                                        }

                                        //Move all the files
                                        foreach (string newPath in Directory.GetFiles(workingPath, "*.*",
                                            SearchOption.AllDirectories))
                                            File.Copy(newPath, newPath.Replace(workingPath, destPath), true);

                                        // After moving the files, delete the folder.
                                        try
                                        {
                                            if (Directory.Exists(workingPath))
                                            {
                                                Directory.Delete(workingPath, true);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Exception(ex, workingPath);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string message = string.Format("Error occured in moving the working job document folder on function 'ModifyDIYTemplate '. orderID:{0}", orderID);
                                        Logger.Exception(ex, message);
                                    }

                                    //move the completed folder
                                    try
                                    {
                                        string workingPath = String.Format(@"{0}\{1}", ServiceConfig.AOP_DOC_OUTPUT_DIR, orderID);

                                        if (Directory.Exists(workingPath))
                                        {
                                            //New AOP Modify DIY completed folder
                                            string destPath = String.Format(@"{0}\{1}", ServiceConfig.AOP_WORKING_DIR_MODIFY_ORDER_COMPLETED, orderID);

                                            if (!Directory.Exists(destPath))
                                                Directory.CreateDirectory(destPath);

                                            //Move all the files
                                            foreach (string newPath in Directory.GetFiles(workingPath, orderID + "_" + existingJobDoc.JobDocumentId + "*.*",
                                                SearchOption.AllDirectories))
                                            {
                                                File.Copy(newPath, newPath.Replace(workingPath, destPath), true);
                                                File.Delete(newPath);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        string message = string.Format("Error occured in moving the completed folder on function 'ModifyDIYTemplate '. orderID:{0}", orderID);
                                        Logger.Exception(ex, message);
                                    }

                                    //update template ProductId, StatusID, Template Name, Template original file name, Template Date modified stamp
                                    List<EntityRelations> options = new List<EntityRelations>();
                                    options.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
                                    options.Add(EntityRelations.AOP_TemplateProduct_To_AOP_DocumentPolicy);

                                    AOPService aopService = new AOPService();
                                    AOP_TemplateProduct templateProduct = aopService.GetTemplateProductById(templateProductId, options);

                                    existingJobDoc.TemplateProductId = templateProductId;
                                    existingJobDoc.StatusId = 0;
                                    existingJobDoc.TemplateName = templateProduct.Name;
                                    if (!string.IsNullOrEmpty(templateProduct.Description))
                                    {
                                        existingJobDoc.TemplateName += " " + templateProduct.Description;
                                    }
                                    string originalFileNameWithoutExt = Path.GetFileNameWithoutExtension(templateProduct.AOP_Template.TemplatePath);
                                    //string originalFileNameWithoutExt = existingJobDoc.TemplateName.Replace("/", " ");
                                    string ext = Path.GetExtension(templateProduct.AOP_Template.TemplatePath);

                                    string newTemplateFileName = string.Empty;
                                    if (!string.IsNullOrEmpty(originalFileNameWithoutExt) && originalFileNameWithoutExt.Contains("Smart") && !string.IsNullOrEmpty(templateProduct.Name))
                                    {
                                        newTemplateFileName = string.Format("{0}_{1}_{2}{3}", templateProduct.Name.Replace("/", " "), originalFileNameWithoutExt, GetDocumentTypeSuffix(templateProduct.Type), ext);
                                    }
                                    else
                                    {
                                        newTemplateFileName = string.Format("{0}_{1}{2}", originalFileNameWithoutExt, GetDocumentTypeSuffix(templateProduct.Type), ext);
                                    }

                                    templateChanges = templateChanges + "; Change From: " + existingJobDoc.TemplateOriginalFileName + ", Change To: " + newTemplateFileName;

                                    existingJobDoc.TemplateOriginalFileName = newTemplateFileName;
                                    existingJobDoc.TemplateDateModifiedStamp = templateProduct.AOP_Template.DateModified.HasValue ? templateProduct.AOP_Template.DateModified.Value : templateProduct.AOP_Template.DateCreated;

                                    //update document model
                                    string documentModel = templateProduct.AOP_Template.TemplateModel.ToString();
                                    string policyModel = templateProduct.AOP_DocumentPolicy.PolicyModel.ToString();
                                    Dom.Document workingDoc = FeedContents(propertyOrder, od.PropertyOrder.Property, od.Client, templateProduct, policyModel, documentModel, orderID);
                                    workingDoc.ImageRequirement = new Dom.ImageQuality()
                                    {
                                        MinimumMegaPixels = Convert.ToDecimal(templateProduct.MinimumMegaPixels),
                                        RecommendedMegaPixels = Convert.ToDecimal(templateProduct.RecommendedMegaPixels)
                                    };

                                    Dom.Document oldDocument;
                                    if (existingJobDoc.TempDocumentModel == null)
                                    {
                                        oldDocument = Dom.Document.Deserialize(existingJobDoc.WorkingDocumentModel.ToString());
                                    }
                                    else
                                    {
                                        oldDocument = Dom.Document.Deserialize(existingJobDoc.TempDocumentModel.ToString());
                                    }

                                    workingDoc.CopyFormFieldValuesFrom(oldDocument);
                                    string workingDocumentModel = Dom.Document.Serialize(workingDoc);
                                    existingJobDoc.WorkingDocumentModel = System.Xml.Linq.XElement.Parse(workingDocumentModel);
                                    existingJobDoc.TempDocumentModel = System.Xml.Linq.XElement.Parse(workingDocumentModel);

                                    //update OrderDetails
                                    OrderDetail odDetail = ctx.OrderDetails.SingleOrDefault(o => o.OrderDetailsID == existingJobDoc.OrderDetailsID);
                                    odDetail.Layout = templateProduct.Format;

                                    string sourceTemplateFile = AOP_Template.GetTemplatePath(templateProduct.TemplateId);
                                    sourceTemplateFile = Path.Combine(OnlineBLConfig.AOP_TEMPLATE_ROOT_DIR.TrimEnd('\\'), sourceTemplateFile);

                                    string destTemplatePath = GetJobDocumentPath(OnlineBLConfig.AOP_TEMPLATE_ROOT_DIR,
                                        propertyOrder.ClientId, orderID, existingJobDoc.JobDocumentId);
                                    string destTemplateFile = GetJobDocumentTemplatePath(OnlineBLConfig.AOP_TEMPLATE_ROOT_DIR,
                                        propertyOrder.ClientId, orderID, existingJobDoc.JobDocumentId);

                                    if (!Directory.Exists(destTemplatePath))
                                        Directory.CreateDirectory(destTemplatePath);

                                    File.Copy(sourceTemplateFile, destTemplateFile, true);

                                }
                            }

                        }
                    }

                    if (!string.IsNullOrEmpty(templateChanges))
                    {
                        if (templateChanges.StartsWith("; "))
                        {
                            templateChanges = templateChanges.Substring(2);

                            //ROnlineBLe an Event to send email notification to Admin
                            int eventID = EventSettings.ChangeOrderTemplates;
                            string sub = "Change Order Templates";
                            string xmlData = @"
									<EVENT>
									<OrderID>" + orderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.PropertyOrder.Property.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                    <Email>" + od.SendProofTo.Replace("&", "&amp;") + @"</Email>
									<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
									<ChangeDetails>" + templateChanges.Replace("&", "&amp;") + @"</ChangeDetails>
									</EVENT>";

                            string textData = @"Notification:" +
                                                    Environment.NewLine +
                                                    @"Request Template Changes." +
                                                    Environment.NewLine +
                                                    @"Job Id: " + orderID +
                                                    Environment.NewLine +
                                                    @"ChangeDetails: " + templateChanges;

                            string source = "OnlineBL_OrderService_ModifyDIYTemplate";

                            ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, null, null, source, String.Empty);

                        }
                    }
                    ctx.SubmitChanges();
                    ret.PropertyId = propertyOrder.PropertyId;
                    ret.OrderId = orderID;
                }
            }
            catch (Exception ex)
            {
                ret.OrderHasError = true;
                string message = string.Format("Error occured in 'ModifyDIYTemplate'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

            return ret;
        }

        public Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ModifyDIYOrder(int orderID, int clientID, Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, string managerID, bool isWorkshop)
        {
            bool isPackageProductExist = false;
            Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ret = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse();
            try
            {
                if (orderID <= 0)
                {
                    throw new ArgumentNullException("orderID");
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (od == null)
                        throw new ArgumentNullException("orderID does not exist");
                    else
                    {
                        if (od.ProofDetail.DateApproved.HasValue)
                        {
                            throw new ArgumentNullException("Order Already Approved");
                        }
                        else if (!od.OrderDetails.Any(odt => odt.UserDesignOnline == true))
                        {
                            throw new ArgumentNullException("Order is not DIY");
                        }

                        if (od.OrderDetails.Any(odt => odt.Product.CategoryId == CategoryTypes.Packages))
                        {
                            isPackageProductExist = true;
                        }
                    }
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();

                        List<EntityRelations> loadOptions = new List<EntityRelations>();
                        loadOptions.Add(EntityRelations.Client_To_ClientsDisplayInfo);
                        loadOptions.Add(EntityRelations.Property_To_Location);
                        loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                        loadOptions.Add(EntityRelations.Order_To_ProofDetail);

                        ctx.DeferredLoadingEnabled = false;
                        ctx.SetDataLoadOptions(loadOptions);

                        //Split the Cart into NotChanged List and Changed List
                        List<CartItem> newProducts = propertyOrder.Cart.FindAll(i => i.IsOldProduct == false).ToList();
                        List<CartItem> oldProducts = propertyOrder.Cart.FindAll(i => i.IsOldProduct == true).ToList();

                        //try to update item qty
                        foreach (var item in oldProducts)
                        {
                            OrderDetail od = ctx.OrderDetails.SingleOrDefault(o => o.OrderDetailsID == item.OrderDetailsID);
                            if (od != null)
                            {
                                od.Qty = item.ItemQty;
                                od.SubTotal = od.Qty * od.Price;
                                od.GST = od.SubTotal * (decimal)0.1;
                                od.Total = od.SubTotal + od.GST;

                                ctx.SubmitChanges();
                            }
                        }

                        List<int> odIDs = oldProducts.Select(i => i.OrderDetailsID).ToList();

                        bool clientHasStockboardDIY = false;
                        ClientsPref cpHasSBDIY = (from c in ctx.ClientsPrefs
                                                  where c.ClientId == propertyOrder.ClientId && c.PrefID == Abc.OnlineBL.Entities.Utility.Preferences.StockboardDIY
                                                  select c).FirstOrDefault();

                        if (cpHasSBDIY != null && cpHasSBDIY.BitValue.HasValue && cpHasSBDIY.BitValue == true)
                            clientHasStockboardDIY = true;

                        //Remove all product from OrderDetail by OrderId-----------------------
                        //join products to check type in billboard, brochure, window card
                        List<OrderDetail> ods = (from od in ctx.OrderDetails
                                                 join pd in ctx.Products on od.ProductID equals pd.ProductID
                                                 where od.OrderID == orderID
                                                 && (pd.TypeID == ProductTypes.BillBoard || (pd.TypeID == ProductTypes.StockboardOverlay && clientHasStockboardDIY) || pd.TypeID == ProductTypes.Brochure
                                                    || pd.TypeID == ProductTypes.WindowCard || pd.CategoryId == CategoryTypes.Overlays || pd.CategoryId == CategoryTypes.MiscellaneousProducts || pd.TypeID == ProductTypes.Corflute
                                                    || pd.TypeID == ProductTypes.DIYStickers)
                                                 select od).ToList();


                        if (ods != null && ods.Count > 0)
                        {
                            foreach (var item in ods)
                            {
                                if (!odIDs.Contains(item.OrderDetailsID))
                                {
                                    ctx.OrderDetails.DeleteOnSubmit(item);
                                }
                            }
                            ctx.SubmitChanges();
                        }
                        //------------------------------------------------------------------------------

                        //Remove all aop_jobdocument by OrderId------------------------------------
                        List<AOP_JobDocument> aops = (from aop in ctx.AOP_JobDocuments
                                                      where aop.JobId == orderID
                                                      select aop).ToList();

                        List<AOP_JobDocument> aopToRemove = new List<AOP_JobDocument>();
                        if (aops != null && aops.Count > 0)
                        {
                            foreach (var item in aops)
                            {
                                if (!odIDs.Contains(item.OrderDetailsID.Value))
                                {
                                    aopToRemove.Add(item);
                                    ctx.AOP_JobDocuments.DeleteOnSubmit(item);
                                }
                            }
                            ctx.SubmitChanges();
                        }

                        //---------------------------------------------------------------------------------

                        //Remove all AOP folder by clientId + OrderId + JobdocumentID -------------------------
                        //back it up somewhere in case we rollback
                        IFile file = VirtualFileSystemFactory.GetFile();

                        try
                        {
                            foreach (var item in aopToRemove)
                            {
                                string workingPath = String.Format(@"{0}\{1}\{2}\{3}", ServiceConfig.AOP_TEMPLATE_ROOT_DIR, clientID, orderID, item.JobDocumentId);

                                //New AOP Modify DIY folder
                                string destPath = String.Format(@"{0}\{1}\{2}\{3}", ServiceConfig.AOP_WORKING_DIR_MODIFY_ORDER, clientID, orderID, item.JobDocumentId);

                                if (!Directory.Exists(destPath))
                                {
                                    file.CreateDir(destPath);
                                    //Directory.CreateDirectory(destPath);
                                }

                                //Move all the files
                                foreach (string newPath in Directory.GetFiles(workingPath, "*.*",
                                    SearchOption.AllDirectories))
                                    file.Copy(newPath, newPath.Replace(workingPath, destPath), true);
                                //File.Move(newPath, newPath.Replace(workingPath, destPath));

                                // After moving the files, delete the folder.
                                try
                                {
                                    if (file.ExistsDir(workingPath))
                                    {
                                        file.DeleteDir(workingPath, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, workingPath);
                                    //throw ex; //might not need as get job by aop_jobdocument
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format("Error occured in 'ModifyDIYOrder '. orderID:{0}", orderID);
                            Logger.Exception(ex, message);
                            //throw ex;  //might not need as get job by aop_jobdocument
                        }


                        //move complete folder
                        try
                        {
                            string workingPath = String.Format(@"{0}\{1}", ServiceConfig.AOP_DOC_OUTPUT_DIR, orderID);

                            if (file.ExistsDir(workingPath))
                            {
                                foreach (var item in aopToRemove)
                                {
                                    //New AOP Modify DIY completed folder
                                    string destPath = String.Format(@"{0}\{1}", ServiceConfig.AOP_WORKING_DIR_MODIFY_ORDER_COMPLETED, orderID);

                                    if (!file.ExistsDir(destPath))
                                        file.CreateDir(destPath);

                                    //Move all the files
                                    foreach (string newPath in Directory.GetFiles(workingPath, orderID + "_" + item.JobDocumentId + "*.*",
                                        SearchOption.AllDirectories))
                                    {
                                        file.Copy(newPath, newPath.Replace(workingPath, destPath), true);
                                        file.Delete(newPath);
                                        //File.Move(newPath, newPath.Replace(workingPath, destPath));
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            string message = string.Format("Error occured in 'ModifyDIYOrder '. orderID:{0}", orderID);
                            Logger.Exception(ex, message);
                        }

                        //Add all product from cart to OrderDetail
                        InsertProducts(newProducts, propertyOrder, managerID, orderID, ctx, isWorkshop);

                        if (isPackageProductExist)
                        {
                            Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);
                            foreach (var item in od.OrderDetails)
                            {
                                item.Price = item.Discount = item.SubTotal = item.GST = item.Total = 0;
                            }
                        }

                        ctx.SubmitChanges();

                        //Add all templates into AOP_JobDocument
                        //Add all template folder to AOP path
                        Property property = (from p in ctx.Properties
                                             where p.PropertyId == propertyOrder.PropertyId
                                             select p).FirstOrDefault();


                        Client client = (from c in ctx.Clients
                                         where c.ClientID == clientID
                                         select c).FirstOrDefault();
                        ProcessAOP(orderID, propertyOrder, property, client, newProducts);

                        //Generate event
                        //Build up OrderDataExchange before pass it on to FormatOrder
                        Abc.OnlineBL.Orders.Workflow.OrderDataExchange orderDataExchange = new Abc.OnlineBL.Orders.Workflow.OrderDataExchange();
                        orderDataExchange.PropertyOrder = propertyOrder;
                        orderDataExchange.OrderId = orderID;

                        orderDataExchange.PropertyOrder.Property = property;

                        orderDataExchange.Client = client;

                        Abc.OnlineBL.Orders.Workflow.ClientInfo clientInfo = new Abc.OnlineBL.Orders.Workflow.ClientInfo();
                        clientInfo.ClientID = orderDataExchange.Client.ClientID;
                        clientInfo.ClientName = orderDataExchange.Client.ClientName;
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

                        Abc.OnlineBL.Service.Implementation.Utility.OnlineOder.FormatOrder formatOrder = new Abc.OnlineBL.Service.Implementation.Utility.OnlineOder.FormatOrder(orderDataExchange, orderID);
                        Abc.OnlineBL.Service.Implementation.Utility.OnlineOder.HtmlFormats html = formatOrder.GetHtmlFileContents();
                        string sub = "Modify DIY order: " + orderDataExchange.PropertyOrder.Property.PropertyAddressWithSuburb + " - Order No: " + orderID;
                        if (isPackageProductExist)
                            sub = "Need to Calculate Pack Pricing -- " + sub;

                        //Create new event to notify Clients
                        ctx.SP_EventQueueAdd(EventSettings.ModifyDIYOrder, sub, html.ForClient, sub, orderID, clientID, null, null, "OnlineBL_OrderService_ModifyDIYOrder", string.Empty);

                        //Create new event to notify ABC
                        ctx.SP_EventQueueAdd(EventSettings.ModifyDIYOrderInternal, sub, html.ForAbc, sub, orderID, clientID, null, null, "OnlineBL_OrderService_ModifyDIYOrder", string.Empty);

                        ctx.Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        Logger.Exception(ex, "Error occured in 'ModifyDIYOrder'.");
                        throw ex;
                    }
                    ret.PropertyId = propertyOrder.PropertyId;
                    ret.OrderId = orderID;
                }
            }
            catch (Exception ex)
            {
                ret.OrderHasError = true;
                string message = string.Format("Error occured in 'ModifyDIYOrder'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
            return ret;
        }

        private void InsertProducts(List<CartItem> cart, Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, string managerId, int orderId, AbcDataContext ctx, bool isWorkshop)
        {
            foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item in cart)
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

        private int InsertProductIntoOrderDetails(AbcDataContext ctx, int clientId,
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

        private void ProcessAOP(int orderId, OnlinePropertyOrder propertyOrder, Property property, Client client, List<CartItem> cart)
        {
            List<AOP_JobDocument> jobDocuments = new List<AOP_JobDocument>();
            foreach (CartItem item in cart)
            {
                if (item.TypeId == ProductTypes.BillBoard || item.TypeId == ProductTypes.StockboardOverlay || item.TypeId == ProductTypes.Brochure || item.TypeId == ProductTypes.WindowCard || item.TypeId == ProductTypes.Corflute || item.TypeId == ProductTypes.DIYStickers)
                {
                    int templateProductId = item.SelectedDIYTemplateId;
                    if (templateProductId <= 0)
                        continue;

                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
                    options.Add(EntityRelations.AOP_TemplateProduct_To_AOP_DocumentPolicy);

                    // If service can't return templateProduct exception will be thrown from the service.
                    AOPService aopService = new AOPService();
                    AOP_TemplateProduct templateProduct = aopService.GetTemplateProductById(templateProductId, options);

                    string documentModel = templateProduct.AOP_Template.TemplateModel.ToString();
                    string policyModel = templateProduct.AOP_DocumentPolicy.PolicyModel.ToString();
                    Dom.Document workingDoc = FeedContents(propertyOrder, property, client, templateProduct, policyModel, documentModel, orderId);
                    workingDoc.ImageRequirement = new Dom.ImageQuality()
                    {
                        MinimumMegaPixels = Convert.ToDecimal(templateProduct.MinimumMegaPixels),
                        RecommendedMegaPixels = Convert.ToDecimal(templateProduct.RecommendedMegaPixels)
                    };
                    string workingDocumentModel = Dom.Document.Serialize(workingDoc);

                    AOP_JobDocument jobDocument = new AOP_JobDocument();
                    jobDocument.SetAsChangeTrackingRoot(EntityState.New);
                    jobDocument.JobId = orderId;
                    jobDocument.TemplateProductId = templateProductId;
                    jobDocument.StatusId = 0;
                    jobDocument.WorkingDocumentModel = System.Xml.Linq.XElement.Parse(workingDocumentModel);
                    jobDocument.TemplateName = templateProduct.Name;
                    jobDocument.OrderDetailsID = item.OrderDetailsID;
                    if (!string.IsNullOrEmpty(templateProduct.Description))
                    {
                        jobDocument.TemplateName += " " + templateProduct.Description;
                    }

                    // Here we include the document suffix like _B/_BR/_O for the AJPS to eventually pick the files for
                    // automatic printing
                    string originalFileNameWithoutExt = Path.GetFileNameWithoutExtension(templateProduct.AOP_Template.TemplatePath);
                    string ext = Path.GetExtension(templateProduct.AOP_Template.TemplatePath);

                    if (!string.IsNullOrEmpty(originalFileNameWithoutExt) && originalFileNameWithoutExt.Contains("Smart") && !string.IsNullOrEmpty(templateProduct.Name))
                    {
                        jobDocument.TemplateOriginalFileName = string.Format("{0}_{1}_{2}{3}", templateProduct.Name.Replace("/", " "), originalFileNameWithoutExt, GetDocumentTypeSuffix(templateProduct.Type), ext);
                    }
                    else
                    {
                        jobDocument.TemplateOriginalFileName = string.Format("{0}_{1}{2}", originalFileNameWithoutExt, GetDocumentTypeSuffix(templateProduct.Type), ext);
                    }

                    jobDocument.TemplateDateModifiedStamp = templateProduct.AOP_Template.DateModified.HasValue ? templateProduct.AOP_Template.DateModified.Value : templateProduct.AOP_Template.DateCreated;
                    jobDocument.SetAsInsertOnSubmit();

                    // Add to insert later as a batch. Because we don't handle transaction here.
                    jobDocuments.Add(jobDocument);
                }
            }

            jobDocuments = UpdateJobDocument(jobDocuments);
        }

        #endregion

        #region FeedContents
        private Dom.Document FeedContents(OnlinePropertyOrder propertyOrder, Property pro, Client client, AOP_TemplateProduct templateProduct, string policyModel, string documentModel, int orderID)
        {
            if (string.IsNullOrEmpty(documentModel)) return null;

            Dom.Document doc = Dom.Document.Deserialize(documentModel);
            if (doc == null) return null;

            Abc.OnlineBL.Entities.Property property = pro;
            Abc.OnlineBL.Entities.Client clientRow = client;

            try
            {
                if (!string.IsNullOrEmpty(policyModel))
                {
                    Dom.DocumentPolicy policy = Dom.DocumentPolicy.Deserialize(policyModel);
                    if (policy != null)
                        doc.MergeWithPolicy(policy);
                }

                FeedFormElement(doc, "glgr_SaleType", "Q_SaleType_" + propertyOrder.CommonDetails.SaleType);

                FeedContent(doc.Root, "OrderId", orderID.ToString());
                FeedContent(doc.Root, "ClientId", propertyOrder.ClientId.ToString());
                FeedContent(doc.Root, "ClientName", clientRow.ClientName);
                FeedContent(doc.Root, "ClientOffice", clientRow.Office);

                if (clientRow.Address != null) FeedContent(doc.Root, "ClientAddress", clientRow.Address);
                if (clientRow.Suburb != null) FeedContent(doc.Root, "ClientSuburb", clientRow.Suburb);
                if (clientRow.State != null) FeedContent(doc.Root, "ClientState", clientRow.State);
                if (clientRow.PostCode != null) FeedContent(doc.Root, "ClientPostCode", clientRow.PostCode);
                if (clientRow.Phone != null) FeedContent(doc.Root, "ClientPhone", clientRow.Phone);
                if (clientRow.Fax != null) FeedContent(doc.Root, "ClientFax", clientRow.Fax);
                if (clientRow.Email != null) FeedContent(doc.Root, "ClientEmail", clientRow.Email);
                if (clientRow.LicenceNo != null) FeedContent(doc.Root, "ClientLicNo", clientRow.LicenceNo);

                FeedContent(doc.Root, "UnitNo", property.UnitNo);
                FeedContent(doc.Root, "PropertyAddress", property.PropertyAddressWithSuburb);
                FeedContent(doc.Root, "PostCode", property.Location.PostCode);
                FeedContent(doc.Root, "State", property.Location.State);
                FeedContent(doc.Root, "StreetName", property.StreetName);
                FeedContent(doc.Root, "StreetNo", property.StreetNo);
                FeedContent(doc.Root, "Suburb", property.Location.Location1);

                if (!string.IsNullOrEmpty(propertyOrder.CommonDetails.SaleType))
                {
                    FeedContent(doc.Root, "SaleType", propertyOrder.CommonDetails.SaleType);
                    FeedContent(doc.Root, "AuctionDate", propertyOrder.CommonDetails.AuctionDate +
                                " at " + propertyOrder.CommonDetails.AuctionTime);
                }

                FeedContent(doc.Root, "ClientsReferenceId", propertyOrder.ClientRefNumber);

                if (templateProduct.ContentType == "Colour Front/Back")
                {
                    ProcessLayers(doc, "CT_ColourColour", "CT_CC", true);
                    ProcessLayers(doc, "CT_ColourMono", "CT_CM", false);
                }
                else if (templateProduct.ContentType == "Colour Front/Mono Back")
                {
                    ProcessLayers(doc, "CT_ColourColour", "CT_CC", false);
                    ProcessLayers(doc, "CT_ColourMono", "CT_CM", true);
                }
                else if (templateProduct.ContentType == "Colour Front")
                {
                    ProcessLayers(doc, "CT_ColourColour", "CT_CC", false);
                    ProcessLayers(doc, "CT_ColourMono", "CT_CM", false);
                    if (doc.Pages != null && doc.Pages.Count == 2)
                    {
                        // The page number is the page name.
                        Dom.Page page = doc.GetPageByName("2");
                        if (page != null)
                            page.Visible = false;
                    }
                }

                doc.Name = templateProduct.SizeCode;
            }
            catch (Exception ex)
            {
                string mes = string.Format("Policy {0} null, ", string.IsNullOrEmpty(policyModel) ? "is" : "is not");
                mes += string.Format("Document {0} null.", string.IsNullOrEmpty(documentModel) ? "is" : "is not");
                Logger.Exception(ex, mes);
            }

            return doc;
        }

        private void FeedContent(Dom.Tag root, string tagName, string aValue)
        {
            List<Dom.Tag> tags = root.FindAll(delegate(Dom.Tag t) { return (t.TagName == tagName); });
            foreach (Dom.Tag tag in tags)
            {
                if (tag.FormElement == null) continue;

                if (string.IsNullOrEmpty(aValue))
                    tag.FormElement.Value = string.Empty;
                else
                    tag.FormElement.Value = aValue;
            }
        }

        private void FeedFormElement(Dom.Document doc, string formId, string aValue)
        {
            List<Dom.FormElement> fes = doc.FormElements.FindAll(delegate(Dom.FormElement fe) { return (fe.Id == formId); });
            foreach (Dom.FormElement fe in fes)
            {
                fe.Value = aValue;
            }
        }

        private void ProcessLayers(Dom.Document doc, string layerName, string alternateLayerName, bool visible)
        {
            Dom.Layer layer = doc.GetLayerByNameStartsWith(layerName);
            if (layer == null)
                layer = doc.GetLayerByNameStartsWith(alternateLayerName);

            if (layer != null)
                layer.Visible = visible;
        }
        #endregion

        #region GetDocumentType
        private static string GetDocumentTypeSuffix(string type)
        {
            if (string.Compare(type, "Billboard", true) == 0 || string.Compare(type, "Stockboard", true) == 0)
                return "B";
            else if (string.Compare(type, "Brochure", true) == 0 ||
                string.Compare(type, "Window Card", true) == 0)
                return "Br";
            else if (string.Compare(type, "Stockboard Overlay", true) == 0)
                return "OL";
            else if (string.Compare(type, "DIY Stickers", true) == 0)
                return "OL";
            else
                return "O";
        }
        #endregion

        #region UpdateJobDocument
        private static List<AOP_JobDocument> UpdateJobDocument(List<AOP_JobDocument> jobs)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    var newJobs = (from j in jobs
                                   where j.LINQEntityState == EntityState.New
                                   select j).ToList();

                    jobs.ForEach(jd => jd.SynchroniseWithDataContext(ctx));

                    ctx.SubmitChanges();

                    foreach (var newJob in newJobs)
                    {
                        var templateDetails = (from jd in ctx.AOP_JobDocuments
                                               where jd.JobDocumentId == newJob.JobDocumentId
                                               select new { OrderId = jd.SafeJobId, ClientId = jd.Order.ClientID, TemplateId = jd.AOP_TemplateProduct.TemplateId }).FirstOrDefault();

                        string sourceTemplateFile = AOP_Template.GetTemplatePath(templateDetails.TemplateId);
                        sourceTemplateFile = Path.Combine(OnlineBLConfig.AOP_TEMPLATE_ROOT_DIR.TrimEnd('\\'), sourceTemplateFile);

                        string destTemplatePath = GetJobDocumentPath(OnlineBLConfig.AOP_TEMPLATE_ROOT_DIR,
                            templateDetails.ClientId, templateDetails.OrderId, newJob.JobDocumentId);
                        string destTemplateFile = GetJobDocumentTemplatePath(OnlineBLConfig.AOP_TEMPLATE_ROOT_DIR,
                            templateDetails.ClientId, templateDetails.OrderId, newJob.JobDocumentId);

                        IFile file = VirtualFileSystemFactory.GetFile();

                        if (!file.ExistsDir(destTemplatePath))
                            file.CreateDir(destTemplatePath);

                        file.Copy(sourceTemplateFile, destTemplateFile, true);
                    }

                    return jobs;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "UpdateJobDocument");
                throw;
            }
        }

        private static string GetJobDocumentPath(string documentRootPath, int clientId, int jobId, int jobDocumentId)
        {
            string templateFilePath = string.Format("{0}\\{1}\\{2}\\{3}", documentRootPath.TrimEnd('\\'), clientId, jobId, jobDocumentId);
            return templateFilePath;
        }

        private static string GetJobDocumentTemplatePath(string documentRootPath, int clientId, int jobId, int jobDocumentId)
        {
            string templateFile = Path.Combine(GetJobDocumentPath(documentRootPath, clientId, jobId, jobDocumentId), jobDocumentId + ".indt");
            return templateFile;
        }
        #endregion

        #region GetDIYOrder
        public OnlinePropertyOrder GetDIYOrder(int orderID)
        {
            try
            {
                if (orderID <= 0)
                {
                    throw new ArgumentNullException("orderID");
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_PropertyOrder);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_ProofDetail);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                    loadOptions.Add(EntityRelations.Product_To_ProductType);
                    loadOptions.Add(EntityRelations.Product_To_ProductSizeCode);
                    loadOptions.Add(EntityRelations.Product_To_OnlineOrderCategory);
                    loadOptions.Add(EntityRelations.Product_To_PriceListDetails);
                    loadOptions.Add(EntityRelations.PriceListDetail_To_PriceList);

                    loadOptions.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);

                    loadOptions.Add(EntityRelations.Order_To_AOP_JobDocuments);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    List<OrderDetail> odd = new List<OrderDetail>();

                    if (od == null)
                        throw new ArgumentNullException("orderID does not exist");
                    else
                    {
                        if (od.ProofDetail.DateApproved.HasValue)
                        {
                            throw new ArgumentNullException("Order Already Approved");
                        }
                        else if (!od.OrderDetails.Any(odt => odt.UserDesignOnline == true))
                        {
                            throw new ArgumentNullException("Order is not DIY");
                        }

                        bool clientHasStockboardDIY = false;
                        ClientsPref cpHasSBDIY = (from c in ctx.ClientsPrefs
                                                  where c.ClientId == od.ClientID && c.PrefID == Abc.OnlineBL.Entities.Utility.Preferences.StockboardDIY
                                                  select c).FirstOrDefault();

                        if (cpHasSBDIY != null && cpHasSBDIY.BitValue.HasValue && cpHasSBDIY.BitValue == true)
                            clientHasStockboardDIY = true;

                        if (clientHasStockboardDIY)
                        {
                            odd = od.OrderDetails.Where(i => !IsProductApproved(i.Product.TypeID, i.Order.ProofDetail) && (i.UserDesignOnline == true
                                && (i.Product.TypeID == ProductTypes.BillBoard || i.Product.TypeID == ProductTypes.Stockboard || i.Product.TypeID == ProductTypes.StockboardOverlay || i.Product.TypeID == ProductTypes.Brochure || i.Product.TypeID == ProductTypes.WindowCard || i.Product.TypeID == ProductTypes.DIYStickers)
                                            || (i.Product.CategoryId == CategoryTypes.Overlays || i.Product.CategoryId == CategoryTypes.MiscellaneousProducts || i.Product.TypeID == ProductTypes.Corflute))).ToList();
                            if (odd.Count == 0)
                            {
                                throw new ArgumentNullException("Order does not have BillBoard, Brochure or WindowCard product");
                            }
                        }
                        else
                        {
                            odd = od.OrderDetails.Where(i => !IsProductApproved(i.Product.TypeID, i.Order.ProofDetail) && (i.UserDesignOnline == true
                            && (i.Product.TypeID == ProductTypes.BillBoard || i.Product.TypeID == ProductTypes.Brochure || i.Product.TypeID == ProductTypes.WindowCard || i.Product.TypeID == ProductTypes.DIYStickers)
                                        || (i.Product.CategoryId == CategoryTypes.Overlays || i.Product.CategoryId == CategoryTypes.MiscellaneousProducts || i.Product.TypeID == ProductTypes.Corflute))).ToList();
                            if (odd.Count == 0)
                            {
                                throw new ArgumentNullException("Order does not have BillBoard, Brochure or WindowCard product");
                            }
                        }
                    }

                    List<AOP_TemplateProduct> tempTP = (from tm in ctx.AOP_TemplateProducts
                                                        where tm.AOP_Template.ClientId == od.ClientID && tm.AOP_Template.GroupId == od.Client.GroupId
                                                        select tm).ToList();

                    List<AOP_TemplateProduct> corporateTemplates = (from tm in ctx.AOP_TemplateProducts
                                                                    where tm.AOP_Template.GroupId == od.Client.GroupId && tm.AOP_Template.ClientId == null
                                                                    select tm).ToList();

                    if (tempTP != null && tempTP.Count() == 0) //if we don't have any client level template then look at group level
                    {
                        tempTP = corporateTemplates;
                    }

                    OnlinePropertyOrder oPO = new OnlinePropertyOrder();
                    oPO.PropertyId = od.PropertyOrder.PropertyId;
                    oPO.ClientId = od.ClientID;
                    oPO.IsDIYOrder = true;
                    oPO.OrderType = OrderType.DIY;
                    oPO.CartCreatedOn = od.CreatedOn.Value;

                    //iterate to add cart item
                    foreach (var ord in odd)
                    {
                        //Build up OnlineProduct first
                        OnlineProductHelper helper = new OnlineProductHelper();
                        OnlineProduct model = helper.SettingupOnlineProduct(od.Client.GroupId, od.ClientID, ctx, ord.Product, new OnlineProduct(), true, od.Client.PriceListID, false, tempTP, corporateTemplates);

                        CartItem item = new CartItem(oPO.GetNextCartId(), model, ord.Qty);
                        item.IsOldProduct = true;
                        item.OrderDetailsID = ord.OrderDetailsID;

                        var aopJobDo = od.AOP_JobDocuments.FirstOrDefault(a => a.OrderDetailsID == ord.OrderDetailsID);
                        if (aopJobDo != null && aopJobDo.TemplateProductId.HasValue)
                        {
                            item.SelectedDIYTemplateId = aopJobDo.TemplateProductId.Value;
                            item.PreviousSelectedDIYTemplateId = aopJobDo.TemplateProductId.Value;
                            item.JobDocumentID = aopJobDo.JobDocumentId;
                        }

                        oPO.Cart.Add(item);

                    }
                    return oPO;
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ModifyDIYOrder'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        private bool IsProductApproved(int productTypeId, ProofDetail proof)
        {
            var flag = false;
            switch (productTypeId)
            {
                case ProductTypes.BillBoard:
                case ProductTypes.StockboardOverlay:
                    flag = proof.DateAppBoards.HasValue;
                    break;
                case ProductTypes.Brochure:
                case ProductTypes.WindowCard:
                    flag = proof.DateAppBrochures.HasValue;
                    break;
                default:
                    flag = proof.DateAppOthers.HasValue;
                    break;
            }
            return flag;
        }
        #endregion

        #region SetOrderPrintedOn
        public void SetOrderPrintedOn(int orderId, DateTime? printedOn)
        {
            try
            {
                if (orderId <= 0)
                {
                    throw new ArgumentNullException("orderID");
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    var despatch = ctx.DespatchDetails.SingleOrDefault(o => o.OrderID == orderId);

                    if (despatch == null)
                        throw new ArgumentNullException("orderID does not exist");
                    else
                    {
                        despatch.PrintedOn = printedOn;
                        ctx.SubmitChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'SetOrderPrintedOn'. orderID:{0}", orderId);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GetOrderData
        public string GetOrderData(int orderID)
        {


            StringBuilder sb = new StringBuilder();


            try
            {

                List<int> relOrders = new List<int>();
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var os = (from o in ctx.Orders
                              from cdi in ctx.ClientsDisplayInfos
                              from l in ctx.Locations
                              from c in ctx.Clients
                              where o.OrderID == orderID
                              && o.ClientID == cdi.ClientID
                              && o.LocationID == l.LocationID
                              && cdi.ClientID == c.ClientID
                              select new
                              {
                                  o.DateReceived,
                                  cdi,
                                  c.Office,
                                  c.LicenceNo,
                                  c.Suburb,
                                  o.PropertyAddress,
                                  l.Location1,
                                  l.State,
                                  l.PostCode
                              }).ToList();



                    if (os.Count < 1)
                    {
                        return null;

                    }

                    foreach (var item in os)
                    {
                        sb.Append("<OnlineOrder>");
                        sb.AppendFormat("<OrderId>{0}</OrderId>", orderID);
                        sb.AppendFormat("<DateReceived>{0}</DateReceived>", Convert.ToDateTime(item.DateReceived.ToString("dd-MMM-yyyy hh:mm tt")));

                        #region ClientDetails ...
                        ClientInfo clientInfo = new ClientInfo();
                        clientInfo.ClientID = Convert.ToInt32(item.cdi.ClientID);
                        clientInfo.ClientName = item.cdi.ClientName as string;
                        clientInfo.Office = item.cdi.Office as string;
                        clientInfo.ActualOffice = item.Office as string;
                        clientInfo.Address = item.cdi.Address as string;
                        clientInfo.Suburb = item.cdi.Suburb as string;
                        clientInfo.State = item.cdi.State as string;
                        clientInfo.Country = item.cdi.Country as string;
                        clientInfo.PostCode = item.cdi.PostCode as string;
                        clientInfo.Phone = item.cdi.Phone as string;
                        clientInfo.Fax = item.cdi.Fax as string;
                        clientInfo.Email = item.cdi.Email as string;
                        clientInfo.LicenseNo = item.LicenceNo as string;
                        clientInfo.WebSite = item.cdi.WebSite as string;

                        sb.Append(clientInfo.ToXML());
                        #endregion

                        #region PropertyDetails ...
                        sb.Append("<PropertyDetails>");

                        if (!string.IsNullOrEmpty(item.PropertyAddress))
                        {
                            sb.AppendFormat("<PropertyAddress>{0}, {1}</PropertyAddress>",
                                                             HttpUtility.HtmlEncode(item.PropertyAddress.ToString()),
                                                             item.Location1.ToString());
                        }
                        else
                        {
                            sb.AppendFormat("<PropertyAddress>{0}, {1}</PropertyAddress>",
                                                             " ",
                                                             item.Location1.ToString());
                        }

                        //sb.AppendFormat("<StreetNo>{0}</StreetNo>",
                        //                                 item.StreetNo.ToString());

                        //sb.AppendFormat("<StreetName>{0}</StreetName>",
                        //                                 item.StreetName.ToString());

                        //sb.AppendFormat("<Suburb>{0}</Suburb>",
                        //                                 item.Suburb.ToString());

                        sb.Append("</PropertyDetails>");
                        #endregion
                        #region TextDetails ...
                        sb.Append("<TextDetails></TextDetails>");
                        #endregion

                        sb.Append("</OnlineOrder>");

                    }
                    return sb.ToString();

                    //Serializer.BinarySerialize(sb.ToString());

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetOrderData'. propertyId:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }

        }


        #endregion

        #region RequestTemplateChanges
        public void RequestTemplateChanges(int orderID, string requestBy, string requestNumber, string reason)
        {
            RequestTemplateChangesUpload(orderID, requestBy, requestNumber, reason, string.Empty);
        }

        #endregion

        #region RequestTemplateChanges
        public void RequestTemplateChangesUpload(int orderID, string requestBy, string requestNumber, string reason, string requestFile)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(requestBy))
            {
                throw new ArgumentNullException("requestBy");
            }
            if (string.IsNullOrEmpty(requestNumber))
            {
                throw new ArgumentNullException("requestNumber");
            }
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentNullException("reason");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string orderEmail = od.SendProofTo;

                    if (string.IsNullOrEmpty(orderEmail))
                    {
                        orderEmail = od.Client.Email;
                    }

                    requestBy = requestBy.Replace("<", " ");
                    requestBy = requestBy.Replace(">", " ");
                    reason = reason.Replace("<", " ");
                    reason = reason.Replace(">", " ");
                    reason = reason.Replace("'", " ");

                    bool orderHasCommunityBoard = false;

                    try
                    {
                        orderHasCommunityBoard = od.OrderDetails.Any(odd => odd.Product.CategoryId == CategoryTypes.Billboard && odd.Product.ContentType.ToLower().Contains("community"));
                    }
                    catch (Exception ex)
                    {
                        orderHasCommunityBoard = false;
                        Logger.Exception(ex, "Error reading Product Type: " + orderID.ToString());
                    }

                    if (orderHasCommunityBoard)
                    {
                        //ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.CommunityBoardRequestTemplateChanges;
                        string sub = "Request Community Board Template Changes";
                        string xmlData = @"
									<EVENT>
									<OrderID>" + orderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                    <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
									<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
									<ContactName>" + requestBy + @"</ContactName>
									<ContactNumber>" + requestNumber + @"</ContactNumber>
									<RequestDetails>" + reason.Replace("&", "&amp;") + @"</RequestDetails>
									</EVENT>";

                        string textData = @"Notification:" +
                                                Environment.NewLine +
                                                @"Request Template Changes." +
                                                Environment.NewLine +
                                                @"Job Id: " + orderID +
                                                Environment.NewLine +
                                                @"Requested By: " + requestBy +
                                                Environment.NewLine +
                                                @"Requested Number: " + requestNumber +
                                                Environment.NewLine +
                                                @"Reason: " + reason;

                        string source = "OnlineBL_OrderService_RequestTemplateChanges";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(requestFile) ? requestFile : String.Empty);

                        ctx.SubmitChanges();

                        try
                        {
                            TemplateChangeRequest tempReq = new TemplateChangeRequest();
                            tempReq.OrderID = orderID;
                            tempReq.DateRequest = DateTime.Now;
                            tempReq.RequestDetails = reason;
                            tempReq.RequestedBy = requestBy;
                            tempReq.RequestedContactNumber = requestNumber;
                            tempReq.IsCommunityBoard = true;
                            ctx.TemplateChangeRequests.InsertOnSubmit(tempReq);
                            ctx.SubmitChanges();
                        }
                        catch (Exception exc)
                        {
                            Logger.Exception(exc, "Could not save request community board changes'. orderID: " + orderID.ToString());
                        }
                    }
                    else
                    {
                        //ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.RequestTemplateChanges;
                        string sub = "Request Template Changes";
                        string xmlData = @"
									<EVENT>
									<OrderID>" + orderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                    <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
									<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
									<ContactName>" + requestBy + @"</ContactName>
									<ContactNumber>" + requestNumber + @"</ContactNumber>
									<RequestDetails>" + reason.Replace("&", "&amp;") + @"</RequestDetails>
									</EVENT>";

                        string textData = @"Notification:" +
                                                Environment.NewLine +
                                                @"Request Template Changes." +
                                                Environment.NewLine +
                                                @"Job Id: " + orderID +
                                                Environment.NewLine +
                                                @"Requested By: " + requestBy +
                                                Environment.NewLine +
                                                @"Requested Number: " + requestNumber +
                                                Environment.NewLine +
                                                @"Reason: " + reason;

                        string source = "OnlineBL_OrderService_RequestTemplateChanges";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(requestFile) ? requestFile : String.Empty);

                        ctx.SubmitChanges();

                        try
                        {
                            TemplateChangeRequest tempReq = new TemplateChangeRequest();
                            tempReq.OrderID = orderID;
                            tempReq.DateRequest = DateTime.Now;
                            tempReq.RequestDetails = reason;
                            tempReq.RequestedBy = requestBy;
                            tempReq.RequestedContactNumber = requestNumber;
                            ctx.TemplateChangeRequests.InsertOnSubmit(tempReq);
                            ctx.SubmitChanges();
                        }
                        catch (Exception exc)
                        {
                            Logger.Exception(exc, "Could not save request changes'. orderID: " + orderID.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RequestTemplateChanges'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        public void ApproveJobByArtist(int orderID, int artistID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (artistID <= 0)
            {
                throw new ArgumentNullException("artistID");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.CDAS_ApproveJobByArtist(orderID, artistID);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ApproveJobByArtist'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #region RequestIncorrectPackChanges
        public void RequestIncorrectPackChanges(int orderID, string requestBy, string requestNumber, string reason)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(requestBy))
            {
                throw new ArgumentNullException("requestBy");
            }
            if (string.IsNullOrEmpty(requestNumber))
            {
                throw new ArgumentNullException("requestNumber");
            }
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentNullException("reason");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string orderEmail = od.SendProofTo;

                    if (string.IsNullOrEmpty(orderEmail))
                    {
                        orderEmail = od.Client.Email;
                    }

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.RequestPackChanges;
                    string sub = "Request Pack Changes";
                    string xmlData = @"
									<EVENT>
									<OrderID>" + orderID + @"</OrderID>
									<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
									<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
									<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                    <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
									<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
									<ContactName>" + requestBy + @"</ContactName>
									<ContactNumber>" + requestNumber + @"</ContactNumber>
									<RequestDetails>" + reason.Replace("&", "&amp;") + @"</RequestDetails>
									</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Request Pack Changes." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Requested By: " + requestBy +
                                            Environment.NewLine +
                                            @"Requested Number: " + requestNumber +
                                            Environment.NewLine +
                                            @"Reason: " + reason;

                    string source = "OnlineBL_OrderService_RequestIncorrectPackChanges";

                    Logger.Warn("OnlineBL_OrderService_RequestIncorrectPackChanges");
                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, String.Empty);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RequestIncorrectPackChanges'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region ApproveExpressDIYJob
        //Approve individual product
        public void ApproveExpressDIYJob(int orderID, string byWhom, DateTime? approveAll, DateTime? approveBoard, DateTime? approveBrochure, DateTime? approveOther, string purchaseOrderNumber, List<int> jobDocumentIds)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);
                    if (od == null || od.OrderStatus > 0)
                    {
                        string msg = "Your job can not be approved at this time, please contact 03 9313 0953 for further assistance in this matter.";
                        Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), msg));
                        throw new Exception(msg);
                    }
                }

                int clientID;

                // check if order has AOP job
                bool hasAOPJob = IsDesignNowApplicable(orderID);
                if (hasAOPJob)
                {
                    if (!ServiceConfig.IS_NZ)
                    {
                        UpdateXMLOnApproval(orderID);
                    }
                    //move approved document and preview
                    MoveDIYDocumentsAndPreviewFiles(orderID, jobDocumentIds);
                }

                //Continue to save
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    clientID = od.ClientID;

                    ProofDetail pd = ctx.ProofDetails.SingleOrDefault(p => p.OrderID == orderID);

                    DateTime? appAll = approveAll ?? pd.DateApproved;
                    DateTime? appBoard = approveBoard ?? pd.DateAppBoards;
                    DateTime? appBrochure = approveBrochure ?? pd.DateAppBrochures;
                    DateTime? appOther = approveOther ?? pd.DateAppOthers;

                    ctx.ABCWRKFLOW_ApproveOrder(od.OrderID, appAll, appBoard, appBrochure, appOther, true);

                    ctx.SubmitChanges();

                    if (!string.IsNullOrEmpty(purchaseOrderNumber))
                    {
                        try
                        {
                            OrderOtherInfo of = ctx.OrderOtherInfos.SingleOrDefault(o => o.OrderID == orderID);
                            if (of != null)
                            {
                                of.PurchaseOrder = purchaseOrderNumber;
                            }
                            else
                            {
                                of = new OrderOtherInfo();
                                of.OrderID = orderID;
                                of.PurchaseOrder = purchaseOrderNumber;
                                ctx.OrderOtherInfos.InsertOnSubmit(of);
                            }
                            ctx.SubmitChanges();

                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, "Error occured in 'ApproveExpressDIYJob'. Saving Purchase Order Number");
                        }
                    }
                    
                    string categoriesApproved = string.Empty;
                    if (approveBoard.HasValue)
                        categoriesApproved += ", Board";
                    if (approveBrochure.HasValue)
                        categoriesApproved += ", Brochure";
                    if (approveOther.HasValue)
                        categoriesApproved += ", Other";

                    if (categoriesApproved.Length > 0)
                    {
                        categoriesApproved = categoriesApproved.Remove(0, 2);
                        ctx.SP_EventGen_ApprovedIndividualProductsByClient(orderID, byWhom, DateTime.Now, "ApproveExpressDIYJob", categoriesApproved);
                    }
                }

                try
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        List<EntityRelations> loadOptions = new List<EntityRelations>();
                        loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                        loadOptions.Add(EntityRelations.Order_To_ProofDetail);
                        loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                        ctx.DeferredLoadingEnabled = false;
                        ctx.SetDataLoadOptions(loadOptions);

                        Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                        List<OrderDetail> odd = new List<OrderDetail>();

                        if (!od.ProofDetail.DateApproved.HasValue)
                        {

                            odd = od.OrderDetails.Where(i => !IsProductApproved(i.Product.TypeID, i.Order.ProofDetail) && (i.UserDesignOnline == true
                                && (i.Product.TypeID == ProductTypes.BillBoard || i.Product.TypeID == ProductTypes.Brochure || i.Product.TypeID == ProductTypes.WindowCard
                                           || i.Product.TypeID == ProductTypes.Corflute))).ToList();
                            if (odd.Count == 0)
                            {
                                DateTime? appAll = DateTime.Now;
                                DateTime? appBoard = od.ProofDetail.DateAppBoards;
                                DateTime? appBrochure = od.ProofDetail.DateAppBrochures;
                                DateTime? appOther = od.ProofDetail.DateAppOthers;
                                Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), "Approve All"));

                                ctx.ABCWRKFLOW_ApproveOrder(od.OrderID, appAll, appBoard, appBrochure, appOther, true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                    throw ex;
                }

                //begin new code to update the caption
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_AOP_JobDocuments);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);                   
                    loadOptions.Add(EntityRelations.AOP_JobDocument_To_OrderDetail);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                    loadOptions.Add(EntityRelations.Order_To_OrderOtherDetail);


                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    try
                    {
                        if (string.IsNullOrEmpty(od.Caption) || od.Caption.ToUpper().StartsWith("DIY"))
                        {
                            foreach (AOP_JobDocument aopJob in od.AOP_JobDocuments)
                            {
                                if (aopJob.TempDocumentModel != null)
                                {
                                    Abc.OnlinePublication.Common.DOM.Document doc = Dom.Document.Deserialize(aopJob.TempDocumentModel.ToString());

                                    Abc.OnlinePublication.Common.DOM.Tag captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "Heading"; });
                                    if (captionTag != null)
                                    {
                                        if (captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                        {
                                            od.Caption = captionTag.FormElement.Value;
                                            if (captionTag.FormElement.Value.Length > 20)
                                            {
                                                od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                            }
                                            break;
                                        }
                                    }
                                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BodyCopy"; });
                                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureHeading"; });
                                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureBodyCopy"; });
                                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        var caption = GetNamePlateAndUnitStickerCaption(od);

                        if (!string.IsNullOrEmpty(caption))
                        {
                            if (string.IsNullOrEmpty(od.Caption))
                                od.Caption = string.Format("{0}", caption);
                            else
                                od.Caption += string.Format(" - {0}", caption);
                        }

                        var brochureCaption = GetBrochureCaption(od);
                        if (!string.IsNullOrEmpty(brochureCaption))
                        {
                            if (od.OrderOtherDetail == null)
                            {
                                od.OrderOtherDetail = new OrderOtherDetail();
                            }
                            od.OrderOtherDetail.BrochureCaption = brochureCaption;
                        }
                    }
                    catch (Exception ex)
                    {

                        Logger.Exception(ex, "Error updating order caption" + orderID.ToString());
                    }

                    ctx.SubmitChanges();
                }

                try
                {
                    if (hasAOPJob)
                    {
                        //Move working DIY folder
                        if (!ServiceConfig.IS_NZ)
                        {
                            using (AbcDataContext ctx = new AbcDataContext())
                            {
                                ProofDetail pd = ctx.ProofDetails.SingleOrDefault(p => p.OrderID == orderID);

                                //check if order approve then Move all DIY working folder
                                //or just move Express one
                                if (pd.DateApproved.HasValue)
                                {
                                    MoveWorkingDIYFolder(orderID, clientID);
                                }
                                else
                                {
                                    MoveExpressDIYWorkingFolder(orderID, clientID, jobDocumentIds);
                                }
                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'ApproveExpressDIYJob'. orderID:{0}", orderID);
                    Logger.Exception(ex, message);
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ApproveExpressDIYJob'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw ex;
            }
        }

        #endregion

        #region ApproveExpressBoardDIYJob
        public void ApproveExpressBoardDIYJob(int orderID, string byWhom, DateTime? approveBoard, string purchaseOrderNumber, List<int> jobDocumentIds)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(byWhom))
            {
                throw new ArgumentNullException("byWhom");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    var paygClient = ctx.ClientsPrefs.Where(x => x.ClientId == od.ClientID && x.PrefID == ClientsPref.PayAsYouGo).FirstOrDefault();

                    if (paygClient != null && paygClient.BitValue == true && od.OrderStatus > 0)
                    {
                        throw new Exception(string.Format("Your job can not be approved at this time. Your invoice is ready. Click {0} to pay.", "<a href='/Abc.Web.Express/Accounts/PaymentSelection' target='_parent'>here</a>"));
                    }
                    if (od.OnHold.HasValue && od.Client.GroupId == ClientGroupSettings.OxbridgePropertyGroup)
                    {
                        //ctx.FrontOffice_ServiceQueueAdd("OrderNeedApproval", "OrderID=" + orderID, 0, 1, "support@oxbridge.com.au", null, null, "webdesign@photosigns.com.au", "Express_ApproveJob", 2);
                        ctx.FrontOffice_ServiceQueueAdd("OrderNeedApproval", "OrderID=" + orderID, 0, 1, "webdesign@photosigns.com.au", null, null, null, "Express_ApproveJob", 2);

                        Logger.Warn("group: " + od.Client.GroupId);
                        string msg = "Your job on hold and can not be approved at this time, please contact 03 9313 0953 for further assistance in this matter.";
                        Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), msg));
                        throw new Exception(msg);
                    }
                    if (od == null || od.OrderStatus > 0)
                    {
                        string msg = "Your job can not be approved at this time, please contact 03 9313 0953 for further assistance in this matter.";
                        Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), msg));
                        throw new Exception(msg);
                    }
                    if (od != null && od.OnHold.HasValue && od.Notes.Contains("On Hold – awaiting photography to be completed"))
                    {
                        string msg = @"Apologies for the inconvenience, this order is currently on hold awaiting the photography to be completed first. 
                                       Once the photography shoot has been completed, you will be able to approve your stockboard for installation and complete your brochure design.
                                       If you require further assistance or information, please contact our Head Office on (03) 9313 0999.";
                        Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), msg));
                        throw new Exception(msg);
                    }
                    else if (od != null && od.OnHold.HasValue)
                    {
                        string msg = "Your job on hold and can not be approved at this time, please contact 03 9313 0953 for further assistance in this matter.";
                        Logger.Warn(string.Format("Order: {0}, {1}", orderID.ToString(), msg));
                        throw new Exception(msg);
                    }
                }

                int clientID;

                // check if order has AOP job
                bool hasAOPJob = IsDesignNowApplicable(orderID);
                bool hasOnlineListing = OrderHasOnlineListing(orderID);

                if (hasAOPJob)
                {
                    if (!ServiceConfig.IS_NZ && hasOnlineListing)
                    {
                        UpdateXMLOnApproval(orderID);
                    }
                    //move approved document and preview
                    MoveDIYDocumentsAndPreviewFiles(orderID, jobDocumentIds);
                }

                //Continue to save
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                    loadOptions.Add(EntityRelations.OrderDetail_To_AOP_JobDocuments);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order order = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    clientID = order.ClientID;

                    ProofDetail pd = ctx.ProofDetails.SingleOrDefault(p => p.OrderID == orderID);

                    DateTime? appBoard = approveBoard ?? pd.DateAppBoards;

                    try
                    {
                        Logger.Warn("Order has stockboard - Approve");

                        if (order.OrderStatus <= 0)
                        {
                            var result = ctx.ACC_GetStockOrderDetails(orderID);

                            string prop = order.PropertyAddress + ", " + order.Location.Location1;

                            if (result != null)
                            {
                                bool usePrint = false;
                                string xmlDataItems = string.Empty;
                                var stockBoardOrderDetailIds = new List<int>();
                                foreach (var item in result)
                                {
                                    if (item.QtyRes > item.QtyHand)
                                    {
                                        ///call SP to IncreaseStock
                                        ctx.ACC_InventoryIncrease(item.OrderDetailsID);
                                        usePrint = true;
                                    }
                                    xmlDataItems = @"<ITEM>
			                        <ITEMNAME>" + item.Name + @"</ITEMNAME>
			                        <QTY>" + item.Qty + @"</QTY>
			                        <QTYINHAND>" + item.QtyHand + @"</QTYINHAND>
			                        <QTYRES>" + item.QtyRes + @"</QTYRES>";

                                    if (item.QtyRes > item.QtyHand)
                                        xmlDataItems += @"<DECISION>1</DECISION>";
                                    else
                                        xmlDataItems += @"<DECISION>0</DECISION>";
                                    xmlDataItems += @"</ITEM>";
                                    stockBoardOrderDetailIds.Add(item.OrderDetailsID);
                                }

                                //send email notification to manager
                                int eventID = EventSettings.StockboardProgressToManagers;
                                string sub = "Stockboard Order Progress";
                                string xmlData = string.Empty;

                                xmlData = @"<EVENT>
								<ORDERID>" + orderID + @"</ORDERID>
                                <AGENT>" + order.Client.ClientName.Replace("&", "&amp;") + "/" + order.Client.Office + @"</AGENT>
								<PADDRESS>" + prop.Replace("&", "&amp;") + @"</PADDRESS>
                                <CAPTION>Workshop Stockboard Jobs for Printing Decision</CAPTION>
                                <DATERECEIVED>" + DateTime.Now.ToString() + @"</DATERECEIVED>
                                <ITEMS>" + xmlDataItems + @"</ITEMS>
								</EVENT>";

                                string textData = xmlData;
                                string source = "ProcessOrder";

                                ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, orderID, null, order.ManagerID, null, source, String.Empty);
                                ctx.SubmitChanges();

                                if (usePrint)
                                {
                                    ctx.ABCWRKFLOW_ApproveOrder(order.OrderID, null, appBoard, null, null, true);
                                }
                                else
                                {
                                    bool hasAnyOtherProductsLikeOverlay = order.OrderDetails.Any(x => x.Product.TypeID != ProductTypes.Stockboard);
                                    string path = DirectoryPath.GetOutputFolder(orderID, ServiceConfig.APPROVED_DOC_DIR, Abc.OnlinePublication.Common.FolderStructureType.Hundred);

                                    Logger.Warn("STOCKBOARD Approve - USE FROM");
                                    try
                                    {
                                        foreach (OrderDetail detail in order.OrderDetails)
                                        {
                                            if (detail.Product.TypeID == ProductTypes.Stockboard)
                                            {
                                                detail.ItemNote += "DONT PRINT STOCKBOARD - USE FROM STOCK";

                                                //need to rename or remove the SB file, as we dont want to put SB to print
                                                string inddPattern = string.Format("{0}_{1}_{2}.indd", orderID, detail.AOP_JobDocuments[0].JobDocumentId, Path.GetFileNameWithoutExtension(detail.AOP_JobDocuments[0].TemplateOriginalFileName));
                                                string sourceFile = Path.Combine(path, inddPattern);
                                                string destFile = Path.Combine(path, inddPattern.Replace("_B", "_SB"));
                                                Logger.Warn("destFile: " + destFile);
                                                File.Move(sourceFile, destFile);
                                            }
                                        }
                                        ctx.SubmitChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Exception(ex, "Rename Stockboard file has an issue");
                                    }


                                    //Approve only
                                    if (order.ManagerID == ManagerSettings.WorkshopVictoria)
                                    {
                                        ctx.ABCWRKFLOW_ApproveOrder(order.OrderID, null, appBoard, null, null, false);

                                        //refactor this
                                        bool sendJobsheet = false;
                                        if (order.DespatchDetail != null && order.DespatchDetail.PreferredErectionDate.HasValue)
                                        {

                                            DateTime now = DateTime.Now;
                                            DateTime later = order.DespatchDetail.PreferredErectionDate.Value;

                                            int days = (later - now).Days;

                                            if (days > 4)
                                                sendJobsheet = false;
                                            else
                                                sendJobsheet = true;
                                        }
                                        else
                                        {
                                            sendJobsheet = true;
                                        }

                                        if (sendJobsheet)
                                        {
                                            ctx.FrontOffice_ServiceQueueAdd("PrintJobsheet", "OrderID=" + orderID, 0, 2, null, @"\\adbb\stockboarddepot", null, null, "Online Order Processor", 2);
                                        }
                                        else
                                        {
                                            Logger.Warn("Stockboard DIY Order: " + order.OrderID);
                                            try
                                            {
                                                BoardApproval ba = new BoardApproval();
                                                ba.OrderID = order.OrderID;
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
                                        ctx.ABCWRKFLOW_ApproveOrder(order.OrderID, null, appBoard, null, null, hasAnyOtherProductsLikeOverlay);
                                    }
                                }

                                ctx.SubmitChanges();

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format("Error occured in 'validating SB Inventory'. orderID:{0}", orderID);
                        Logger.Exception(ex, message);
                    }

                    if (!string.IsNullOrEmpty(purchaseOrderNumber))
                    {
                        try
                        {
                            OrderOtherInfo of = ctx.OrderOtherInfos.SingleOrDefault(o => o.OrderID == orderID);
                            if (of != null)
                            {
                                of.PurchaseOrder = purchaseOrderNumber;
                            }
                            else
                            {
                                of = new OrderOtherInfo();
                                of.OrderID = orderID;
                                of.PurchaseOrder = purchaseOrderNumber;
                                ctx.OrderOtherInfos.InsertOnSubmit(of);
                            }
                            ctx.SubmitChanges();

                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex, "Error occured in 'ApproveExpressDIYBoardJob'. Saving Purchase Order Number");
                        }
                    }

                    var categoriesApproved = "Board";

                    ctx.SP_EventGen_ApprovedIndividualProductsByClient(orderID, byWhom, DateTime.Now, "ApproveExpressDIYBoardJob", categoriesApproved);
                }

                //begin new code to update the caption
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_AOP_JobDocuments);
                    loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                    loadOptions.Add(EntityRelations.OrderDetail_To_Product);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);
                    bool orderHasStockboard = od.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.Stockboard);
                    bool orderHasBoard = od.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.BillBoard);

                    try
                    {
                        if (string.IsNullOrEmpty(od.Caption) || od.Caption.ToUpper().StartsWith("DIY"))
                        {
                            foreach (AOP_JobDocument aopJob in od.AOP_JobDocuments)
                            {
                                if (aopJob.TempDocumentModel != null && aopJob.OrderDetail != null && aopJob.OrderDetail.Product.CategoryId != CategoryTypes.Brochure && aopJob.OrderDetail.Product.CategoryId != CategoryTypes.WindowCard)
                                {
                                    Abc.OnlinePublication.Common.DOM.Document doc = Dom.Document.Deserialize(aopJob.TempDocumentModel.ToString());

                                    Abc.OnlinePublication.Common.DOM.Tag captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "Heading"; });
                                    if (captionTag != null)
                                    {
                                        if (captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                        {
                                            od.Caption = captionTag.FormElement.Value;
                                            if (captionTag.FormElement.Value.Length > 20)
                                            {
                                                od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                            }
                                            break;
                                        }
                                    }
                                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BodyCopy"; });
                                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureHeading"; });
                                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                    captionTag = doc.Root.Find(delegate(Abc.OnlinePublication.Common.DOM.Tag t) { return t.TagName == "BrochureBodyCopy"; });
                                    if (captionTag != null && captionTag.FormElement != null && !string.IsNullOrEmpty(captionTag.FormElement.Value))
                                    {
                                        od.Caption = captionTag.FormElement.Value;
                                        if (captionTag.FormElement.Value.Length > 20)
                                        {
                                            od.Caption = captionTag.FormElement.Value.Substring(0, 20) + "...";
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        //clear the caption if order does not have Board
                        if (!orderHasBoard)
                        {
                            od.Caption = string.Empty;
                        }

                        var stockBoardCaption = GetStockboardName(od);

                        if (!string.IsNullOrEmpty(stockBoardCaption))
                        {
                            if (string.IsNullOrEmpty(od.Caption))
                                od.Caption = string.Format("{0}", stockBoardCaption);
                            else
                                if (!od.Caption.Contains(stockBoardCaption))
                                {
                                    od.Caption = stockBoardCaption + "-" + od.Caption;
                                }
                        }

                        var caption = GetNamePlateAndUnitStickerCaption(od);

                        if (!string.IsNullOrEmpty(caption))
                        {
                            if (string.IsNullOrEmpty(od.Caption))
                                od.Caption = string.Format("{0}", caption);
                            else
                                od.Caption += string.Format(" - {0}", caption);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, "Error updating order caption" + orderID.ToString());
                    }

                    ctx.SubmitChanges();
                }

                try
                {
                    if (hasAOPJob)
                    {
                        //Move working DIY folder
                        if (!ServiceConfig.IS_NZ)
                        {
                            using (AbcDataContext ctx = new AbcDataContext())
                            {
                                ProofDetail pd = ctx.ProofDetails.SingleOrDefault(p => p.OrderID == orderID);

                                //check if order approve then Move all DIY working folder
                                //or just move Express one
                                if (pd.DateApproved.HasValue)
                                {
                                    MoveWorkingDIYFolder(orderID, clientID);
                                }
                                else
                                {
                                    MoveExpressDIYWorkingFolder(orderID, clientID, jobDocumentIds);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error occured in 'ApproveExpressDIYBoardJob'. orderID:{0}", orderID);
                    Logger.Exception(ex, message);
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ApproveExpressDIYBoardJob'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw ex;
            }
        } 
        #endregion

    }
}

