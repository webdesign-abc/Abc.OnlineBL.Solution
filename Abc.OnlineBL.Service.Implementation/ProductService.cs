using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Service.Implementation.BusinessLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace Abc.OnlineBL.Service.Implementation
{
    public partial class ProductService : IProductService
    {

        #region SayHello
        public string SayHello(string name)
        {
            string sName = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
            return string.Format("Hello {0}. You are also {1}", name, sName);
        }
        #endregion

        #region GetProductTypes
        public List<ProductType> GetProductTypes()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from pt in ctx.ProductTypes
                                select pt;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductTypes'.");
                throw;
            }
        }
        #endregion

        #region GetProductById
        public Product GetProductById(int productId, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = (from p in ctx.Products
                                 where p.ProductID == productId
                                 select p).FirstOrDefault();

                    if (query != null && query.ProductRules != null && query.ProductRules.Count > 0)
                    {
                        foreach (var item in query.ProductRules)
                        {
                            var manager = (from m in ctx.Managers
                                           where m.ManagerID == item.ManagerId
                                           select m).FirstOrDefault();
                            if (manager != null)
                                item.ManagerName = manager.Name;

                            var client = (from c in ctx.Clients
                                          where c.ClientID == item.ClientId
                                          select c).FirstOrDefault();
                            if (client != null)
                                item.ClientNameOffice = client.ClientName + "/" + client.Office;

                            var pricing = (from p in ctx.PricingTypes
                                           where p.PricingID == item.PricingID
                                           select p).FirstOrDefault();
                            if (pricing != null)
                                item.PricingType = pricing.PricingType1;

                            var businessRegion = (from b in ctx.BusinessRegions
                                                  where b.BusinessRegionID == item.BusinessRegionID
                                                  select b).FirstOrDefault();
                            if (businessRegion != null)
                                item.BusinessRegionName = businessRegion.BusinessRegionName;
                        }
                    }

                    return query;
                }
            }
            catch (Exception ex)
            {
                string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
                Logger.Exception(ex, "Error occured in 'GetProductById'. ProductId:{0}. LoadOptions:{1}", productId, options);
                throw;
            }
        }
        #endregion

        #region GetProductsByIds
        public List<Product> GetProductsByIds(List<int> productIds, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from p in ctx.Products
                                where productIds.Contains(p.ProductID)
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                string message = string.Empty;
                productIds.ForEach(delegate(int id) { message += string.Format("{0},", id); });
                Logger.Exception(ex, "Error occured in 'GetProductsByIds'. ProductIds:{0}.", message.TrimEnd(','));
                throw;
            }
        }
        #endregion

        #region GetProductsByTypeId
        public List<Product> GetProductsByTypeId(int typeId, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from p in ctx.Products
                                where p.TypeID == typeId
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductsByTypeId'. TypeId:{0}.", typeId);
                throw;
            }
        }
        #endregion

        #region GetProductSizeCodeDetailsByProductTypeId
        public List<ProductSizeCodeDetail> GetProductSizeCodeDetailsByProductTypeId(int productTypeId, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from p in ctx.ProductSizeCodeDetails
                                where p.ProductTypeId == productTypeId
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductSizeCodeDetailsByProductTypeId'. ProductTypeId:{0}.", productTypeId);
                throw;
            }
        }
        #endregion

        #region UpdateProductType
        public void UpdateProductType(ProductType productType)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    productType.SynchroniseWithDataContext(ctx);
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'UpdateProductType'. TypeId:{0}, TypeName:{1}.", productType.TypeID, productType.Type);
                throw;
            }
        }
        #endregion

        #region GetProductsByTypeIDAndPriceListID
        public List<Product> GetProductsByTypeIDAndPriceListID(int typeID, int priceListID, List<EntityRelations> loadOptions)
        {
            if (typeID <= 0)
            {
                throw new ArgumentNullException("typeID");
            }
            if (priceListID <= 0)
            {
                throw new ArgumentNullException("priceListID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from pl in ctx.PriceListDetails
                                where pl.PriceListID == priceListID
                                select pl.ProductID;

                    List<int> productIds = query.ToList();

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var result = from p in ctx.Products
                                 where p.TypeID == typeID
                                 && p.Active == true
                                 && productIds.Contains(p.ProductID)
                                 select p;

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductsByTypeIDAndPriceListID'. TypeId:{0}, PriceListID{1}.", typeID, priceListID);
                throw;
            }
        }

        #endregion

        #region GetProductsByTypeIDAndClientID
        public List<Product> GetProductsByTypeIDAndClientID(int typeID, int clientID, List<EntityRelations> loadOptions)
        {
            if (typeID <= 0)
            {
                throw new ArgumentNullException("typeID");
            }
            if (clientID <= 0)
            {
                throw new ArgumentNullException("clientID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from pl in ctx.PriceListDetails
                                join c in ctx.Clients on pl.PriceListID equals c.PriceListID
                                where c.ClientID == clientID
                                select pl.ProductID;

                    List<int> productIds = query.ToList();

                    var result = from p in ctx.Products
                                 where p.TypeID == typeID
                                 && p.Active == true
                                 && productIds.Contains(p.ProductID)
                                 select p;

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductsByTypeIDAndClientID'. TypeId:{0}, clientID{1}.", typeID, clientID);
                throw;
            }
        }

        #endregion

        #region GetWindowCardProductsByClientID
        public List<Product> GetWindowCardProductsByClientID(int clientID, List<EntityRelations> loadOptions)
        {
            if (clientID <= 0)
            {
                throw new ArgumentNullException("clientID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var results = GetProductsByTypeId(ProductTypes.WindowCard, null);

                    List<Product> tempWCFromPool = results.FindAll(p => p.Active == true
                                            && p.ContentType != null
                                            && (p.SizeCode == "A3" || p.SizeCode == "A4")).ToList();

                    var profileResults = GetProductsByTypeIDAndClientID(ProductTypes.WindowCard, clientID, null);
                    List<Product> tempProf = profileResults.FindAll(p => p.Active == true
                                            && p.ContentType != null
                                            && p.SizeCode != null).ToList();

                    ProductComparer comp = new ProductComparer();
                    List<Product> tempP = (tempProf.Union<Product>(tempWCFromPool)).Distinct(comp).ToList();

                    tempP.ForEach(p => p.Name = p.SizeCode.ToUpper() == "CUSTOM" ? p.Name + " - " + GetProductCustomDescription(clientID, p.ProductID) : p.Name);

                    tempP = tempP.OrderBy(p => p.Name).ToList();

                    return tempP;

                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetWindowCardProductsByClientID'. clientID:{0}.", clientID);
                throw;
            }
        }

        private string GetProductCustomDescription(int clientID, int productId)
        {
            if (clientID <= 0)
            {
                throw new ArgumentNullException("clientID");
            }
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    string result = (from pld in ctx.PriceListDetails
                                     join p in ctx.Products on pld.ProductID equals p.ProductID
                                     join c in ctx.Clients on pld.PriceListID equals c.PriceListID
                                     where c.ClientID == clientID
                                     && p.ProductID == productId
                                     select pld.CustomDescription).FirstOrDefault();


                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductCustomDescription'. clientID:{0}.", clientID);
                throw;
            }
        }

        #endregion

        #region GetBrochureProductsByClientID
        public List<Product> GetBrochureProductsByClientID(int clientID, List<EntityRelations> loadOptions)
        {
            if (clientID <= 0)
            {
                throw new ArgumentNullException("clientID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<Product> results = GetProductsByTypeId(ProductTypes.Brochure, null);

                    List<Product> tempWCFromPool = results.FindAll(p => p.Active == true
                                            && p.ContentType != null
                                            && p.PaperType != null
                                            && p.SizeCode != null
                                            && p.SizeCode.ToUpper() != "CUSTOM").ToList();


                    var profileResults = GetProductsByTypeIDAndClientID(ProductTypes.Brochure, clientID, null);
                    List<Product> tempProf = profileResults.FindAll(p => p.Active == true
                                            && p.ContentType != null
                                            && p.PaperType != null
                                            && p.SizeCode != null).ToList();

                    ProductComparer comp = new ProductComparer();
                    List<Product> tempP = (tempProf.Union<Product>(tempWCFromPool)).Distinct(comp).ToList();

                    tempP.ForEach(p => p.Name = p.SizeCode.ToUpper() == "CUSTOM" ? p.Name + " - " + GetProductCustomDescription(clientID, p.ProductID) : p.Name);

                    tempP = tempP.OrderBy(p => p.Name).ToList();

                    return tempP;

                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetBrochureProductsByClientID'. clientID:{0}.", clientID);
                throw;
            }
        }

        #endregion

        #region GetOnlineOrderProductsByTypeIDAndClientID
        public List<Product> GetOnlineOrderProductsByTypeIDAndClientID(int typeID, int clientID)
        {
            if (clientID <= 0)
            {
                throw new ArgumentNullException("clientID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var result = ctx.AIS_GetAvailableOnlineOrderProducts(clientID, typeID);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetOnlineOrderProductsByTypeIDAndClientID'. TypeId:{0}, clientID{1}.", typeID, clientID);
                throw;
            }
        }

        #endregion

        #region GetPackagesContentProductByPackageID
        public List<PackageProduct> GetPackagesContentProductByPackageID(int packageId)
        {
            if (packageId <= 0)
            {
                throw new ArgumentNullException("packageId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    List<PackageProduct> query = (from pcgp in ctx.PackageContentGroupProducts
                                                  join pcg in ctx.PackageContentGroups on pcgp.GroupId equals pcg.GroupId
                                                  join p in ctx.Products on pcgp.ProductId equals p.ProductID
                                                  where pcg.ProductId == packageId
                                                  select new PackageProduct
                                                  {
                                                      GroupId = pcgp.GroupId,
                                                      GroupName = pcg.GroupName,
                                                      ProductId = p.ProductID,
                                                      ProductName = p.Name,
                                                      Qty = pcgp.Qty
                                                  }).ToList();

                    return query.ToList();

                }

            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetPackagesContentProductByPackageID'. packageId:{0}}.", packageId);
                throw;
            }


        }

        #endregion

        #region GetAllProducts
        public List<Product> GetAllProducts(List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from p in ctx.Products
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetAllProducts'");
                throw;
            }
        }

        #endregion

        #region GetWorkflowTypes
        public List<WorkflowType> GetWorkflowTypes()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from pt in ctx.WorkflowTypes
                                select pt;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetWorkflowTypes'.");
                throw;
            }
        }

        #endregion

        #region GetProductSizeCodeDetails
        public List<ProductSizeCodeDetail> GetProductSizeCodeDetails()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    //Need to add Entity Relation
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.ProductSizeCodeDetail_To_ProductSizeCode);
                    options.Add(EntityRelations.ProductSizeCodeDetail_To_ProductType);

                    ctx.SetDataLoadOptions(options);

                    var query = from pt in ctx.ProductSizeCodeDetails
                                where pt.ProductTypeId != ProductTypes.Stockboard && pt.ProductTypeId != ProductTypes.Corflute
                                select pt;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductSizeCodeDetails'.");
                throw;
            }
        }

        #endregion

        #region GetProductFilterData
        public ProductFilterData GetProductFilterData()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ProductFilterData result = new ProductFilterData();

                    var names = (from pt in ctx.Products
                                 select new AbcKeyValuePair()
                                 {
                                     Key = pt.Name,
                                     Value = pt.Name
                                 }).ToList();

                    result.Names = names;

                    var types = (from pt in ctx.ProductTypes
                                 select new AbcKeyValuePair()
                                 {
                                     Key = pt.Type,
                                     Value = pt.TypeID.ToString()
                                 }).ToList();

                    result.ContentTypes = types;

                    var workflowTypes = (from pt in ctx.WorkflowTypes
                                         select new AbcKeyValuePair()
                                        {
                                            Key = pt.TypeName,
                                            Value = pt.WorkflowTypeId.ToString()
                                        }).ToList();

                    result.WorkflowTypes = workflowTypes;

                    var sizeCodes = (from pt in ctx.ProductSizeCodeDetails
                                     select new AbcKeyValuePair()
                                    {
                                        Key = pt.SizeCode,
                                        Value = pt.SizeCode
                                    }).Distinct().ToList();

                    result.SizeCodes = sizeCodes;

                    var actives = new List<AbcKeyValuePair>()
										{
											new AbcKeyValuePair() { Key = "True", Value = "True"},
											new AbcKeyValuePair() { Key = "False", Value = "False"}
										};

                    result.Actives = actives;

                    var pricings = (from pt in ctx.PricingTypes
                                    select new AbcKeyValuePair()
                                    {
                                        Key = pt.PricingType1,
                                        Value = pt.PricingID.ToString()
                                    }).ToList();

                    result.HasPricingFors = pricings;

                    var frames = (from pt in ctx.Products
                                  where pt.FrameType != null
                                  select new AbcKeyValuePair()
                                  {
                                      Key = pt.FrameType,
                                      Value = pt.FrameType
                                  }).Distinct().ToList();

                    result.FrameTypes = frames;

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductFilterData'.");
                throw;
            }
        }

        #endregion

        #region UpdatePriceListPrice
        public void UpdatePriceListPrice(int productID)
        {
            if (productID <= 0)
            {
                throw new ArgumentNullException("productID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.sp_UpdatePriceListsPrice(productID);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'UpdatePriceListPrice'. productID:{0}", productID);
                throw;
            }
        }

        #endregion

        #region CopyProduct
        public void CopyProduct(int productID)
        {
            if (productID <= 0)
            {
                throw new ArgumentNullException("productID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    int? newID = 0;
                    ctx.DeferredLoadingEnabled = false;

                    ctx.AIS_CopyProduct(productID, ref newID);


                    if (newID.HasValue && newID.Value > 0)
                    {

                        var oldpcgs = (from g in ctx.PackageContentGroups
                                       where g.ProductId == productID
                                       select g).ToList();

                        if (oldpcgs != null && oldpcgs.Count > 0)
                        {
                            foreach (var item in oldpcgs)
                            {
                                PackageContentGroup pcg = new PackageContentGroup();
                                pcg.ProductId = newID.Value;
                                pcg.GroupName = item.GroupName;
                                ctx.PackageContentGroups.InsertOnSubmit(pcg);
                                ctx.SubmitChanges();

                                var oldpcgps = (from p in ctx.PackageContentGroupProducts
                                                where p.GroupId == item.GroupId
                                                select p).ToList();

                                if (oldpcgps != null && oldpcgps.Count > 0)
                                {
                                    foreach (var productItem in oldpcgps)
                                    {
                                        PackageContentGroupProduct pcgp = new PackageContentGroupProduct();
                                        pcgp.Format = productItem.Format;
                                        pcgp.ItemNotes = productItem.ItemNotes;
                                        pcgp.GroupId = pcg.GroupId;
                                        pcgp.ProductId = productItem.ProductId;
                                        pcgp.Qty = productItem.Qty;
                                        ctx.PackageContentGroupProducts.InsertOnSubmit(pcgp);
                                        ctx.SubmitChanges();
                                    }
                                }

                            }
                        }

                        //Copy Product rules
                        var prs = ctx.ProductRules.Where(p => p.ProductId == productID).ToList();
                        if (prs != null && prs.Count > 0)
                        {
                            foreach (var item in prs)
                            {
                                ProductRule pr = new ProductRule();
                                pr.AllowOrDeny = item.AllowOrDeny;
                                pr.BusinessRegionID = item.BusinessRegionID;
                                pr.ClientId = item.ClientId;
                                pr.ClientName = item.ClientName;
                                pr.ManagerId = item.ManagerId;
                                pr.PricingID = item.PricingID;
                                pr.ProductId = newID.Value;
                                pr.State = item.State;
                                ctx.ProductRules.InsertOnSubmit(pr);
                                ctx.SubmitChanges();
                            }

                        }


                        //Copy Product Pricing
                        var pps = ctx.ProductPricings.Where(p => p.ProductID == productID).ToList();
                        if (pps != null && pps.Count > 0)
                        {
                            foreach (var item in pps)
                            {
                                ProductPricing pp = new ProductPricing();
                                pp.ProductID = newID.Value;
                                pp.PricingID = item.PricingID;
                                pp.Price = item.Price;
                                ctx.ProductPricings.InsertOnSubmit(pp);
                                ctx.SubmitChanges();
                            }
                        }

                        //Copy Available For Manager
                        var pmes = ctx.ProductManagerExclusives.Where(p => p.ProductId == productID).ToList();
                        if (pmes != null && pmes.Count > 0)
                        {
                            foreach (var item in pmes)
                            {
                                ProductManagerExclusive pme = new ProductManagerExclusive();
                                pme.ProductId = newID.Value;
                                pme.ManagerId = item.ManagerId;
                                ctx.ProductManagerExclusives.InsertOnSubmit(pme);
                                ctx.SubmitChanges();
                            }
                        }

                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'CopyProduct'. productID:{0}", productID);
                throw;
            }
        }

        #endregion

        #region UpdateProduct
        public void UpdateProduct(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException("Product is null");
                }
                if (!product.CategoryId.HasValue)
                {
                    throw new ArgumentNullException("CategoryID is null");
                }
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();

                        List<EntityRelations> options = new List<EntityRelations>();
                        options.Add(EntityRelations.Product_To_ProductPricings);
                        options.Add(EntityRelations.ProductPricing_To_PricingType);
                        options.Add(EntityRelations.Product_To_ProductManagerExclusives);
                        options.Add(EntityRelations.Product_To_ProductRules);

                        ctx.DeferredLoadingEnabled = false;
                        if (options != null && options.Count > 0)
                            ctx.SetDataLoadOptions(options);

                        var pcks = from pck in ctx.PackageContentGroups
                                   where pck.ProductId == product.ProductID
                                   select pck;

                        foreach (var item in pcks)
                        {
                            ctx.PackageContentGroups.DeleteOnSubmit(item);
                            ctx.SubmitChanges();
                        }

                        Product query = (from p in ctx.Products
                                         where p.ProductID == product.ProductID
                                         select p).FirstOrDefault();

                        if (query == null)
                            query = new Product();

                        if (product.AJPS_Rule == null || (!product.AJPS_Rule.PrintJobsheet && !product.AJPS_Rule.EmailJobsheet && !product.AJPS_Rule.GeneratePS))
                        {
                            query.RuleId = null;
                        }
                        else
                        {

                            var rule = (from r in ctx.AJPS_Rules
                                        where object.Equals(r.EmailCc, product.AJPS_Rule.EmailCc) && r.EmailJobsheet == product.AJPS_Rule.EmailJobsheet
                                        && object.Equals(r.EmailTo, product.AJPS_Rule.EmailTo) && r.GeneratePS == product.AJPS_Rule.GeneratePS
                                        && object.Equals(r.PrinterName, product.AJPS_Rule.PrinterName) && r.PrintJobsheet == product.AJPS_Rule.PrintJobsheet
                                        && object.Equals(r.RIPLocation1, product.AJPS_Rule.RIPLocation1) && object.Equals(r.RIPLocation2, product.AJPS_Rule.RIPLocation2)
                                        && object.Equals(r.RIPLocation3, product.AJPS_Rule.RIPLocation3) && object.Equals(r.RIPLocation4, product.AJPS_Rule.RIPLocation4)
                                        && object.Equals(r.RIPLocation5, product.AJPS_Rule.RIPLocation5) && object.Equals(r.RIPLocation6, product.AJPS_Rule.RIPLocation6)
                                        && r.SendToLessBusyRIP == product.AJPS_Rule.SendToLessBusyRIP && r.EmailPdfAttachment == product.AJPS_Rule.EmailPdfAttachment
                                        select r).FirstOrDefault();


                            if (rule == null)
                            {
                                rule = new AJPS_Rule()
                                {
                                    EmailCc = product.AJPS_Rule.EmailCc,
                                    EmailJobsheet = product.AJPS_Rule.EmailJobsheet,
                                    EmailTo = product.AJPS_Rule.EmailTo,
                                    GeneratePS = product.AJPS_Rule.GeneratePS,
                                    PrinterName = product.AJPS_Rule.PrinterName,
                                    PrintJobsheet = product.AJPS_Rule.PrintJobsheet,
                                    RIPLocation1 = product.AJPS_Rule.RIPLocation1,
                                    RIPLocation2 = product.AJPS_Rule.RIPLocation2,
                                    RIPLocation3 = product.AJPS_Rule.RIPLocation3,
                                    RIPLocation4 = product.AJPS_Rule.RIPLocation4,
                                    RIPLocation5 = product.AJPS_Rule.RIPLocation5,
                                    RIPLocation6 = product.AJPS_Rule.RIPLocation6,
                                    SendToLessBusyRIP = product.AJPS_Rule.SendToLessBusyRIP,
                                    EmailPdfAttachment = product.AJPS_Rule.EmailPdfAttachment
                                };

                                ctx.AJPS_Rules.InsertOnSubmit(rule);
                                ctx.SubmitChanges();

                            }
                            query.RuleId = rule.RuleId;
                        }

                        query.Name = product.Name;
                        query.TypeID = product.TypeID;
                        query.WorkflowTypeId = product.WorkflowTypeId;
                        query.CategoryId = product.CategoryId;
                        query.SubCategoryId = product.SubCategoryId;
                        query.ShowOnWebSite = product.ShowOnWebSite;
                        query.ApplyGST = product.ApplyGST;
                        query.Active = product.Active;
                        if (product.RelatedClientId >= 0)
                        {
                            query.RelatedClientId = product.RelatedClientId;
                        }
                        else
                        {
                            query.RelatedClientId = null;
                        }
                        query.SizeCode = product.SizeCode;
                        query.Dimensions = product.Dimensions;
                        query.ContentType = !string.IsNullOrEmpty(product.ContentType) ? product.ContentType : null;
                        query.FrameType = !string.IsNullOrEmpty(product.FrameType) ? product.FrameType : null;
                        query.Qty = product.Qty > 0 ? product.Qty : null;
                        query.Format = !string.IsNullOrEmpty(product.Format) ? product.Format : null;
                        query.CustomName = product.CustomName;
                        query.IsPhotoItem = product.IsPhotoItem;
                        query.IsStockboardItem = product.IsStockboardItem;
                        query.WebFriendlyName = product.WebFriendlyName;
                        query.ProductDescription = product.ProductDescription;
                        query.ProductGroupID = product.ProductGroupID;

                        if (product.ProductPricings != null && product.ProductPricings.Count > 0)
                        {
                            query.ProductPricings = null;
                        }
                        foreach (var item in product.ProductPricings)
                        {
                            query.ProductPricings.Add(new ProductPricing()
                            {
                                Price = item.Price,
                                ProductID = item.ProductID,
                                PricingID = item.PricingID
                            });
                        }

                        query.ProductManagerExclusives = null;
                        foreach (var item in product.ProductManagerExclusives)
                        {
                            query.ProductManagerExclusives.Add(new ProductManagerExclusive()
                            {
                                ManagerId = item.ManagerId,
                                ProductId = item.ProductId,
                            });
                        }

                        query.ProductRules = null;
                        foreach (var item in product.ProductRules)
                        {
                            query.ProductRules.Add(new ProductRule()
                            {
                                ProductId = item.ProductId,
                                AllowOrDeny = item.AllowOrDeny,
                                ManagerId = item.ManagerId,
                                ClientId = item.ClientId,
                                ClientName = item.ClientName,
                                State = item.State,
                                PricingID = item.PricingID,
                                BusinessRegionID = item.BusinessRegionID
                            });
                        }

                        query.PackageContentGroups = null;

                        foreach (var item in product.PackageContentGroups)
                        {
                            //build up package content group object
                            PackageContentGroup pcg = new PackageContentGroup() { GroupName = item.GroupName, ProductId = item.ProductId };
                            //loop through item to build package content group product
                            pcg.PackageContentGroupProducts = new System.Data.Linq.EntitySet<PackageContentGroupProduct>();
                            foreach (var element in item.PackageContentGroupProducts)
                            {
                                pcg.PackageContentGroupProducts.Add(new PackageContentGroupProduct()
                                {
                                    ProductId = element.ProductId,
                                    Format = element.Format,
                                    ItemNotes = element.ItemNotes,
                                    Qty = element.Qty
                                });
                            }
                            query.PackageContentGroups.Add(pcg);
                        }

                        if (query.ProductID == 0)
                        {
                            ctx.Products.InsertOnSubmit(query);
                        }

                        ctx.SubmitChanges();

                        ctx.Transaction.Commit();

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
                Logger.Exception(ex, "Error occured in 'UpdateProduct'");
                throw ex;
            }
        }

        #endregion

        public string DeleteProduct(int productId)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var product = (from pd in ctx.Products
                                   where pd.ProductID == productId
                                   select pd).FirstOrDefault();

                    ctx.Products.DeleteOnSubmit(product);
                    ctx.SubmitChanges();

                    RemoveAllProductFile(productId);
                }
                return "OK";
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'DeleteProduct'");
                return ex.Message;
            }

        }

        #region GetAJPSRuleById
        public AJPS_Rule GetAJPSRuleById(int ruleId, List<EntityRelations> loadOptions)
        {
            if (ruleId <= 0)
            {
                throw new ArgumentNullException("ruleId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from p in ctx.AJPS_Rules
                                where p.RuleId == ruleId
                                select p;

                    return query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
                Logger.Exception(ex, "Error occured in 'GetAJPSRuleById'. ruleId:{0}. LoadOptions:{1}", ruleId, options);
                throw;
            }
        }

        #endregion

        #region GetRIPLocations
        public List<RIPLocation> GetRIPLocations()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from pt in ctx.RIPLocations
                                select pt;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetRIPLocations'.");
                throw;
            }
        }

        #endregion

        #region GetAllManagers
        public List<ManagerDetails> GetAllManagers()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var pricings = (from m in ctx.Managers
                                    select new ManagerDetails()
                                    {
                                        ManagerID = m.ManagerID,
                                        Name = m.Name,
                                        IsActive = m.Active
                                    }).ToList();

                    return pricings.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetAllManagers'.");
                throw;
            }
        }

        #endregion

        #region GetAllStates
        public List<State> GetAllStates()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from p in ctx.States
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetAllStates'");
                throw;
            }
        }

        #endregion

        #region GetAllBusinessRegions
        public List<BusinessRegion> GetAllBusinessRegions()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from p in ctx.BusinessRegions
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetAllBusinessRegions'");
                throw;
            }
        }

        #endregion

        #region GetAllOnlineOrderCategories
        public List<OnlineOrderCategory> GetAllOnlineOrderCategories(List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);
                    var query = from p in ctx.OnlineOrderCategories
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetAllOnlineOrderCategories'");
                throw;
            }
        }

        #endregion

        #region GetProductFileList
        public List<string> GetProductFileList(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            try
            {
                string text = string.Empty;
                List<string> fileList = new List<string>();

                string path = Path.Combine(ServiceConfig.PRODUCT_FILES_DIR, productId.ToString());

                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path);

                    foreach (string file in files)
                    {
                        if (Path.GetFileName(file).StartsWith(productId.ToString()))
                        {
                            fileList.Add(Path.GetFileName(file));
                        }
                    }
                }

                return fileList;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProductFileList'. productId:{0}", productId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region RemoveProductFile
        public void RemoveProductFile(int productId, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            if (!fileName.StartsWith(productId + "_"))
            {
                throw new ArgumentNullException("FileName does not containts product ID");
            }
            try
            {
                string filePath = Path.Combine(ServiceConfig.PRODUCT_FILES_DIR, productId.ToString());
                filePath = Path.Combine(filePath, fileName);

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RemoveProductFile'. fileName:{0}", fileName);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region MoveProductFileToRightFolder
        public void MoveProductFileToRightFolder(int productId, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            if (!fileName.StartsWith(productId + "_"))
            {
                throw new ArgumentNullException("FileName does not containts product ID");
            }
            try
            {

                string filePath = Path.Combine(ServiceConfig.PRODUCT_FILES_DIR, productId.ToString());


                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath = Path.Combine(filePath, fileName);

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }

                string tempFilePath = Path.Combine(ServiceConfig.PRODUCT_FILES_DIR_TEMP, fileName);
                File.Move(tempFilePath, filePath);
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'MoveProductFileToRightFolder'. fileName:{0}", fileName);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetAllProductGroupDescriptions
        public List<ProductGroupDescription> GetAllProductGroupDescriptions(List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from p in ctx.ProductGroupDescriptions
                                select p;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetAllProductGroupDescriptions'");
                throw;
            }
        }

        #endregion

        #region GetProductGroupDescriptionById
        public ProductGroupDescription GetProductGroupDescriptionById(int productGroupId, List<EntityRelations> loadOptions)
        {
            if (productGroupId <= 0)
            {
                throw new ArgumentNullException("productGroupId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = (from p in ctx.ProductGroupDescriptions
                                 where p.ProductGroupID == productGroupId
                                 select p).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
                Logger.Exception(ex, "Error occured in 'GetProductGroupDescriptionById'. ProductGroupId:{0}. LoadOptions:{1}", productGroupId, options);
                throw;
            }
        }

        #endregion

        #region GetProductGroupFileList
        public List<string> GetProductGroupFileList(int productGroupId)
        {
            if (productGroupId <= 0)
            {
                throw new ArgumentNullException("productGroupId");
            }
            try
            {
                string text = string.Empty;
                List<string> fileList = new List<string>();

                string path = Path.Combine(ServiceConfig.PRODUCT_GROUP_FILES_DIR, productGroupId.ToString());

                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path);

                    foreach (string file in files)
                    {
                        if (Path.GetFileName(file).StartsWith(productGroupId.ToString()))
                        {
                            fileList.Add(Path.GetFileName(file));
                        }
                    }
                }

                return fileList;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProductGroupFileList'. productGroupId:{0}", productGroupId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region MoveProductGroupFileToRightFolder
        public void MoveProductGroupFileToRightFolder(int productGroupId, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (productGroupId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            if (!fileName.StartsWith(productGroupId + "_"))
            {
                throw new ArgumentNullException("FileName does not containts product group ID");
            }
            try
            {
                string filePath = Path.Combine(ServiceConfig.PRODUCT_GROUP_FILES_DIR, productGroupId.ToString());

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath = Path.Combine(filePath, fileName);

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }

                string tempFilePath = Path.Combine(ServiceConfig.PRODUCT_GROUP_FILES_DIR_TEMP, fileName);
                File.Move(tempFilePath, filePath);
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'MoveProductGroupFileToRightFolder'. fileName:{0}", fileName);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region RemoveProductGroupFile
        public void RemoveProductGroupFile(int productGroupId, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (productGroupId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            if (!fileName.StartsWith(productGroupId + "_"))
            {
                throw new ArgumentNullException("FileName does not contains product ID");
            }
            try
            {
                string filePath = Path.Combine(ServiceConfig.PRODUCT_GROUP_FILES_DIR, productGroupId.ToString());
                filePath = Path.Combine(filePath, fileName);

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RemoveProductGroupFile'. fileName:{0}", fileName);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region UpdateProductGroupDescription
        public void UpdateProductGroupDescription(ProductGroupDescription productGroupDescription)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();

                        ProductGroupDescription query = (from p in ctx.ProductGroupDescriptions
                                                         where p.ProductGroupID == productGroupDescription.ProductGroupID
                                                         select p).FirstOrDefault();

                        if (query == null)
                            query = new ProductGroupDescription();

                        query.Name = productGroupDescription.Name;
                        query.Description = productGroupDescription.Description;

                        if (query.ProductGroupID == 0)
                        {
                            ctx.ProductGroupDescriptions.InsertOnSubmit(query);
                        }

                        ctx.SubmitChanges();

                        ctx.Transaction.Commit();

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
                Logger.Exception(ex, "Error occured in 'UpdateProductGroupDescription'");
                throw ex;
            }
        }

        #endregion

        #region DeleteProductGroupDescription
        public void DeleteProductGroupDescription(int productGroupId)
        {
            if (productGroupId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var product = (from pd in ctx.ProductGroupDescriptions
                                   where pd.ProductGroupID == productGroupId
                                   select pd).FirstOrDefault();

                    ctx.ProductGroupDescriptions.DeleteOnSubmit(product);
                    ctx.SubmitChanges();

                    RemoveAllProductGroupFile(productGroupId);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'DeleteProductGroupDescription'");
                throw;
            }
        }

        #endregion

        #region RemoveAllProductGroupFile
        public void RemoveAllProductGroupFile(int productGroupId)
        {
            if (productGroupId <= 0)
            {
                throw new ArgumentNullException("productGroupId");
            }
            try
            {
                string filePath = Path.Combine(ServiceConfig.PRODUCT_GROUP_FILES_DIR, productGroupId.ToString());

                if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, true);
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RemoveAllProductGroupFile'. productGroupId:{0}", productGroupId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region RemoveAllProductFile
        public void RemoveAllProductFile(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            try
            {
                string filePath = Path.Combine(ServiceConfig.PRODUCT_FILES_DIR, productId.ToString());

                if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, true);
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RemoveAllProductFile'. productId:{0}", productId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetProductImageByFileName
        public byte[] GetProductImageByFileName(string fileName)
        {
            int index = fileName.IndexOf("_");
            int productID;
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (index < 0)
            {
                throw new ArgumentNullException("FileName does not contains product ID");
            }
            if (!int.TryParse(fileName.Substring(0, index), out productID))
            {
                throw new ArgumentNullException("FileName does not contains correct product ID");
            }
            try
            {
                string filePath = Path.Combine(ServiceConfig.PRODUCT_FILES_DIR, productID.ToString());
                filePath = Path.Combine(filePath, fileName);

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    return File.ReadAllBytes(filePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProductImageByFileName'. fileName:{0}", fileName);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetProductImageFileList
        public List<string> GetProductImageFileList(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            try
            {
                List<string> fileList = new List<string>();

                fileList = GetProductFileList(productId);

                fileList = fileList.Where(f => f.ToLower().EndsWith(".jpg") || f.ToLower().EndsWith(".jpeg") || f.ToLower().EndsWith(".png") || f.ToLower().EndsWith(".gif")).ToList();
                return fileList;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProductImageFileList'. productId:{0}", productId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetProductSpecSheetFileList
        public List<string> GetProductSpecSheetFileList(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }
            try
            {
                List<string> fileList = new List<string>();

                fileList = GetProductFileList(productId);

                fileList = fileList.Where(f => f.ToLower().EndsWith(".pdf")).ToList();
                return fileList;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProductSpecSheetFileList'. productId:{0}", productId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetGroupImageByFileName
        public byte[] GetGroupImageByFileName(string fileName)
        {
            int index = fileName.IndexOf("_");
            int groupID;
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (index < 0)
            {
                throw new ArgumentNullException("FileName does not contains product group ID");
            }
            if (!int.TryParse(fileName.Substring(0, index), out groupID))
            {
                throw new ArgumentNullException("FileName does not contains correct product group ID");
            }
            try
            {
                string filePath = Path.Combine(ServiceConfig.PRODUCT_GROUP_FILES_DIR, groupID.ToString());
                filePath = Path.Combine(filePath, fileName);

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    return File.ReadAllBytes(filePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetGroupImageByFileName'. fileName:{0}", fileName);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region SavePackage
        public void SavePackage(Abc.OnlineBL.Entities.Model.Myop.MyOwnPackageModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException("Package model is null");
                }
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();

                        List<EntityRelations> options = new List<EntityRelations>();
                        options.Add(EntityRelations.Product_To_ProductPricings);
                        options.Add(EntityRelations.ProductPricing_To_PricingType);
                        options.Add(EntityRelations.Product_To_ProductManagerExclusives);
                        options.Add(EntityRelations.Product_To_ProductRules);

                        ctx.DeferredLoadingEnabled = false;
                        if (options != null && options.Count > 0)
                            ctx.SetDataLoadOptions(options);

                        Product query;

                        if (model.ExistingMyopProductId > 0)
                        {
                            query = (from p in ctx.Products
                                     where p.ProductID == model.ExistingMyopProductId && p.RelatedClientId == model.ClientId
                                     && p.CategoryId == CategoryTypes.Packages && p.UsePackageContentPrice == true
                                     select p).FirstOrDefault();

                            if (query != null)
                            {
                                var pcks = from pck in ctx.PackageContentGroups
                                           where pck.ProductId == model.ExistingMyopProductId
                                           select pck;

                                foreach (var item in pcks)
                                {
                                    ctx.PackageContentGroups.DeleteOnSubmit(item);
                                    ctx.SubmitChanges();
                                }

                                query.PackageContentGroups = null;

                                foreach (var item in model.PackageItems)
                                {
                                    //build up package content group object
                                    PackageContentGroup pcg = new PackageContentGroup() { GroupName = item.CategoryName };
                                    //loop through item to build package content group product
                                    pcg.PackageContentGroupProducts = new System.Data.Linq.EntitySet<PackageContentGroupProduct>();
                                    pcg.PackageContentGroupProducts.Add(new PackageContentGroupProduct()
                                    {
                                        ProductId = item.ProductId,
                                        Qty = 1
                                    });
                                    query.PackageContentGroups.Add(pcg);
                                }

                                ctx.SubmitChanges();

                            }
                        }
                        else
                        {
                            query = new Product();

                            Client cl = (from c in ctx.Clients
                                         where c.ClientID == model.ClientId
                                         select c).FirstOrDefault();

                            if (cl != null)
                            {
                                query.Name = cl.ClientName + " - " + cl.Office + " - " + model.PackageName;
                                query.WebFriendlyName = cl.ClientName + " - " + cl.Office + " - " + model.PackageName;
                            }
                            query.RuleId = null;
                            query.TypeID = ProductTypes.Packages;
                            query.WorkflowTypeId = 4;
                            query.CategoryId = 4;
                            query.ShowOnWebSite = true;
                            query.Active = true;
                            query.Myop = true;
                            query.UsePackageContentPrice = true;
                            query.RelatedClientId = model.ClientId;
                            //query.ApplyGST = product.ApplyGST;
                            //query.SizeCode = product.SizeCode;
                            //query.Dimensions = product.Dimensions;
                            //query.ContentType = !string.IsNullOrEmpty(product.ContentType) ? product.ContentType : null;
                            //query.FrameType = !string.IsNullOrEmpty(product.FrameType) ? product.FrameType : null;
                            //query.Qty = product.Qty > 0 ? product.Qty : null;
                            //query.Format = !string.IsNullOrEmpty(product.Format) ? product.Format : null;
                            //query.CustomName = product.CustomName;
                            //query.IsPhotoItem = product.IsPhotoItem;
                            //query.IsStockboardItem = product.IsStockboardItem;
                            //query.ProductDescription = product.ProductDescription;
                            //query.ProductGroupID = product.ProductGroupID;

                            //if (product.ProductPricings != null && product.ProductPricings.Count > 0)
                            //{
                            //    query.ProductPricings = null;
                            //}
                            //foreach (var item in product.ProductPricings)
                            //{
                            //    query.ProductPricings.Add(new ProductPricing()
                            //    {
                            //        Price = item.Price,
                            //        ProductID = item.ProductID,
                            //        PricingID = item.PricingID
                            //    });
                            //}

                            //query.ProductManagerExclusives = null;
                            //foreach (var item in product.ProductManagerExclusives)
                            //{
                            //    query.ProductManagerExclusives.Add(new ProductManagerExclusive()
                            //    {
                            //        ManagerId = item.ManagerId,
                            //        ProductId = item.ProductId,
                            //    });
                            //}

                            query.ProductRules = new System.Data.Linq.EntitySet<ProductRule>();
                            query.ProductRules.Add(new ProductRule()
                            {
                                AllowOrDeny = true,
                                ClientId = model.ClientId,
                            });

                            query.PackageContentGroups = null;

                            foreach (var item in model.PackageItems)
                            {
                                //build up package content group object
                                PackageContentGroup pcg = new PackageContentGroup() { GroupName = item.CategoryName };
                                //loop through item to build package content group product
                                pcg.PackageContentGroupProducts = new System.Data.Linq.EntitySet<PackageContentGroupProduct>();
                                pcg.PackageContentGroupProducts.Add(new PackageContentGroupProduct()
                                {
                                    ProductId = item.ProductId,
                                    Qty = 1
                                });
                                query.PackageContentGroups.Add(pcg);
                            }

                            if (query.ProductID == 0)
                            {
                                ctx.Products.InsertOnSubmit(query);
                            }

                            ctx.SubmitChanges();

                            var priceList = (from pl in ctx.PriceLists
                                             join c in ctx.Clients on pl.PriceListID equals c.PriceListID
                                             where c.ClientID == model.ClientId
                                             select pl).FirstOrDefault();

                            if (priceList != null && priceList.IsComplete == true)
                            {
                                //insert product into PriceListDetail
                                PriceListDetail pld = new PriceListDetail();
                                pld.PriceListID = priceList.PriceListID;
                                pld.ProductID = query.ProductID;
                                //TODO: Change this value
                                pld.ProductPrice = 100M;
                                pld.IsCustom = false;
                                pld.Active = true;

                                ctx.PriceListDetails.InsertOnSubmit(pld);
                                ctx.SubmitChanges();
                            }
                        }

                        ctx.Transaction.Commit();
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
                Logger.Exception(ex, "Error occured in 'SavePackage'");
                throw ex;
            }
        }

        #endregion

        #region GetPDFTemplate
        public PDF_Template GetPDFTemplate(int productID, string sizeCode, string frameType, string contentType, string orientation)
        {
            //Logger.Warn("Get PDF Template Method: " + sizeCode); 
            //if (string.IsNullOrEmpty(sizeCode))
            //{
            //    throw new ArgumentNullException("sizeCode");
            //}

            if (string.IsNullOrEmpty(orientation))
            {
                throw new ArgumentNullException("orientation");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> options = new List<EntityRelations>();
                    options.Add(EntityRelations.PDF_Template_To_ProductSizeCode);

                    ctx.DeferredLoadingEnabled = false;
                    if (options != null && options.Count > 0)
                        ctx.SetDataLoadOptions(options);

                    if (productID == 1359 || productID == 4615 || productID == 3463 || productID == 8191 || productID == 8198)
                    {
                        var query = from p in ctx.PDF_Templates
                                    where p.ProductID == productID
                                    select p;
                        return query.FirstOrDefault();
                    }
                    if (sizeCode == "A3" || sizeCode == "PC")
                    {
                        var query = from p in ctx.PDF_Templates
                                    where p.SizeCode == sizeCode && p.Orientation == orientation
                                    select p;
                        return query.FirstOrDefault();
                    }
                    else if (sizeCode == "A4")
                    {
                        if (contentType == "Colour Front/Mono Back" || contentType == "Colour Front/Back")
                        {
                            var query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == "CC"
                                        select p;
                            return query.FirstOrDefault();
                        }
                        else if (contentType == "Colour Front/Back Booklet (4 Page) - Calendar Fold" || contentType == "Colour Front/Back Booklet (4 Page)")
                        {
                            var query = from p in ctx.PDF_Templates
                                         where p.SizeCode == sizeCode && p.ContentType != null && p.ContentType.ToLower() == contentType.ToLower() && p.FrameType.ToLower() == frameType.ToLower() && p.Orientation == orientation
                                         select p;
                            return query.FirstOrDefault();
                        }
                        else
                        {
                            if(string.IsNullOrEmpty(frameType))
                            {
                                var query = from p in ctx.PDF_Templates
                                            where p.SizeCode == sizeCode && p.ContentType != null && p.ContentType.ToLower() == contentType.ToLower() && p.FrameType == "" && p.Orientation == orientation
                                            select p;
                                if (query == null || query.FirstOrDefault() == null)
                                {
                                    query = from p in ctx.PDF_Templates
                                            where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == string.Empty
                                            select p;
                                }
                                return query.FirstOrDefault();
                            }
                            else
                            {
                                var query = from p in ctx.PDF_Templates
                                            where p.SizeCode == sizeCode && p.ContentType != null && p.ContentType.ToLower() == contentType.ToLower() && p.FrameType.ToLower() == frameType.ToLower() && p.Orientation == orientation
                                            select p;
                                if (query == null || query.FirstOrDefault() == null)
                                {
                                    query = from p in ctx.PDF_Templates
                                            where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == string.Empty
                                            select p;
                                }
                                return query.FirstOrDefault();
                            }
                        }
                    }
                    else if (sizeCode == "DL")
                    {
                        if (contentType == "Colour Front/Mono Back" || contentType == "Colour Front/Back")
                        {
                            var query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == "CC"
                                        select p;
                            return query.FirstOrDefault();
                        }
                        else
                        {
                            var query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == string.Empty
                                        select p;
                            return query.FirstOrDefault();
                        }
                    }
                    else if (sizeCode == "ABISign4x3" || sizeCode == "ABISign6x4" || sizeCode == "ABISign8x4")
                    {
                        if (contentType.ToLower().Contains("overlay"))
                        {
                            var query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == "Metal Board With Auction Overlay"
                                        select p;
                            return query.FirstOrDefault();
                        }
                        else
                        {
                            var query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType.ToLower() == frameType.ToLower()
                                        select p;
                            return query.FirstOrDefault();
                        }
                    }
                    else if (sizeCode == "C")
                    {
                        if (contentType.ToLower().Contains("overlay") && frameType == "Wrap")
                        {
                            var query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == "Wrap Board with Overlay"
                                        select p;
                            return query.FirstOrDefault();
                        }
                        if (contentType.ToLower().Contains("overlay") && frameType == "Wing Wrap")
                        {
                            var query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType == "Wing Wrap with Overlay"
                                        select p;
                            return query.FirstOrDefault();
                        }
                        else
                        {
                            var query = from p in ctx.PDF_Templates
                                    where p.SizeCode == sizeCode && p.ContentType != null && p.ContentType.ToLower() == contentType.ToLower() && p.FrameType.ToLower() == frameType.ToLower() && p.Orientation == orientation
                                    select p;
                            if (query == null || query.FirstOrDefault() == null)
                            {
                                query = from p in ctx.PDF_Templates
                                        where p.SizeCode == sizeCode && p.Orientation == orientation && p.FrameType.ToLower() == frameType.ToLower()
                                        select p;
                            }
                            return query.FirstOrDefault();
                        }
                    }
                    else
                    {
                        var query = from p in ctx.PDF_Templates
                                    where p.SizeCode == sizeCode && p.ContentType != null && p.ContentType.ToLower() == contentType.ToLower() && p.FrameType.ToLower() == frameType.ToLower() && p.Orientation == orientation
                                    select p;
                        if (query == null || query.FirstOrDefault() == null)
                        {
                            query = from p in ctx.PDF_Templates
                                    where p.SizeCode == sizeCode && p.FrameType.ToLower() == frameType.ToLower() && p.Orientation == orientation
                                    select p;
                        }
                        return query.FirstOrDefault();
                    }

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetPDFTemplate'. sizeCode:{0} frameType:{1} orientation:{2}", sizeCode, frameType, orientation);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetContentTypes
        public List<string> GetContentTypes()
        {
            try
            {
                var ctFile = System.IO.File.ReadAllLines(@"\\mercury\ABCFiles\FrontEnd_SetProduct\Contenttypes.txt");
                System.Collections.Generic.List<string> ct = new System.Collections.Generic.List<string>(ctFile);

                return ct;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetContentTypes'.");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetPriceListDetailsByPriceListIDAndProductID
        public decimal GetPriceListDetailsByPriceListIDAndProductID(int priceListId, int productId)
        {
            try
            {
                if (priceListId <= 0)
                {
                    throw new ArgumentNullException("priceListId");
                }
                if (productId <= 0)
                {
                    throw new ArgumentNullException("productId");
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from pl in ctx.PriceListDetails
                                where pl.PriceListID == priceListId && pl.ProductID == productId
                                select pl;

                    if (query != null && query.FirstOrDefault() != null)
                    {
                        return query.FirstOrDefault().ProductPrice * new Decimal(ServiceConfig.GST);
                    }
                    else
                        return 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetPriceListDetailsByPriceListIDAndProductID'. ProductId:{0}, PriceListID{1}.", productId, priceListId);
                throw;
            }
        }

        #endregion

        #region GetDronePhotographyAvailability
        public ProductAvailability GetDronePhotographyAvailability(int propertyID, List<EntityRelations> loadOptions)
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
                    if (pro != null)
                    {
                        var dl = ctx.DroneLocations.Where(loc => loc.Suburb.ToUpper() == pro.Location.Location1.ToUpper() && loc.PostCode == pro.Location.PostCode && loc.State.ToUpper() == pro.Location.State.ToUpper()).FirstOrDefault();
                        if(dl != null)
                        {
                            int status = dl.Status;
                            return (ProductAvailability)status;
                        }
                        return ProductAvailability.No;
                    }
                    return ProductAvailability.No;
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
    }
}
