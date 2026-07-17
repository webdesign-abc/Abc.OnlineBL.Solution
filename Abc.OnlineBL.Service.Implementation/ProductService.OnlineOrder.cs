using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Service.Implementation.BusinessLogic;
using Abc.OnlineBL.Entities.Model;
using System.Xml;
using System.Xml.Linq;

namespace Abc.OnlineBL.Service.Implementation
{
    public partial class ProductService
    {
        #region NEW GetAllOnlineProductsForExpressOrder
        public OnlineCategoryAndProductModel GetAllOnlineProductsForExpressOrder(int clientId)
        {
            OnlineCategoryAndProductModel retModel = new OnlineCategoryAndProductModel();

            List<OnlineProduct> ret = new List<OnlineProduct>();

            using (AbcDataContext ctx = new AbcDataContext())
            {
				 List<EntityRelations> loadOptions = new List<EntityRelations>();
				loadOptions.Add(EntityRelations.Client_To_Manager);
				loadOptions.Add(EntityRelations.Client_To_PriceList);

				ctx.DeferredLoadingEnabled = false;
				ctx.SetDataLoadOptions(loadOptions);
                var results = ctx.OnlineBL_GetAllAvailableOnlineOrderProductIds(clientId);
                var products = results.GetResult<OnlineBL_GetAllAvailableOnlineOrderProductIds_Products>().ToList();
                var pkgGroups = results.GetResult<PackageContentGroup>().ToList();
                var pkgGroupProducts = results.GetResult<OnlineBL_GetAllAvailableOnlineOrderProductIds_PackageProducts>().ToList();

				bool isWorkshop = false;
                bool showPriceOnWeb = false;
               

				Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == clientId);

                if (cl != null)
                {
                    if (cl.Manager != null)
                    {
                        isWorkshop = cl.Manager.IsWorkshop;
                    }

                    if (cl.PriceList != null)
                    {
                        if (isWorkshop)
                        {
                            showPriceOnWeb = cl.PriceList.ShowPricingOnWeb;
                        }
                    }
                }
				
                foreach (var product in products)
                {
                    var op = new OnlineProduct();

                    //setup product
                    SetupProduct(product, op);
										
                    //work out product price depending on the logic as in if price ir free(0) or to show TBA
                    WorkoutPrice(product, op, showPriceOnWeb);

                    //map attributes of the product
                    MapAttributes(product, op);

                    //fix package
                    if (product.CategoryId == CategoryTypes.Packages)
                    {
                        var relatedPG = from p in pkgGroups
                                        where p.ProductId == product.ProductId
                                        select p;
                        foreach (var pg in relatedPG)
                        {
                            PackageGroup pgModel = new PackageGroup();
                            pgModel.GroupId = pg.GroupId;
                            pgModel.GroupName = pg.GroupName;
                            op.PackageGroups.Add(pgModel);

                            //fix package contents
                            var relatedPGC = from p in pkgGroupProducts
                                             where p.GroupId == pg.GroupId
                                             select p;
                            foreach (var pgc in relatedPGC)
                            {
                                PackageContentProduct pgcModel = new PackageContentProduct();
                                pgcModel.UniqueId = pgc.PackageContentGroupId;
                                pgcModel.PkgQty = pgc.Qty.HasValue ? pgc.Qty.Value : 0;
                                pgcModel.ItemNotes = pgc.ItemNotes;
                                pgcModel.PkgFormat = pgc.Format;

                                //setup product
                                SetupProduct(pgc, pgcModel);

                                //work out product price depending on the logic as in if price ir free(0) or to show TBA
                                WorkoutPrice(pgc, pgcModel, showPriceOnWeb);

                                //map attributes of the product
                                MapAttributes(pgc, pgcModel);

                                pgModel.Products.Add(pgcModel);
                            }
                        }
                    }
                    ret.Add(op);
                    if (!string.IsNullOrEmpty(op.Tags))
                    {
                        op.Tags = op.Tags.Replace(" ,", "").TrimEnd(',');
                    }
                    retModel.AddOnlineOrderProduct(op, product);
                }
            }

            retModel.Products = ret;
            return retModel;
        }

        public OnlineCategoryAndProductModel GetAllRegularOnlineProductsForExpressOrder(int clientId)
        {
            OnlineCategoryAndProductModel retModel = new OnlineCategoryAndProductModel();

            List<OnlineProduct> ret = new List<OnlineProduct>();

            using (AbcDataContext ctx = new AbcDataContext())
            {
				 List<EntityRelations> loadOptions = new List<EntityRelations>();
				loadOptions.Add(EntityRelations.Client_To_Manager);
				loadOptions.Add(EntityRelations.Client_To_PriceList);

				ctx.DeferredLoadingEnabled = false;
				ctx.SetDataLoadOptions(loadOptions);
                //cal regular products SP instead of normal order products SP
                var results = ctx.AIS_GetAllAvailableRegularOnlineOrderProductIds(clientId);

                var products = results.GetResult<OnlineBL_GetAllAvailableOnlineOrderProductIds_Products>().ToList();
                var pkgGroups = results.GetResult<PackageContentGroup>().ToList();
                var pkgGroupProducts = results.GetResult<OnlineBL_GetAllAvailableOnlineOrderProductIds_PackageProducts>().ToList();

				bool isWorkshop = false;
                bool showPriceOnWeb = false;
                
				Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == clientId);

                if (cl != null)
                {
                    if (cl.Manager != null)
                    {
                        isWorkshop = cl.Manager.IsWorkshop;
                    }

                    if (cl.PriceList != null)
                    {
                        if (isWorkshop)
                        {
                            showPriceOnWeb = cl.PriceList.ShowPricingOnWeb;
                        }
                    }
                }
				
                foreach (var product in products)
                {
                    var op = new OnlineProduct();

                    //setup product
                    SetupProduct(product, op);

                    //work out product price depending on the logic as in if price ir free(0) or to show TBA
                    WorkoutPrice(product, op, showPriceOnWeb);

                    //map attributes of the product
                    MapAttributes(product, op);

                    //fix package
                    if (product.CategoryId == CategoryTypes.Packages)
                    {
                        var relatedPG = from p in pkgGroups
                                        where p.ProductId == product.ProductId
                                        select p;
                        foreach (var pg in relatedPG)
                        {
                            PackageGroup pgModel = new PackageGroup();
                            pgModel.GroupId = pg.GroupId;
                            pgModel.GroupName = pg.GroupName;
                            op.PackageGroups.Add(pgModel);

                            //fix package contents
                            var relatedPGC = from p in pkgGroupProducts
                                             where p.GroupId == pg.GroupId
                                             select p;
                            foreach (var pgc in relatedPGC)
                            {
                                PackageContentProduct pgcModel = new PackageContentProduct();
                                pgcModel.UniqueId = pgc.PackageContentGroupId;
                                pgcModel.PkgQty = pgc.Qty.HasValue ? pgc.Qty.Value : 0;
                                pgcModel.ItemNotes = pgc.ItemNotes;
                                pgcModel.PkgFormat = pgc.Format;

                                //setup product
                                SetupProduct(pgc, pgcModel);

                                //work out product price depending on the logic as in if price ir free(0) or to show TBA
                                WorkoutPrice(pgc, pgcModel, showPriceOnWeb);

                                //map attributes of the product
                                MapAttributes(pgc, pgcModel);

                                pgModel.Products.Add(pgcModel);
                            }
                        }
                    }
                    ret.Add(op);
                    if (!string.IsNullOrEmpty(op.Tags))
                    {
                        op.Tags = op.Tags.Replace(" ,", "").TrimEnd(',');
                    }
                    retModel.AddOnlineOrderProduct(op, product);
                }
            }

