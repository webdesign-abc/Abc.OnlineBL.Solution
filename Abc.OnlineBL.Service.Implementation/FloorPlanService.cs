using System;
using System.Collections.Generic;
using System.Linq;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;

namespace Abc.OnlineBL.Service.Implementation
{
    public class FloorPlanService : IFloorPlanService
    {
        /// <summary>
        /// Gets the floorplan Package list.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>
        /// Returns the list of the floorplan Package Objects.
        /// </returns>
        public List<FloorplanPackage> GetFloorPlanPackageList(int clientId)
        {
            if (clientId <= 0)
            {
                return null;
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.FloorplanPackage_To_Location);
                    ctx.SetDataLoadOptions(loadOptions);

                    List<FloorplanPackage> floorPalnPackageObjList = (from fpp in ctx.FloorplanPackages
                                                                      where fpp.ClientId == clientId
                                                                      select fpp).ToList();

                    return floorPalnPackageObjList;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetFloorPlanPackageList'. ClientID:{0}", clientId);
                throw;
            }
        }

        /// <summary>
        /// Gets the floor plan package by id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns>
        /// Returns the floor plan package object.
        /// </returns>
        public FloorplanPackage GetFloorPlanPackageById(int floorPlanPackageId, int clientId, List<EntityRelations> loadOptions)
        {
            if (floorPlanPackageId <= 0 || clientId <= 0)
            {
                return null;
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null)
                    {
                        ctx.SetDataLoadOptions(loadOptions);
                    }

                    FloorplanPackage floorPalnPackageObj = (from fp in ctx.FloorplanPackages
                                                            where fp.ClientId == clientId && fp.FloorplanPackageID == floorPlanPackageId
                                                            select fp).SingleOrDefault();
                    return floorPalnPackageObj;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetFloorPlanPackage'. FloorplanPackageID:{0}, ClientID:{1}", floorPlanPackageId, clientId);
                throw;
            }
        }

        /*
        /// <summary>
        /// Gets the floor plan package by id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="clientId">The client id.</param>
        /// <returns>Returns the floor plan package object.</returns>
        //public FloorplanPackage GetFloorPlanPackageById(int floorPlanPackageId, int clientId)
        //{
        //    if (floorPlanPackageId <= 0 || clientId <= 0)
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        using (AbcDataContext ctx = new AbcDataContext())
        //        {
        //            ctx.DeferredLoadingEnabled = false;

        //            List<EntityRelations> loadOptions = new List<EntityRelations>();
        //            loadOptions.Add(EntityRelations.FloorplanPackage_To_Client);
        //            loadOptions.Add(EntityRelations.FloorplanPackage_To_Location);
        //            loadOptions.Add(EntityRelations.FloorplanPackage_To_Floorplans);
        //            loadOptions.Add(EntityRelations.FloorplanPackage_To_FloorplanImageStores);
        //            loadOptions.Add(EntityRelations.Floorplan_To_FloorplanIcons);
        //            ctx.SetDataLoadOptions(loadOptions);

        //            FloorplanPackage floorPalnPackageObj = (from fp in ctx.FloorplanPackages
        //                                                    where fp.ClientId == clientId && fp.FloorplanPackageID == floorPlanPackageId
        //                                                    select fp).SingleOrDefault();

        //            return floorPalnPackageObj;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Exception(ex, "Error occured in 'GetFloorPlanPackage'. FloorplanPackageID:{0}, ClientID:{1}", floorPlanPackageId, clientId);
        //        throw;
        //    }
        //}
        */

        /// <summary>
        /// Gets the floor plan by id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="floorPlanId">The floor plan id.</param>
        /// <returns>
        /// Returns the Floor Plan object.
        /// </returns>
        public Floorplan GetFloorPlanById(int floorPlanPackageId, int floorPlanId)
        {
            if (floorPlanId <= 0)
            {
                return null;
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Floorplan_To_FloorplanIcons);
                    ctx.SetDataLoadOptions(loadOptions);

                    Floorplan floorPalnObj = (from fp in ctx.Floorplans
                                              where fp.FloorplanPackageId == floorPlanPackageId && fp.FloorplanId == floorPlanId
                                              select fp).SingleOrDefault();

                    return floorPalnObj;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetFloorPlan'. FloorplanId:{0}", floorPlanId);
                throw;
            }
        }

        /// <summary>
        /// Inserts the floor plan image store.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileType">Type of the file.</param>
        public void InsertFloorPlanImageStore(int floorPlanPackageId, string fileName, int fileType)
        {
            if (floorPlanPackageId <= 0)
            {
                throw new ArgumentException("floorPlanPackageId is null");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.DeferredLoadingEnabled = false;

                        /// Insert record into table
                        FloorplanImageStore floorplanImageStoreObj = new FloorplanImageStore();
                        floorplanImageStoreObj.floorplanPackageId = floorPlanPackageId;
                        floorplanImageStoreObj.Filename = fileName;
                        floorplanImageStoreObj.FileType = fileType;
                        ctx.FloorplanImageStores.InsertOnSubmit(floorplanImageStoreObj);
                        ctx.SubmitChanges();
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
                string message = string.Format("Error occured in 'FloorPlan'. FloorPlanPackageId:{0}", floorPlanPackageId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        /// <summary>
        /// Deletes the floor plan by id.
        /// </summary>
        /// <param name="floorPlanId">The floor plan id.</param>
        public void DeleteFloorPlanById(int floorPlanId)
        {
            if (floorPlanId <= 0)
            {
                throw new System.NullReferenceException("Error occured in 'Delete FloorPlan'");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var floorPlanObj = (from fp in ctx.Floorplans
                                        where fp.FloorplanId == floorPlanId
                                        select fp).FirstOrDefault();

                    if (floorPlanObj == null)
                    {
                        return;
                    }

                    ctx.Floorplans.DeleteOnSubmit(floorPlanObj);
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'Delete FloorPlan'");
                throw;
            }
        }

        /// <summary>
        /// Updates the status by floor plan package id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        public void UpdateStatusByFloorPlanPackageId(int floorPlanPackageId, int statusValue)
        {
            if (floorPlanPackageId <= 0 || statusValue > 0)
            {
                throw new System.NullReferenceException("Error occured in 'Publish FloorPlan'");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var floorPlanObj = (from fp in ctx.FloorplanPackages
                                        where fp.FloorplanPackageID == floorPlanPackageId
                                        select fp).FirstOrDefault();

                    if (floorPlanObj == null)
                    {
                        return;
                    }

                    /// Published
                    floorPlanObj.status = statusValue;
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'Publish FloorPlan'");
                throw;
            }
        }

        /// <summary>
        /// Deletes the floor plan icons.
        /// </summary>
        /// <param name="floorPlanIconsList">The floor plan icons list.</param>
        public void DeleteFloorPlanIcons(List<FloorplanIcon> floorPlanIconsList)
        {
            if (floorPlanIconsList == null || floorPlanIconsList.Count == 0)
            {
                throw new System.NullReferenceException("Error occured in 'Delete FloorplanIcon'");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    foreach (FloorplanIcon floorPlanIconItem in floorPlanIconsList)
                    {
                        FloorplanIcon floorPlanIconObj = (from fp in ctx.FloorplanIcons
                                                          where fp.FloorplanIconId == floorPlanIconItem.FloorplanIconId
                                                          select fp).FirstOrDefault();

                        if (floorPlanIconObj == null)
                        {
                            continue;
                        }

                        ctx.FloorplanIcons.DeleteOnSubmit(floorPlanIconObj);
                        ctx.SubmitChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'Delete FloorPlan'");
                throw;
            }
        }

        /// <summary>
        /// Inserts the floor plan icons.
        /// </summary>
        /// <param name="floorPlanIcons">The floor plan icons.</param>
        public void InsertFloorPlanIcons(List<FloorplanIcon> floorPlanIcons)
        {
            if (floorPlanIcons == null || floorPlanIcons.Count == 0)
            {
                throw new ArgumentException("floorPlanIcon is null");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.DeferredLoadingEnabled = false;

                        foreach (FloorplanIcon floorPlanIcon in floorPlanIcons)
                        {
                            /// Insert record into table
                            ctx.FloorplanIcons.InsertOnSubmit(floorPlanIcon);
                            ctx.SubmitChanges();
                        }

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
                string message = string.Format("Error occured in 'FloorPlanEdit'. FloorPlanId:{0}", floorPlanIcons[0].FloorplanId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        /// <summary>
        /// Inserts the floor plan.
        /// </summary>
        /// <param name="floorPlanObj">The floor plan obj.</param>
        /// <returns></returns>
        public int? InsertFloorPlan(Floorplan floorPlanObj)
        {
            if (floorPlanObj == null || floorPlanObj.FloorplanPackageId <= 0)
            {
                throw new ArgumentException("floorPlan is null");
            }

            int? floorPlanId = null;

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.DeferredLoadingEnabled = false;

                        /// Insert record into table
                        ctx.Floorplans.InsertOnSubmit(floorPlanObj);
                        ctx.SubmitChanges();

                        if (floorPlanObj.FloorplanId > 0)
                        {
                            floorPlanId = floorPlanObj.FloorplanId;
                        }
                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        throw ex;
                    }

                    return floorPlanId;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'Add FloorPlan'. FloorplanPackageId:{0}", floorPlanObj.FloorplanPackageId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        /// <summary>
        /// Gets the location match.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="location">The location.</param>
        /// <param name="postCode">The post code.</param>
        /// <returns>
        /// Returns the List of the Location Obj.
        /// </returns>
        public List<Location> GetLocationMatch(string state, string location, string postCode)
        {
            if (state == null || string.IsNullOrEmpty(state))
            {
                return null;
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var qry = from l in ctx.Locations
                              where l.State.ToUpper() == state.ToUpper() && l.Location1.Contains(location)
                              select l;

                    if (!string.IsNullOrEmpty(postCode))
                        qry = (from q in ctx.Locations
                              where q.PostCode.Contains(postCode)
                              select q).Union(qry);

                    List<Location> locationObj = qry.ToList();

                    return locationObj;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetLocationMatch'. State:{0}", state);
                throw;
            }
        }

        /// <summary>
        /// Inserts the floorplan package.
        /// </summary>
        /// <param name="floorplanPackageObj">The floorplan package obj.</param>
        /// <returns>
        /// Returns FloorplanPackage Obj
        /// </returns>
        public FloorplanPackage InsertFloorplanPackage(FloorplanPackage floorplanPackageObj)
        {
            if (floorplanPackageObj == null)
            {
                throw new ArgumentException("FloorplanPackage is null");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    /// Insert record into table
                    ctx.FloorplanPackages.InsertOnSubmit(floorplanPackageObj);
                    ctx.SubmitChanges();
                    return floorplanPackageObj;
                }
            }
            catch (Exception ex)
            {
                string message = "Error occured in Add FloorplanPackage";
                Logger.Exception(ex, message);
                throw;
            }
        }

        ///// <summary>
        ///// Gets the floor plan price.
        ///// </summary>
        ///// <param name="clientId">The client id.</param>
        ///// <param name="productId">The product id.</param>
        ///// <param name="subTotal">The sub total.</param>
        ///// <param name="gst">The GST.</param>
        //public void GetFloorPlanPrice(int clientId, int productId, out decimal subTotal, out decimal gst)
        //{
        //    if (clientId <= 0 || productId <= 0)
        //    {
        //        throw new System.NullReferenceException("ClientId Or ProductId is 0 Or invalid");
        //    }

        //    using (AbcDataContext ctx = new AbcDataContext())
        //    {
        //        ctx.DeferredLoadingEnabled = false;

        //        /// Gst Applied or Not.
        //        bool applyGST = ctx.Products.SingleOrDefault(x => x.ProductID == productId).ApplyGST;

        //        /// Get Price List ID by ClientId.
        //        int? priceListID = ctx.Clients.SingleOrDefault(x => x.ClientID == clientId).PriceListID;

        //        if (!priceListID.HasValue)
        //        {
        //            throw new Exception("This priceListId is not available for this client.");
        //        }

        //        int priceListId = (int)priceListID;
        //        int? countProd =  (from pd in ctx.PriceListDetails
        //                           where pd.PriceListID == priceListId && pd.ProductID == productId
        //                           select pd).Count();
        //        decimal price = 0;

        //        if (countProd.HasValue)
        //        {
        //            price = ctx.PriceListDetails.SingleOrDefault(x => x.PriceListID == priceListId && x.ProductID == productId).ProductPrice;

        //            if (price <= 0)
        //            {
        //                throw new Exception("This product is not available for this client.");
        //            }
        //        }
        //        else
        //        {
        //            price = (from pp in ctx.ProductPricings
        //                     join pl in ctx.PriceLists on pp.PricingID equals pl.PricingID
        //                     where pp.ProductID == productId && pl.PriceListID == priceListId
        //                     select pp).SingleOrDefault().Price;

        //            if (price <= 0)
        //            {
        //                throw new Exception("This product is not available for this client.");
        //            }

        //            /// INSERT INTO PriceListDetails
        //            PriceListDetail priceListDetailObj =new PriceListDetail();
        //            priceListDetailObj.PriceListID = priceListId;
        //            priceListDetailObj.ProductID = productId;
        //            priceListDetailObj.ProductPrice = price;
        //            priceListDetailObj.IsCustom = false;
                    
        //            ctx.PriceListDetails.InsertOnSubmit(priceListDetailObj);
        //            ctx.SubmitChanges();
        //        }

        //        subTotal = price;

        //        gst = -1; 

        //        if (applyGST)
        //        {
        //            gst = price * OnlineBLConfig.CURRENT_GST; 
        //        }
        //    }
        //}

        /// <summary>
        /// Gets the floor plan price.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="productId">The product id.</param>
        /// <returns>
        /// Returns FloorPlanPrice Obj
        /// </returns>
        public FloorPlanPrice GetFloorPlanPrice(int clientId, int productId)
        {
           if (clientId <= 0 || productId <= 0)
           {
               throw new System.NullReferenceException("ClientId Or ProductId is 0 Or invalid");
           }

           try
           {
               using (AbcDataContext ctx = new AbcDataContext())
               {
                   ctx.DeferredLoadingEnabled = false;
                   decimal? subTotal = 0;
                   decimal? gst = 0;

                   /// Get FloorPlan Price
                   ctx.Floorplan_GetFloorplanPrice(clientId, productId, ref subTotal, ref gst);
                   ctx.SubmitChanges();

                   FloorPlanPrice floorPlanPriceObj = new FloorPlanPrice();

                   floorPlanPriceObj.SubTotal = subTotal;
                   floorPlanPriceObj.Gst = gst;

                   return floorPlanPriceObj;
               }
           }
           catch (Exception ex)
           {
               string message = "Error occured in FloorplanPrice";
               Logger.Exception(ex, message);
               throw;
           } 
        }

        /// <summary>
        /// Pres the payment process.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <returns>
        /// Pre payment processing task
        /// </returns>
        public int? PrePaymentProcess(int? productId, int? floorPlanPackageId)
        {
            if (!floorPlanPackageId.HasValue || !productId.HasValue)
            {
                throw new System.NullReferenceException("FloorPlanPackageId Or ProductId is 0 Or invalid");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    /// Get FloorPlan Price
                    int? returnValue = ctx.Floorplan_PrePaymentProcess(productId, floorPlanPackageId);
                    ctx.SubmitChanges();

                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                string message = "Error occured in PrePaymentProcess";
                Logger.Exception(ex, message);
                throw;
            } 
        }

        public int? PostPaymentProcess(int? floorPlanPackageId, string paymentResponseData, int? status)
        {
            if (!floorPlanPackageId.HasValue || !status.HasValue)
            {
                throw new System.NullReferenceException("FloorPlanPackageId Or Status is 0 Or invalid");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    /// Get FloorPlan Price
                    int? returnValue = ctx.Floorplan_PostPayemntProcess(floorPlanPackageId, paymentResponseData,status);
                    ctx.SubmitChanges();

                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                string message = "Error occured in PostPaymentProcess";
                Logger.Exception(ex, message);
                throw;
            }
        }
     }
}
