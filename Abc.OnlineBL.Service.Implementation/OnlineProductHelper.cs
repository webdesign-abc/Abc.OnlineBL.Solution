using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Service.Implementation.BusinessLogic;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.Service.Implementation
{
	public class OnlineProductHelper
	{
		public OnlineProduct SetupOnlineProduct(int groupId, int clientId, AbcDataContext ctx, Product product, OnlineProduct model, bool returnDIYOnly, int priceListId, bool isPackageProduct)
		{
			//basic mappings
			if (product.CategoryId.HasValue)
			{
				model.OnlineCategoryId = product.CategoryId.Value;
			}
			model.ProductId = product.ProductID;
			model.ProductName = product.Name;
			model.WebFriendlyName = product.WebFriendlyName ?? product.Name;
			model.Description = product.ProductDescription;
			model.OnlineSubCategoryId = product.SubCategoryId;
			model.TypeId = product.TypeID;
			model.CategoryName = product.OnlineOrderCategory.CategoryName;
			model.IsMyop = product.Myop;
			model.UsePackageContentPrice = product.UsePackageContentPrice;

			//product xml has precedence, then category xml config
			model.XmlConfig = product.ProductFormConfig;
			if (string.IsNullOrEmpty(model.XmlConfig)) //else lets try category level config
				model.XmlConfig = product.OnlineOrderCategory.XmlFormConfig;

			//custom name logic:
			//if a clients price list has the product and has a custom name in the price list
			//we have to use that name to display, which will help the client identify the right
			//product, these are client specific products sometime we add. eg. Stockboard for particular agent
			if (product.PriceListDetails != null && product.PriceListDetails.Count > 0)
			{
				var pl = product.PriceListDetails.Where(p => p.PriceListID == priceListId).FirstOrDefault();
				if (pl != null)
				{
					if (pl.PriceList != null && pl.PriceList.IsComplete)
					{
						if (pl.ProductPrice > 0)
						{
							if (product.ApplyGST)
							{
								pl.ProductPrice = pl.ProductPrice * new Decimal(ServiceConfig.GST);
							}
							model.PriceDisplay = "$" + pl.ProductPrice.ToString("F");
							model.ProductPrice = pl.ProductPrice;
						}
						else if (pl.ProductPrice == 0)
						{
							//If custom pricing then display as 0 (product is free)
							if (pl.IsCustom)
							{
								if (product.ApplyGST)
								{
									pl.ProductPrice = pl.ProductPrice * new Decimal(ServiceConfig.GST);
								}
								model.PriceDisplay = "$" + pl.ProductPrice.ToString("F");
								model.ProductPrice = pl.ProductPrice;
							}
						}
					}

					if (!string.IsNullOrEmpty(pl.CustomDescription))
					{
						model.CustomDesc = pl.CustomDescription;
						//replace product name with custom name
						model.ProductName = model.CustomDesc;
						model.WebFriendlyName = model.CustomDesc;
					}
				}
				else
				{
					//Need to display price from ProductPricing where pricingid = pricelist.pricingid and productid = pro.productid
					//Not enable due to incomplete product pricing info

					//var priceList = (from p in ctx.PriceLists
					//                 where p.PriceListID == priceListId
					//                 select p).FirstOrDefault();
					//if (priceList != null)
					//{
					//    var productPricing = product.ProductPricings.Where(p => p.PricingID == priceList.PricingID).FirstOrDefault();
					//    if (productPricing != null)
					//    {
					//        if (product.ApplyGST)
					//        {
					//            productPricing.Price = productPricing.Price * new Decimal(ServiceConfig.GST);
					//        }
					//        model.PriceDisplay = "$" + productPricing.Price.ToString("F");
					//        model.ProductPrice = productPricing.Price;
					//    }
					//}
				}
			}

			//load attributes
			ProductAttributeMapper.MapAttributes(ctx, product, model, product.OnlineOrderCategory.AttributeList);
			
			if (returnDIYOnly && product.OnlineOrderCategory.MayContainDIYProducts)
			{
				MatchDIYTemplates(groupId, clientId, model, product, ctx, isPackageProduct);
			}

			return model;
		}

        public OnlineProduct SettingupOnlineProduct(int groupId, int clientId, AbcDataContext ctx, Product product, OnlineProduct model, bool returnDIYOnly, int priceListId, bool isPackageProduct, List<AOP_TemplateProduct> tempTP, List<AOP_TemplateProduct> corporateTemplates)
		{
			//basic mappings
			if (product.CategoryId.HasValue)
			{
				model.OnlineCategoryId = product.CategoryId.Value;
			}
			model.ProductId = product.ProductID;
			model.ProductName = product.Name;
			model.WebFriendlyName = product.WebFriendlyName ?? product.Name;
			model.Description = product.ProductDescription;
			model.OnlineSubCategoryId = product.SubCategoryId;
			model.TypeId = product.TypeID;
			model.CategoryName = product.OnlineOrderCategory.CategoryName;
			model.IsMyop = product.Myop;
			model.UsePackageContentPrice = product.UsePackageContentPrice;
            model.FrameType = product.FrameType;

			//product xml has precedence, then category xml config
			model.XmlConfig = product.ProductFormConfig;
			if (string.IsNullOrEmpty(model.XmlConfig)) //else lets try category level config
				model.XmlConfig = product.OnlineOrderCategory.XmlFormConfig;

			//custom name logic:
			//if a clients price list has the product and has a custom name in the price list
			//we have to use that name to display, which will help the client identify the right
			//product, these are client specific products sometime we add. eg. Stockboard for particular agent
			if (product.PriceListDetails != null && product.PriceListDetails.Count > 0)
			{
				var pl = product.PriceListDetails.Where(p => p.PriceListID == priceListId).FirstOrDefault();
				if (pl != null)
				{
					if (pl.PriceList != null && pl.PriceList.IsComplete)
					{
						if (pl.ProductPrice > 0)
						{
							if (product.ApplyGST)
							{
								pl.ProductPrice = pl.ProductPrice * new Decimal(ServiceConfig.GST);
							}
							model.PriceDisplay = "$" + pl.ProductPrice.ToString("F");
							model.ProductPrice = pl.ProductPrice;
						}
						else if (pl.ProductPrice == 0)
						{
							//If custom pricing then display as 0 (product is free)
							if (pl.IsCustom)
							{
								if (product.ApplyGST)
								{
									pl.ProductPrice = pl.ProductPrice * new Decimal(ServiceConfig.GST);
								}
								model.PriceDisplay = "$" + pl.ProductPrice.ToString("F");
								model.ProductPrice = pl.ProductPrice;
							}
						}
					}

					if (!string.IsNullOrEmpty(pl.CustomDescription))
					{
						model.CustomDesc = pl.CustomDescription;
						//replace product name with custom name
						model.ProductName = model.CustomDesc;
						model.WebFriendlyName = model.CustomDesc;
					}
				}
				else
				{

				}
			}

			//load attributes
			ProductAttributeMapper.MapAttributes(ctx, product, model, product.OnlineOrderCategory.AttributeList);

			if (returnDIYOnly && product.OnlineOrderCategory.MayContainDIYProducts)
			{
				MatchingDIYTemplates(groupId, clientId, model, product, ctx, isPackageProduct, tempTP);
                if (model.DIYTemplates.Count < 1)
                {
                    MatchingDIYTemplates(groupId, clientId, model, product, ctx, isPackageProduct, corporateTemplates);
                }
			}

			return model;
		}

		public void SetupPackages(int groupId, int clientId, OnlineProduct model, Product product, AbcDataContext ctx, bool returnDIYOnly, int priceListId)
		{
			foreach (var item in product.PackageContentGroups)
			{
				Abc.OnlineBL.Entities.Model.OnlineOrder.PackageGroup grp = new PackageGroup();
				grp.GroupId = item.GroupId;
				grp.GroupName = item.GroupName;
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
						pkgProduct = (PackageContentProduct)SetupOnlineProduct(groupId, clientId, ctx, dbProduct, pkgProduct, returnDIYOnly, priceListId, true);
						grp.Products.Add(pkgProduct);
					}
				}
				model.PackageGroups.Add(grp);
			}
		}

		public void MatchDIYTemplates(int groupId, int clientId, OnlineProduct model, Product product, AbcDataContext ctx, bool isPackageProduct)
		{
			var sz = model.Attributes.Find(q => q.Key == "SizeCode");
			if (sz == null)
				return; //a product must have a proper size code defined			

			var diyCountquery = from tm in ctx.AOP_TemplateProducts
						where (tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId) 
						select tm;

			var query = from tm in ctx.AOP_TemplateProducts
							where (tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId) && tm.Type == product.ProductType.Type && tm.ContentType == product.ContentType && tm.SizeCode == sz.Value
							select tm;

			if (diyCountquery.ToList().Count == 0)
			{
				query = null;
				query = from tm in ctx.AOP_TemplateProducts
						where (tm.AOP_Template.ClientId == null && tm.AOP_Template.GroupId == groupId) && tm.Type == product.ProductType.Type && tm.ContentType == product.ContentType && tm.SizeCode == sz.Value
						select tm;
			}

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
				if (item.Active == false)
					continue;

				DIYTemplate tmpl = new DIYTemplate();
				tmpl.ProductTemplateId = item.TemplateProductId;
				tmpl.TemplateName = item.Name;
				tmpl.TemplateFormat = item.Format;
				tmpl.TemplateDescription = item.Description;
				model.DIYTemplates.Add(tmpl);
			}
		}

		public void MatchingDIYTemplates(int groupId, int clientId, OnlineProduct model, Product product, AbcDataContext ctx, bool isPackageProduct, List<AOP_TemplateProduct> tempTP)
		{
			var sz = model.Attributes.Find(q => q.Key == "SizeCode");
			if (sz == null)
				return; //a product must have a proper size code defined			

			var temp = tempTP.AsQueryable();

			var diyCountquery = from tm in temp
								where (tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId)
								select tm;

			var query = from tm in temp
						where (tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId) && tm.Type == product.ProductType.Type && tm.ContentType == product.ContentType && tm.SizeCode == sz.Value
						select tm;

			if (diyCountquery.ToList().Count == 0)
			{
				query = null;
				query = from tm in ctx.AOP_TemplateProducts
						where (tm.AOP_Template.ClientId == null && tm.AOP_Template.GroupId == groupId) && tm.Type == product.ProductType.Type && tm.ContentType == product.ContentType && tm.SizeCode == sz.Value
						select tm;
			}

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
				model.DIYTemplates.Add(tmpl);
			}
		}

		public bool ClientHasOwnAvailableTemplate(int groupId, int clientId, AbcDataContext ctx)
		{
			var query = from tm in ctx.AOP_TemplateProducts
						where tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId
						select tm;

			return query.ToList().Count > 0;

		}

		public bool ProductNotHasMatchClientDIYTemplates(int groupId, int clientId, Product product, AbcDataContext ctx, OnlineProduct model)
		{
			var sz = model.Attributes.Find(q => q.Key == "SizeCode");
			if (sz == null)
				return true; //a product must have a proper size code defined			

			var query = from tm in ctx.AOP_TemplateProducts
						where (tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId) && tm.Type == product.ProductType.Type && tm.ContentType == product.ContentType && tm.SizeCode == sz.Value
						select tm;

			if (!string.IsNullOrEmpty(product.FrameType))
				query = from q in query
						where q.FrameType == product.FrameType
						select q;

			if (!string.IsNullOrEmpty(product.Format))
				query = from q in query
						where q.Format == product.Format
						select q;

			return query.ToList().Count <= 0;
		}

		public bool ProductDoesNotHasMatchClientDIYTemplates(int groupId, int clientId, Product product, OnlineProduct model, List<AOP_TemplateProduct> tempTP)
		{
			var sz = model.Attributes.Find(q => q.Key == "SizeCode");
			if (sz == null)
				return true; //a product must have a proper size code defined			

			var temp = tempTP.AsQueryable();
			var query = from tm in temp
						where (tm.AOP_Template.ClientId == clientId && tm.AOP_Template.GroupId == groupId) && tm.Type == product.ProductType.Type && tm.ContentType == product.ContentType && tm.SizeCode == sz.Value
						select tm;

			if (!string.IsNullOrEmpty(product.FrameType))
				query = from q in query
						where q.FrameType == product.FrameType
						select q;

			if (!string.IsNullOrEmpty(product.Format))
				query = from q in query
						where q.Format == product.Format
						select q;

			return query.ToList().Count <= 0;
		}

        public OnlineProduct SetupOptionalProduct(Product product, OnlineProduct model)
        {
            //basic mappings
            model.OnlineCategoryId = product.CategoryId.Value;
            model.ProductId = product.ProductID;
            model.WebFriendlyName = product.WebFriendlyName ?? product.Name;
            model.CategoryName = product.OnlineOrderCategory.CategoryName;

            return model;
        }

        public OnlineProduct SetupOnlineFavProduct(int groupId, int clientId, AbcDataContext ctx, Product product, OnlineProduct model, bool returnDIYOnly, int priceListId, bool isPackageProduct)
        {
            //basic mappings
            if (product.CategoryId.HasValue)
            {
                model.OnlineCategoryId = product.CategoryId.Value;
            }
            model.ProductId = product.ProductID;
            model.WebFriendlyName = product.WebFriendlyName ?? product.Name;
            model.Description = product.ProductDescription;
            model.CategoryName = product.OnlineOrderCategory.CategoryName;

            //custom name logic:
            //if a clients price list has the product and has a custom name in the price list
            //we have to use that name to display, which will help the client identify the right
            //product, these are client specific products sometime we add. eg. Stockboard for particular agent
            if (product.PriceListDetails != null && product.PriceListDetails.Count > 0)
            {
                var pl = product.PriceListDetails.Where(p => p.PriceListID == priceListId).FirstOrDefault();
                if (pl != null)
                {
                    if (!string.IsNullOrEmpty(pl.CustomDescription))
                    {
                        model.WebFriendlyName = pl.CustomDescription;
                    }
                }
            }

            //load attributes
            ProductAttributeMapper.MapAttributes(ctx, product, model, product.OnlineOrderCategory.AttributeList);

            if (returnDIYOnly && product.OnlineOrderCategory.MayContainDIYProducts)
            {
                MatchDIYTemplates(groupId, clientId, model, product, ctx, isPackageProduct);
            }

            return model;
        }

        #region Deprecated
        public OnlineProduct SetupOnlineRelatedProduct(int groupId, int clientId, AbcDataContext ctx, Product product, OnlineProduct model)
        {

            //basic mappings
            model.OnlineCategoryId = product.CategoryId.Value;
            model.ProductId = product.ProductID;
            model.ProductName = product.Name;
            model.WebFriendlyName = product.WebFriendlyName ?? product.Name;
            model.Description = product.ProductDescription;
            model.OnlineSubCategoryId = product.SubCategoryId;
            model.TypeId = product.TypeID;
            model.CategoryName = product.OnlineOrderCategory.CategoryName;

            //product xml has precedence, then category xml config
            model.XmlConfig = product.ProductFormConfig;
            if (string.IsNullOrEmpty(model.XmlConfig)) //else lets try category level config
                model.XmlConfig = product.OnlineOrderCategory.XmlFormConfig;

            //custom name logic:
            //if a clients price list has the product and has a custom name in the price list
            //we have to use that name to display, which will help the client identify the right
            //product, these are client specific products sometime we add. eg. Stockboard for particular agent
            var customDescPLD = (from c in ctx.Clients
                                 join pl in ctx.PriceLists on c.PriceListID equals pl.PriceListID
                                 join pld in ctx.PriceListDetails on pl.PriceListID equals pld.PriceListID
                                 where pld.ProductID == product.ProductID && c.ClientID == clientId
                                 select pld).FirstOrDefault();

            if (customDescPLD != null)
            {
                if (customDescPLD.PriceList != null && customDescPLD.PriceList.IsComplete)
                {

                    if (customDescPLD.ProductPrice > 0)
                    {
                        model.PriceDisplay = "$" + customDescPLD.ProductPrice.ToString("F");
                        model.ProductPrice = customDescPLD.ProductPrice;
                    }
                    else if (customDescPLD.ProductPrice == 0)
                    {
                        if (customDescPLD.IsCustom)
                        {
                            model.PriceDisplay = "$" + customDescPLD.ProductPrice.ToString("F");
                            model.ProductPrice = customDescPLD.ProductPrice;
                        }
                        else
                        {
                            model.PriceDisplay = "TBA";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(customDescPLD.CustomDescription))
                {
                    model.CustomDesc = customDescPLD.CustomDescription;
                    //replace product name with custom name
                    model.ProductName = model.CustomDesc;
                    model.WebFriendlyName = model.CustomDesc;
                }
            }

            //load attributes
            ProductAttributeMapper.MapAttributes(ctx, product, model, product.OnlineOrderCategory.AttributeList);

            if (product.OnlineOrderCategory.MayContainDIYProducts)
            {
                MatchDIYTemplates(groupId, clientId, model, product, ctx, false);
            }

            return model;
        }

        public void SetupRelatedPackages(int groupId, int clientId, OnlineProduct model, Product product, AbcDataContext ctx)
        {
            foreach (var item in product.PackageContentGroups)
            {
                Abc.OnlineBL.Entities.Model.OnlineOrder.PackageGroup grp = new PackageGroup();
                grp.GroupId = item.GroupId;
                grp.GroupName = item.GroupName;
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
                        pkgProduct = (PackageContentProduct)SetupOnlineRelatedProduct(groupId, clientId, ctx, dbProduct, pkgProduct);
                        grp.Products.Add(pkgProduct);
                    }
                }
                model.PackageGroups.Add(grp);
            }
        }
        #endregion

	}
}