            retModel.Products = ret;
            return retModel;
        }

        private static void SetupProduct(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products product, OnlineProduct op)
        {
            op.ProductGuid = Guid.NewGuid().ToString();
            op.ProductId = product.ProductId;
            op.ProductName = product.Name;
            op.WebFriendlyName = product.WebFriendlyName;
            op.Description = product.ProductDescription;
            op.OnlineCategoryId = product.CategoryId;
            op.CategoryName = product.CategoryName;
            op.TypeId = product.TypeID;
            op.IsMyop = product.Myop;
            op.UsePackageContentPrice = product.UsePackageContentPrice;
            op.FavProductId = product.FavId;
            op.Tags = op.CategoryName + ", " + op.WebFriendlyName + ", " + (product.ProductDescription ?? string.Empty) + ", " + (product.CustomDescription ?? string.Empty) + ",";
        }

        private static void WorkoutPrice(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products product, OnlineProduct op, Boolean showPriceOnWeb)
        {

            //TODO: Check to see if product is corflute then TBA
            if (product.CategoryId == CategoryTypes.Corflute)
            {

            }
            else
            {
                if (product.ProductPrice.HasValue && showPriceOnWeb)
                {
                    if (product.ProductPrice > 0)
                    {
                        if (product.ApplyGST)
                        {
                            product.ProductPrice = product.ProductPrice * new Decimal(ServiceConfig.GST);
                        }
                        op.PriceDisplay = "$" + product.ProductPrice.Value.ToString("F");
                        op.ProductPrice = product.ProductPrice.Value;
                        op.TotalPrice = op.ProductPrice;
                    }
                    else if (product.ProductPrice == 0)
                    {
                        //If custom pricing then display as 0 (product is free)
                        if (product.IsCustomPrice.HasValue == true && product.IsCustomPrice.Value == true)
                        {
                            if (product.ApplyGST)
                            {
                                product.ProductPrice = product.ProductPrice * new Decimal(ServiceConfig.GST);
                            }
                            op.PriceDisplay = "$" + product.ProductPrice.Value.ToString("F");
                            op.ProductPrice = product.ProductPrice.Value;
                            op.TotalPrice = op.ProductPrice;
                        }
                    }
                }
                //custom price list name rule
                //if (!string.IsNullOrEmpty(product.CustomDescription))
                //{
                //    op.CustomDesc = product.CustomDescription;
                //    //replace product name with custom name
                //    op.ProductName = op.CustomDesc;
                //    op.WebFriendlyName = op.CustomDesc;
                //}
            }
        }

        private static void MapAttributes(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products product, OnlineProduct op)
        {
            if (op.Tags == null)
            {
                op.Tags = string.Empty;
            }
            if (product.CategoryId.In(1, 2, 3, 5, 14, 15, 16, 17))
            {
                if (!string.IsNullOrEmpty(product.ContentType))
                {
                    op.Attributes.Add(new AbcKeyValuePair() { Key = "ContentType", Value = product.ContentType });
                    op.ContentType = product.ContentType;
                    op.Tags += op.ContentType + ", ";
                }
                if (!string.IsNullOrEmpty(product.Format))
                {
                    op.Attributes.Add(new AbcKeyValuePair() { Key = "Format", Value = product.Format });
                    op.Format = product.Format;
                    op.Tags += op.Format + ", ";
                }
                if (!string.IsNullOrEmpty(product.SizeCodeOnWeb))
                {
                    op.Attributes.Add(new AbcKeyValuePair() { Key = "SizeCode", Value = product.SizeCodeOnWeb });
                    op.SizeCode = product.SizeCodeOnWeb;
                    op.Tags += op.SizeCode + ", ";
                }
            }
            if (!string.IsNullOrEmpty(product.FrameType) && product.CategoryId.In(1, 2, 14))
            {
                op.Attributes.Add(new AbcKeyValuePair() { Key = "FrameType", Value = product.FrameType });
                op.FrameType = product.FrameType;
                op.Tags += op.FrameType + ", ";
            }
            if (product.Qty > 0 && product.CategoryId.In(3))
            {
                op.Attributes.Add(new AbcKeyValuePair() { Key = "Qty", Value = product.Qty.ToString() });
                op.Qty = product.Qty.ToString();
                if (product.Qty > 1)
                {
                    op.Tags += "Qty - " + op.Qty + ", ";
                }
            }
            if (!string.IsNullOrEmpty(product.CustomName) && product.CategoryId.In(2))
            {
                op.Attributes.Add(new AbcKeyValuePair() { Key = "CustomName", Value = product.CustomName });
                op.CustomName = product.CustomName;
                op.Tags += op.CustomName + ", ";
            }
        }
        #endregion

        #region OnlineProduct
        public OnlineProduct PopulateConfigAndMatchingTemplates(int clientId, OnlineProduct onlineProduct)
        {
            List<EntityRelations> loadOptions = new List<EntityRelations>();
            loadOptions.Add(EntityRelations.Product_To_OnlineOrderCategory);
            loadOptions.Add(EntityRelations.Product_To_ProductSizeCode);
            loadOptions.Add(EntityRelations.Product_To_PackageContentGroups);
            loadOptions.Add(EntityRelations.PackageContentGroup_To_PackageContentGroupProducts);
            loadOptions.Add(EntityRelations.Product_To_PriceListDetails);
            loadOptions.Add(EntityRelations.PriceListDetail_To_PriceList);
            loadOptions.Add(EntityRelations.Product_To_ProductType);
            loadOptions.Add(EntityRelations.Product_To_ProductPricings);
            loadOptions.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
            //loadOptions.Add(EntityRelations.AOP_TemplateProduct_To_AOP_DocumentPolicy);
            OnlineProductHelper helper = new OnlineProductHelper();

            var groupId = 0;

            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                Client cl = (from c in ctx.Clients
                             where c.ClientID == clientId
                             select c).First();

                groupId = cl.GroupId;

                var dbProduct = (from p in ctx.Products
                                 where p.ProductID == onlineProduct.ProductId
                                 select p).FirstOrDefault();

                if (dbProduct == null)
                    return onlineProduct;

                IQueryable<AOP_TemplateProduct> availableTemplates = from tm in ctx.AOP_TemplateProducts
                                                                     where tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId
                                                                     select tm;

                IQueryable<AOP_TemplateProduct> corporateTemplates = from tm in ctx.AOP_TemplateProducts
                                                                     where tm.AOP_Template.GroupId == groupId && tm.AOP_Template.ClientId == null
                                                                     select tm;
                
                if (availableTemplates != null && availableTemplates.Count() == 0) //if we don't have any client level template then look at group level
                {
                    availableTemplates = corporateTemplates;
                }

                var cpHasSBDIY = ctx.ClientsPrefs.Where(x => x.ClientId == clientId && x.PrefID == Abc.OnlineBL.Entities.Utility.Preferences.StockboardDIY).FirstOrDefault();
                bool clientHasStockboardDIY = false;
                if (cpHasSBDIY != null && cpHasSBDIY.BitValue.HasValue && cpHasSBDIY.BitValue == true)
                    clientHasStockboardDIY = true;

                if (!clientHasStockboardDIY)
                {
                    availableTemplates = availableTemplates.Where(a => a.Type != "Stockboard" && a.Type != "Stockboard Overlay").AsQueryable();
                    corporateTemplates = corporateTemplates.Where(a => a.Type != "Stockboard" && a.Type != "Stockboard Overlay").AsQueryable();
                }

                //If client need to proof artwork then order should not be DIY
                var cpref = ctx.ClientsPrefs.SingleOrDefault(x => x.ClientId == clientId && x.PrefID == ClientsPref.ClientNeedToProofArtwork);
                if (cpref != null)
                {
                    if (cpref.BitValue.HasValue && cpref.BitValue.Value == true)
                    {

                        availableTemplates = availableTemplates.Where(a => a.Format == "n/a").AsQueryable();
                        corporateTemplates = corporateTemplates.Where(a => a.Format == "n/a").AsQueryable();
                    }
                }

                decimal solarPanelPrice = 150;
                var pld = (from pl in ctx.PriceListDetails
                           where pl.PriceListID == cl.PriceListID && pl.ProductID == ServiceConfig.SOLAR_PANEL_PRODUCT_ID
                           select pl).FirstOrDefault();

                if (pld != null && pld.ProductPrice >= 0)
                {
                    solarPanelPrice = pld.ProductPrice * new Decimal(ServiceConfig.GST);
                }

                //if this is a package the check if any group exists or not.. else not to include this in the list
                //if (product.OnlineOrderCategory.CategoryName.ToLower() == "packages")
                if (onlineProduct.OnlineCategoryId == CategoryTypes.Packages)
                {
                    if (onlineProduct.PackageGroups.Count > 0)
                    {
                        foreach (var pkgGroup in onlineProduct.PackageGroups)
                        {
                            foreach (var item in pkgGroup.Products)
                            {
                                var dbPkgProduct = (from pp in ctx.PackageContentGroupProducts
                                                    where pp.PackageContentGroupId == item.UniqueId
                                                    select pp).FirstOrDefault();

                                var dbPkgProductMain = (from pp in ctx.Products
                                                        where pp.ProductID == dbPkgProduct.ProductId
                                                        select pp).FirstOrDefault();

                                if (dbPkgProduct != null && dbPkgProductMain != null)
                                {
                                    //product xml has precedence, then category xml config
                                    item.XmlConfig = dbPkgProductMain.ProductFormConfig;
                                    //if (string.IsNullOrEmpty(item.XmlConfig)) //else lets try category level config
                                    //    item.XmlConfig = dbPkgProductMain.OnlineOrderCategory.XmlFormConfig;

                                    if (string.IsNullOrEmpty(item.XmlConfig)) //else lets try category level config
                                    {
                                        if (item.OnlineCategoryId == CategoryTypes.Billboard && !string.IsNullOrEmpty(item.FrameType) && item.FrameType.ToLower() == "light board")
                                        {
                                            item.XmlConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>  <ProductConfig>   <fields>    <field>     <fieldName>Qty</fieldName>     <caption>Qty</caption>     <helpText>Enter Product Quantity</helpText>     <getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value>1</value>     <fieldType>TextBox</fieldType>     <validation required=""true"">      <rangeValidation min=""1"" max=""100""/>     </validation>    </field>    <field>     <fieldName>Format</fieldName>     <caption>Format</caption>     <helpText>Select Orientation</helpText>     <getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value></value>     <fieldType>ComboBox</fieldType>     <listItems>      <listItem displayText=""Portrait"" valueText=""Portrait"" />      <listItem displayText=""Landscape"" valueText=""Landscape"" />     </listItems>     <validation required=""true""></validation>    </field>   <field>    <fieldName>SolarPanel</fieldName>     <caption>Solar Panel</caption>     <helpText>Select Solar Panel</helpText>     <getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value></value>     <fieldType>ComboBox</fieldType>     <listItems>      <listItem displayText=""Without Solar Panel"" valueText=""0"" />      <listItem displayText=""With Solar Panel $150 Additional"" valueText=""1"" />      </listItems>     <validation required=""true""></validation>    </field>	</fields>	</ProductConfig>";
                                            item.SolarPanelPrice = solarPanelPrice;
                                        }
                                        else
                                        {
                                            item.XmlConfig = dbPkgProductMain.OnlineOrderCategory.XmlFormConfig;
                                        }

                                    }

                                    //remove method if product rule deny solar panel
                                    if (item.OnlineCategoryId == CategoryTypes.Billboard && !string.IsNullOrEmpty(item.FrameType) && item.FrameType.ToLower() == "light board")
                                    {
                                        ProductConfig pc = ProductConfig.GetFromString(item.XmlConfig);
                                        var field = pc.Fields.Field.Find(f => f.FieldName.ToLower() == "solarpanel");
                                        var result = ctx.AIS_EvaluateProductRuleOnProduct(clientId, ServiceConfig.SOLAR_PANEL_PRODUCT_ID);
                                        if (field != null && (result == null || result.Count() <= 0))
                                        {
                                            pc.Fields.Field.Remove(field);
                                        }
                                        
                                        item.XmlConfig = ProductConfig.GetStringFromObject(pc);
                                    }

                                    //remove method base on onlineproduct.Format
                                    if (onlineProduct.TypeId == ProductTypes.BillBoard && !string.IsNullOrEmpty(onlineProduct.Format) && !string.IsNullOrEmpty(onlineProduct.XmlConfig))
                                    {
                                        ProductConfig pc = ProductConfig.GetFromString(onlineProduct.XmlConfig);
                                        if (pc.Fields != null && pc.Fields.Field != null && pc.Fields.Field.Count > 0)
                                        {
                                            var field = pc.Fields.Field.Find(f => f.FieldName.ToLower() == "format");
                                            if (field != null && field.ListItems != null && field.ListItems.ListItem.Count > 0)
                                            {
                                                field.ListItems.ListItem.RemoveAll(l => l.DisplayText.ToLower() != onlineProduct.Format.ToLower());
                                            }
                                        }

                                        onlineProduct.XmlConfig = ProductConfig.GetStringFromObject(pc);
                                    }

                                    if (item.OnlineCategoryId == CategoryTypes.Stockboard && !string.IsNullOrEmpty(item.XmlConfig))
                                    {
                                        ProductConfig pc = ProductConfig.GetFromString(item.XmlConfig);
                                        if (pc.Fields != null && pc.Fields.Field != null && pc.Fields.Field.Count > 0)
                                        {
                                            var field = pc.Fields.Field.Find(f => f.FieldName.ToLower() == "qty");
                                            if (field != null && field.Validation != null && field.Validation.RangeValidation != null)
                                            {
                                                field.Value = dbPkgProduct.Qty.ToString();
                                                field.Validation.RangeValidation.Min = dbPkgProduct.Qty;
                                                field.Validation.RangeValidation.Max = dbPkgProduct.Qty;
                                            }
                                        }

                                        item.XmlConfig = ProductConfig.GetStringFromObject(pc);
                                    }

                                    if (dbProduct.OnlineOrderCategory.MayContainDIYProducts)
                                    {
                                        MatchSingleProductDIYTemplates(item, dbPkgProductMain, availableTemplates);
                                        if (item.DIYTemplates.Count < 1)
                                        {
                                            MatchSingleProductDIYTemplates(item, dbPkgProductMain, corporateTemplates);
                                        }
                                    }
                                }
                            }
                        }
                        onlineProduct.ProductGroupsHaveAvailableTemplate = onlineProduct.ProductHasDIYTemplateOnAllGroup();
                    }
                }
                else
                {
                    //product xml has precedence, then category xml config
                    onlineProduct.XmlConfig = dbProduct.ProductFormConfig;
                    //if (string.IsNullOrEmpty(onlineProduct.XmlConfig)) //else lets try category level config
                    //    onlineProduct.XmlConfig = dbProduct.OnlineOrderCategory.XmlFormConfig;

                    if (string.IsNullOrEmpty(onlineProduct.XmlConfig)) //else lets try category level config
                    {
                        if (onlineProduct.OnlineCategoryId == CategoryTypes.Billboard && !string.IsNullOrEmpty(onlineProduct.FrameType) && onlineProduct.FrameType.ToLower() == "light board")
                        {
                            onlineProduct.XmlConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>  <ProductConfig>   <fields>    <field>     <fieldName>Qty</fieldName>     <caption>Qty</caption>     <helpText>Enter Product Quantity</helpText>     <getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value>1</value>     <fieldType>TextBox</fieldType>     <validation required=""true"">      <rangeValidation min=""1"" max=""100""/>     </validation>    </field>    <field>     <fieldName>Format</fieldName>     <caption>Format</caption>     <helpText>Select Orientation</helpText>     <getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value></value>     <fieldType>ComboBox</fieldType>     <listItems>      <listItem displayText=""Portrait"" valueText=""Portrait"" />      <listItem displayText=""Landscape"" valueText=""Landscape"" />     </listItems>     <validation required=""true""></validation>    </field>   <field>    <fieldName>SolarPanel</fieldName>     <caption>Solar Panel</caption>     <helpText>Select Solar Panel</helpText>     <getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value></value>     <fieldType>ComboBox</fieldType>     <listItems>      <listItem displayText=""Without Solar Panel"" valueText=""0"" />      <listItem displayText=""With Solar Panel $150 Additional"" valueText=""1"" />      </listItems>     <validation required=""true""></validation>    </field>	</fields>	</ProductConfig>";
                        }
                        else
                        {
                            onlineProduct.XmlConfig = dbProduct.OnlineOrderCategory.XmlFormConfig;
                        }

                    }

                    //remove method if product rule deny solar panel
                    if (onlineProduct.OnlineCategoryId == CategoryTypes.Billboard && !string.IsNullOrEmpty(onlineProduct.FrameType) && onlineProduct.FrameType.ToLower() == "light board")
                    {
                        ProductConfig pc = ProductConfig.GetFromString(onlineProduct.XmlConfig);
                        if (!string.IsNullOrEmpty(onlineProduct.FrameType) && onlineProduct.FrameType.ToLower() == "light board")
                        {
                            var field = pc.Fields.Field.Find(f => f.FieldName.ToLower() == "solarpanel");
                            var result = ctx.AIS_EvaluateProductRuleOnProduct(clientId, ServiceConfig.SOLAR_PANEL_PRODUCT_ID);
                            if (field != null && (result == null || result.Count() <= 0))
                            {
                                pc.Fields.Field.Remove(field);
                            }

                            onlineProduct.XmlConfig = ProductConfig.GetStringFromObject(pc);
                        }
                    }

                    //remove method base on onlineproduct.Format
                    if (onlineProduct.TypeId == ProductTypes.BillBoard && !string.IsNullOrEmpty(onlineProduct.Format) && !string.IsNullOrEmpty(onlineProduct.XmlConfig))
                    {
                        ProductConfig pc = ProductConfig.GetFromString(onlineProduct.XmlConfig);
                        if (pc.Fields != null && pc.Fields.Field != null && pc.Fields.Field.Count > 0)
                        {
                            var field = pc.Fields.Field.Find(f => f.FieldName.ToLower() == "format");
                            if (field != null && field.ListItems != null && field.ListItems.ListItem.Count > 0)
                            {
                                field.ListItems.ListItem.RemoveAll(l => l.DisplayText.ToLower() != onlineProduct.Format.ToLower());
                            }
                        }

                        onlineProduct.XmlConfig = ProductConfig.GetStringFromObject(pc);
                    }

                    if (dbProduct.OnlineOrderCategory.MayContainDIYProducts)
                    {

                        MatchSingleProductDIYTemplates(onlineProduct, dbProduct, availableTemplates);

                        if (onlineProduct.DIYTemplates.Count < 1)
                        {
                            MatchSingleProductDIYTemplates(onlineProduct, dbProduct, corporateTemplates);
                        }
                    }
                }
            }
            return onlineProduct;
        }

        private void MatchSingleProductDIYTemplates(OnlineProduct model, Product product, IQueryable<AOP_TemplateProduct> avlTemplates)
        {

            if (string.IsNullOrEmpty(model.SizeCode))
                return;

            var temp = avlTemplates;

            var query = from tm in temp
                        where tm.Type == product.ProductType.Type && tm.ContentType == product.ContentType && tm.SizeCode == model.SizeCode
                        select tm;

            if (!string.IsNullOrEmpty(product.FrameType))
                query = from q in query
                        where q.FrameType == product.FrameType
                        select q;

            if (!string.IsNullOrEmpty(product.Format))
                query = from q in query
                        where q.Format == product.Format
                        select q;

            foreach (var item in query)
            {
                DIYTemplate tmpl = new DIYTemplate();
                tmpl.ProductTemplateId = item.TemplateProductId;
                tmpl.TemplateName = item.Name;
                tmpl.TemplateFormat = item.Format;
                tmpl.TemplateDescription = item.Description;
                //tmpl.Layers = ParseLayers(item.AOP_Template.TemplateModel);
                model.DIYTemplates.Add(tmpl);
            }
        }

        private List<LayerContainer> ParseLayers(XElement xElement)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xElement.ToString()); //
            XmlNodeList xnList = xml.SelectNodes("/Document/Layers/Layer");
            var layers = new List<Abc.OnlineBL.Entities.Model.OnlineOrder.Layer>(xnList.Count);

            foreach (XmlNode xn in xnList)
            {
                var layer = new Abc.OnlineBL.Entities.Model.OnlineOrder.Layer
                {
                    Id = int.Parse(xn["Id"].InnerText),
                    Locked = bool.Parse(xn["Locked"].InnerText),
                    Visible = bool.Parse(xn["Visible"].InnerText),
                    Name = xn["Name"].InnerText
                };

                var tokens = layer.Name.Split('_');
                if (tokens.Length == 3)
                {
                    layer.Group = tokens[1];
                    layer.Value = tokens[2];
                    layers.Add(layer);
                }
            }

            var dic = layers
                .GroupBy(l => l.Group)
                .ToDictionary(grp => grp.Key, grp => grp.ToList());

            var res = new List<LayerContainer>(dic.Count);

            foreach (var item in dic.Keys)
            {
                res.Add(new LayerContainer
                {
                    Name = item,
                    Layers = dic[item]
                });
            }

            return res;
        }

        #endregion

        #region IProductService Members

        public List<OnlineProduct> GetOnlineProductsByCategoryId(int clientId, int categoryId, int? subCategoryId, bool returnDIYOnly)
        {
            List<OnlineProduct> ret = new List<OnlineProduct>();
            List<EntityRelations> loadOptions = new List<EntityRelations>();
            loadOptions.Add(EntityRelations.Product_To_OnlineOrderCategory);
            loadOptions.Add(EntityRelations.Product_To_ProductSizeCode);
            loadOptions.Add(EntityRelations.Product_To_PackageContentGroups);
            loadOptions.Add(EntityRelations.PackageContentGroup_To_PackageContentGroupProducts);
            loadOptions.Add(EntityRelations.Product_To_PriceListDetails);
            loadOptions.Add(EntityRelations.PriceListDetail_To_PriceList);
            loadOptions.Add(EntityRelations.Product_To_ProductType);
            loadOptions.Add(EntityRelations.Product_To_ProductPricings);
            loadOptions.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
            //loadOptions.Add(EntityRelations.AOP_TemplateProduct_To_AOP_DocumentPolicy);
            OnlineProductHelper helper = new OnlineProductHelper();

            var groupId = 0;

            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.CommandTimeout = ctx.CommandTimeout * 2;

                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                Client cl = (from c in ctx.Clients
                             where c.ClientID == clientId
                             select c).First();

                groupId = cl.GroupId;

                var products = from p in ctx.AIS_GetAvailableOnlineOrderProducts(clientId, categoryId)
                               select p;

                List<AOP_TemplateProduct> tempTP = (from tm in ctx.AOP_TemplateProducts
                                                    where tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId
                                                    select tm).ToList();

                List<AOP_TemplateProduct> corporateTemplates = (from tm in ctx.AOP_TemplateProducts
                                                                where tm.AOP_Template.GroupId == groupId && tm.AOP_Template.ClientId == null
                                                                select tm).ToList();

                if (tempTP != null && tempTP.Count() == 0) //if we don't have any client level template then look at group level
                {
                    tempTP = corporateTemplates;
                }

                bool clientHasOwnAvailableTemplate = false;
                if (tempTP != null && tempTP.Count > 0)
                {
                    clientHasOwnAvailableTemplate = true;
                }

                foreach (Product product in products)
                {
                    //if this is a package the check if any group exists or not.. else not to include this in the list
                    //if (product.OnlineOrderCategory.CategoryName.ToLower() == "packages")
                    if (categoryId == CategoryTypes.Packages)
                    {
                        if (product.PackageContentGroups.Count == 0)
                        {
                            continue;
                        }
                    }

                    //lets create the equivanet model object
                    OnlineProduct model = helper.SettingupOnlineProduct(groupId, clientId, ctx, product, new OnlineProduct(), returnDIYOnly, cl.PriceListID, false, tempTP, corporateTemplates);

                    //Check if client has their own available template
                    //if (returnDIYOnly && clientHasOwnAvailableTemplate &&
                    //    ((product.TypeID == ProductTypes.BillBoard && (string.IsNullOrEmpty(product.ContentType)
                    //                                                    || !product.ContentType.Contains("Community Board")))
                    //    || product.TypeID == ProductTypes.Brochure || product.TypeID == ProductTypes.WindowCard))
                    //{
                    //    //check if Product has their own template otherwise continue
                    //    if (helper.ProductDoesNotHasMatchClientDIYTemplates(groupId, clientId, product, model, tempTP))
                    //        continue;
                    //}

                    //packages
                    if (categoryId == CategoryTypes.Packages)
                    {
                        if (product.PackageContentGroups.Count > 0)
                        {
                            helper.SetupPackages(groupId, clientId, model, product, ctx, returnDIYOnly, cl.PriceListID);

                            /////
                            //Check if client has their own available template
                            if (returnDIYOnly && helper.ClientHasOwnAvailableTemplate(groupId, clientId, ctx))
                            {

                                //packages
                                if (product.PackageContentGroups.Count > 0)
                                {
                                    bool notHaveTemplates = false;
                                    foreach (var item in product.PackageContentGroups)
                                    {
                                        bool groupHaveTemplate = false;
                                        foreach (var pkgContentProd in item.PackageContentGroupProducts)
                                        {
                                            var dbProduct = (from p in ctx.Products
                                                             where p.ProductID == pkgContentProd.ProductId
                                                             select p).FirstOrDefault();
                                            if (dbProduct != null)
                                            {
                                                Abc.OnlineBL.Entities.Model.OnlineOrder.PackageContentProduct pkgProduct = new PackageContentProduct();
                                                pkgProduct.UniqueId = pkgContentProd.PackageContentGroupId;
                                                pkgProduct.PkgQty = pkgContentProd.Qty;
                                                pkgProduct.ItemNotes = pkgContentProd.ItemNotes;
                                                pkgProduct.PkgFormat = pkgContentProd.Format;
                                                pkgProduct = (PackageContentProduct)helper.SetupOnlineProduct(groupId, clientId, ctx, dbProduct, pkgProduct, returnDIYOnly, cl.PriceListID, true);
                                                if (!helper.ProductNotHasMatchClientDIYTemplates(groupId, clientId, dbProduct, ctx, pkgProduct))
                                                {
                                                    groupHaveTemplate = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!groupHaveTemplate)
                                        {
                                            notHaveTemplates = true;
                                            break;
                                        }
                                    }
                                    if (!notHaveTemplates)
                                    {
                                        model.ProductGroupsHaveAvailableTemplate = true;
                                    }

                                }
                            }
                        }
                    }

                    if (product.OnlineOrderCategory.MayContainDIYProducts)
                    {
                        if (returnDIYOnly)
                        {
                            //if we are asking for DIY products and product or package content doesn't have any DIY then to return this product, rather
                            //cotinue through the rest of the products in the loop
                            //if (model.DIYTemplates.Count <= 0)
                            //	continue;
                            if (!model.ProductHasAvailableDIYTemplates() && (model.CategoryName.ToLower() != "packages"))
                                continue;
                        }
                    }

                    ret.Add(model);
                }

            }

            return ret;
        }

        public List<OnlineProduct> GetOptionalProducts(int clientId)
        {
            if (clientId < 0)
                return null;

            List<OnlineProduct> ret = new List<OnlineProduct>();
            List<EntityRelations> loadOptions = new List<EntityRelations>();
            loadOptions.Add(EntityRelations.Product_To_OnlineOrderCategory);
            OnlineProductHelper helper = new OnlineProductHelper();

            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                var products = from p in ctx.AIS_GetOptionalProducts(clientId)
                               select p;

                foreach (Product product in products)
                {
                    //lets create the equivanet model object
                    OnlineProduct model = helper.SetupOptionalProduct(product, new OnlineProduct());

                    ret.Add(model);
                }
            }

            return ret;
        }

        #endregion

        #region Deprecated
        public List<OnlineProduct> GetRelatedOnlineProductsByCategoryId(int clientId, List<int> orderdProductId)
        {
            if (orderdProductId == null)
                return null;

            if (orderdProductId.Count == 0)
                return null;

            string joinedIds = string.Join(",", orderdProductId.Select(x => x.ToString()).ToArray());

            List<OnlineProduct> ret = new List<OnlineProduct>();
            List<EntityRelations> loadOptions = new List<EntityRelations>();
            loadOptions.Add(EntityRelations.Product_To_OnlineOrderCategory);
            loadOptions.Add(EntityRelations.Product_To_ProductSizeCode);
            loadOptions.Add(EntityRelations.Product_To_PackageContentGroups);
            loadOptions.Add(EntityRelations.PackageContentGroup_To_PackageContentGroupProducts);
            loadOptions.Add(EntityRelations.Product_To_PriceListDetails);
            loadOptions.Add(EntityRelations.Product_To_ProductType);
            OnlineProductHelper helper = new OnlineProductHelper();

            var groupId = 0;

            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                groupId = (from c in ctx.Clients
                           where c.ClientID == clientId
                           select c.GroupId).First();

                var products = from p in ctx.AIS_GetRelatedProducts(clientId, joinedIds)
                               select p;

                foreach (Product product in products)
                {
                    //last minute check, although OnlineBL will never return something that has not CategoryId
                    if (!product.CategoryId.HasValue)
                        continue;

                    //TODO: Turn this on from OnlineBL when DB records are all sorted out
                    if (!product.ShowOnWebSite)
                        continue;

                    //if this is a package the check if any group exists or not.. else not to include this in the list
                    if (product.OnlineOrderCategory.CategoryName.ToLower() == "packages")
                    {
                        if (product.PackageContentGroups.Count == 0)
                        {
                            continue;
                        }
                    }

                    //lets create the equivanet model object
                    OnlineProduct model = helper.SetupOnlineRelatedProduct(groupId, clientId, ctx, product, new OnlineProduct());

                    //packages
                    if (product.PackageContentGroups.Count > 0)
                    {
                        helper.SetupRelatedPackages(groupId, clientId, model, product, ctx);
                    }

                    ret.Add(model);
                }
            }

            return ret;
        }

        #endregion

        #region GetUpgradeProductByProductId
        public List<UpgradeProduct> GetUpgradeProductByProductId(int productId, int clientId)
        {
            List<EntityRelations> loadOptions = new List<EntityRelations>();
            loadOptions.Add(EntityRelations.Product_To_OnlineOrderCategory);
            loadOptions.Add(EntityRelations.Product_To_ProductSizeCode);
            loadOptions.Add(EntityRelations.Product_To_PackageContentGroups);
            loadOptions.Add(EntityRelations.PackageContentGroup_To_PackageContentGroupProducts);
            loadOptions.Add(EntityRelations.Product_To_PriceListDetails);
            loadOptions.Add(EntityRelations.PriceListDetail_To_PriceList);
            loadOptions.Add(EntityRelations.Product_To_ProductType);
            loadOptions.Add(EntityRelations.Product_To_ProductPricings);
            loadOptions.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
            OnlineProductHelper helper = new OnlineProductHelper();
            //List<UpgradeProduct> nonDIYTemplateList = new List<UpgradeProduct>();

            var groupId = 0;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Client cl = (from c in ctx.Clients
                                 where c.ClientID == clientId
                                 select c).First();

                    groupId = cl.GroupId;

                    IQueryable<AOP_TemplateProduct> availableTemplates = from tm in ctx.AOP_TemplateProducts
                                                                         where tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId
                                                                         select tm;

                    IQueryable<AOP_TemplateProduct> corporateTemplates = from tm in ctx.AOP_TemplateProducts
                                                                         where tm.AOP_Template.GroupId == groupId && tm.AOP_Template.ClientId == null
                                                                         select tm;

                    if (availableTemplates != null && availableTemplates.Count() == 0) //if we don't have any client level template then look at group level
                    {
                        availableTemplates = corporateTemplates;
                    }	

                    decimal solarPanelPrice = 150;
                    var pld = (from pl in ctx.PriceListDetails
                              where pl.PriceListID == cl.PriceListID && pl.ProductID == ServiceConfig.SOLAR_PANEL_PRODUCT_ID
                                select pl).FirstOrDefault();

                    if (pld != null && pld.ProductPrice >= 0)
                    {
                        solarPanelPrice = pld.ProductPrice * new Decimal(ServiceConfig.GST);
                    }

                    var result = (from up in ctx.UPGRADE_Products
                                  join p in ctx.Products on up.UpgradeProductID equals p.ProductID
                                  where up.ProductID == productId
                                  select new UpgradeProduct()
                                  {
                                      OnlineCategoryId = p.CategoryId.Value,
                                      ProductId = up.ProductID,
                                      UpgradeProductID = up.UpgradeProductID,
                                      ProductName = string.IsNullOrEmpty(p.WebFriendlyName) ? p.Name : p.WebFriendlyName,
                                      WebFriendlyName = string.IsNullOrEmpty(p.WebFriendlyName) ? p.Name : p.WebFriendlyName,
                                      XmlConfig = string.IsNullOrEmpty(p.ProductFormConfig) ? p.OnlineOrderCategory.XmlFormConfig : p.ProductFormConfig,
                                      ProductConfig = ProductConfig.GetFromString(string.IsNullOrEmpty(p.ProductFormConfig) ? p.OnlineOrderCategory.XmlFormConfig : p.ProductFormConfig),
                                      UpgradePrice = up.PriceExtra,
                                      SolarPanelPrice = solarPanelPrice
                                  }).ToList();


                    foreach (var item in result)
                    {
                        if (item.OnlineCategoryId == CategoryTypes.Billboard)
                        {
                            var dbProduct = (from p in ctx.Products
                                             where p.ProductID == item.UpgradeProductID
                                             select p).FirstOrDefault();

                            if (dbProduct == null)
                                break;

                            if (dbProduct.CategoryId.In(1, 2, 3, 5, 14, 15))
                            {
                                if (!string.IsNullOrEmpty(dbProduct.ContentType))
                                {
                                    item.ContentType = dbProduct.ContentType;
                                }
                                if (!string.IsNullOrEmpty(dbProduct.Format))
                                {
                                    item.Format = dbProduct.Format;
                                }
                                if (!string.IsNullOrEmpty(dbProduct.SizeCode))
                                {
                                    var sz = (from p in ctx.ProductSizeCodeDetails
                                              where p.SizeCode == dbProduct.SizeCode && p.ProductTypeId == dbProduct.TypeID
                                              select p).FirstOrDefault();

                                    if (sz != null)
                                        item.SizeCode = sz.SizeCodeOnWeb;
                                }
                            }
                            if (!string.IsNullOrEmpty(dbProduct.FrameType) && dbProduct.CategoryId.In(1, 2, 14))
                            {
                                item.FrameType = dbProduct.FrameType;
                            }
                            if (dbProduct.Qty > 0 && dbProduct.CategoryId.In(3))
                            {
                                item.Qty = dbProduct.Qty.ToString();
                            }

                            if (item.OnlineCategoryId == CategoryTypes.Billboard && !string.IsNullOrEmpty(item.FrameType) && item.FrameType.ToLower() == "light board")
                            {
                                item.XmlConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>  <ProductConfig>   <fields>    <field>     <fieldName>Qty</fieldName>     <caption>Qty</caption>     <helpText>Enter Product Quantity</helpText>     <getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value>1</value>     <fieldType>TextBox</fieldType>     <validation required=""true"">      <rangeValidation min=""1"" max=""100""/>     </validation>    </field>    <field>     <fieldName>Format</fieldName>     <caption>Format</caption>     <helpText>Select Orientation</helpText>     <getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value></value>     <fieldType>ComboBox</fieldType>     <listItems>      <listItem displayText=""Portrait"" valueText=""Portrait"" />      <listItem displayText=""Landscape"" valueText=""Landscape"" />     </listItems>     <validation required=""true""></validation>    </field>   <field>    <fieldName>SolarPanel</fieldName>     <caption>Solar Panel</caption>     <helpText>Select Solar Panel</helpText>     <getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>     <enabled>true</enabled>     <value></value>     <fieldType>ComboBox</fieldType>     <listItems>      <listItem displayText=""Without Solar Panel"" valueText=""0"" />      <listItem displayText=""With Solar Panel $150 Additional"" valueText=""1"" />      </listItems>     <validation required=""true""></validation>    </field>	</fields>	</ProductConfig>";
                                item.ProductConfig = ProductConfig.GetFromString(item.XmlConfig);
                                //Logger.Warn(item.OnlineCategoryId + " -- " + item.FrameType + " -- " + item.ProductId);
                            }

                            //get template
                            if (string.IsNullOrEmpty(item.SizeCode))
                                break;

                            var temp = availableTemplates;

                            var query = from tm in temp
                                        where tm.Type == dbProduct.ProductType.Type && tm.ContentType == dbProduct.ContentType && tm.SizeCode == item.SizeCode
                                        select tm;

                            if (!string.IsNullOrEmpty(item.FrameType))
                                query = from q in query
                                        where q.FrameType == item.FrameType
                                        select q;

                            if (!string.IsNullOrEmpty(item.Format))
                                query = from q in query
                                        where q.Format == item.Format
                                        select q;

                            if (query.Count() > 0)
                            {
                                foreach (var element in query)
                                {
                                    DIYTemplate tmpl = new DIYTemplate();
                                    tmpl.ProductTemplateId = element.TemplateProductId;
                                    tmpl.TemplateName = element.Name;
                                    tmpl.TemplateFormat = element.Format;
                                    tmpl.TemplateDescription = element.Description;
                                    item.DIYTemplates.Add(tmpl);
                                }
                            }
                            else
                            {
                                var corpTemp = corporateTemplates;

                                var corpQuery = from tm in corpTemp
                                                where tm.Type == dbProduct.ProductType.Type && tm.ContentType == dbProduct.ContentType && tm.SizeCode == item.SizeCode
                                                select tm;

                                if (!string.IsNullOrEmpty(item.FrameType))
                                    query = from q in corpQuery
                                            where q.FrameType == item.FrameType
                                            select q;

                                if (!string.IsNullOrEmpty(item.Format))
                                    query = from q in corpQuery
                                            where q.Format == item.Format
                                            select q;

                                if (corpQuery.Count() > 0)
                                {
                                    foreach (var element in query)
                                    {
                                        DIYTemplate tmpl = new DIYTemplate();
                                        tmpl.ProductTemplateId = element.TemplateProductId;
                                        tmpl.TemplateName = element.Name;
                                        tmpl.TemplateFormat = element.Format;
                                        tmpl.TemplateDescription = element.Description;
                                        item.DIYTemplates.Add(tmpl);
                                    }
                                }
                                
                            }

                            if (cl != null && (cl.ClientID == ClientSettings.BuyMyPlaceSouthMelbourne || cl.ClientID == 8871) && (item.SizeCode == "6 x 4 Board" || item.SizeCode == "4 x 6 Board"))
                            {
                                if (item.UpgradePrice > 100)
                                    item.UpgradePrice -= 100;
                            }
                        }
                    }

                    ////Get upgrade product which has available template only
                    //foreach (var elem in nonDIYTemplateList)
                    //{
                    //        result.Remove(elem);
                    //}
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetUpgradeProductByProductId'");
                throw;
            }
        }

        #endregion

        #region GetCorflutePrice
        public List<CorflutePricing> GetCorflutePrice(int productId, int clientId)
        {
            List<EntityRelations> loadOptions = new List<EntityRelations>();
            List<CorflutePricing> result = new List<CorflutePricing>();

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Client cl = (from c in ctx.Clients
                                 where c.ClientID == clientId
                                 select c).First();

                    if (cl == null)
                    {
                        return result;
                    }

                    PriceListDetail prld = (from p in ctx.PriceListDetails
                                            where p.ProductID == productId
                                            && p.PriceListID == cl.PriceListID
                                            select p).FirstOrDefault();

                    if (prld != null && prld.Active && prld.IsCustom && prld.ProductPrice > 0)
                    {
                        Product pr = (from p in ctx.Products
                                      where p.ProductID == productId
                                      select p).First();
                        if (pr != null)
                        {
                            result = (from cp in ctx.CorflutePrices
                                      where cp.ContentType == pr.ContentType && cp.SizeCode == pr.SizeCode
                                      select new CorflutePricing()
                                      {
                                          MaxQty = cp.MaxQty,
                                          ItemPrice = prld.ProductPrice * new Decimal(ServiceConfig.GST)
                                      }).ToList();

                        }
                    }
                    else
                    {

                        Product pr = (from p in ctx.Products
                                      where p.ProductID == productId
                                      select p).First();
                        if (pr != null)
                        {
                            result = (from cp in ctx.CorflutePrices
                                      where cp.ContentType == pr.ContentType && cp.SizeCode == pr.SizeCode
                                      select new CorflutePricing()
                                      {
                                          MaxQty = cp.MaxQty,
                                          ItemPrice = cp.ItemPrice
                                      }).ToList();

                        }
                    }

                    return result.OrderBy(cp => cp.MaxQty).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetCorflutePrice'");
                throw;
            }
        }

        #endregion
        public string xmlString { get; set; }
    }
}
