using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class CartItem : OnlineProduct
	{
		private int id;
		private int itemQty;
        private int orderDetailsID;
        private int jobDocumentID;
        private bool isOldProduct;
        private string photos;
        private string saleType;

		public CartItem()
		{

		}

		public CartItem(int id, OnlineProduct product, int itemQty)
		{
			this.id = id;
			this.itemQty = itemQty;
			product.CopyTo(this);
		}

		[DataMember]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}		

		[DataMember]
		public int ItemQty
		{
			get { return itemQty; }
			set { itemQty = value; }
		}

		[DataMember]
		public bool IsOldProduct
		{
			get { return isOldProduct; }
			set { isOldProduct = value; }
		}

		[DataMember]
		public int OrderDetailsID
		{
			get { return orderDetailsID; }
			set { orderDetailsID = value; }
		}
        
        [DataMember]
        public string Photos
        {
            get { return photos; }
            set { photos = value; }
        }

        [DataMember]
        public string SaleType
        {
            get { return saleType; }
            set { saleType = value; }
        }

        [DataMember]
        public int JobDocumentID
        {
            get { return jobDocumentID; }
            set { jobDocumentID = value; }
        }
        

		public string GetText()
		{
			StringBuilder sb = new StringBuilder();

			if (this.ProductId > 0)
			{
				sb.Append("Product Id: " + this.ProductId + Environment.NewLine);
			}

			if (!string.IsNullOrEmpty(this.ProductName))
			{
				sb.Append("Product Name: " + this.ProductName + Environment.NewLine);
			}

			if (this.productConfig != null)
			{
				foreach (var field in this.productConfig.Fields.Field)
				{
					if (!string.IsNullOrEmpty(field.Value))
						sb.Append(field.Caption + " - " + field.Value.Replace("\n", "\r\n          ") + "\r\n");
				}

                if (this.TypeId == ProductTypes.Brochure)
                {
                    sb.Append("Qty - " + this.ItemQty.ToString() + Environment.NewLine);
                }
			}
			else
			{
				if (this.ItemQty > 0)
				{
					sb.Append("Product Quantity: " + this.ItemQty + Environment.NewLine);
				}
			}

            if (this.SelectedDIYTemplateId > 0)
            {
                DIYTemplate tp = this.DIYTemplates.Find(t => t.ProductTemplateId == this.SelectedDIYTemplateId);
                if(tp != null)
                    sb.Append("Client choose this template: " + tp.TemplateName + " -- " + tp.TemplateFormat + " -- " + tp.TemplateDescription + Environment.NewLine);
            }
            else
            {
                if (this.IsSBRelatedItem)
                {
                    sb.Append("Related item." + Environment.NewLine);
                }
            }

			return sb.ToString();
		}

		public string GetPackageText()
		{
			StringBuilder sb = new StringBuilder();

			if (this.ProductId > 0)
			{
				sb.Append("Product Package Id: " + this.ProductId + Environment.NewLine);
			}

			if (!string.IsNullOrEmpty(this.ProductName))
			{
				sb.Append("Product Name: " + this.ProductName + Environment.NewLine);
			}

			if (this.productConfig != null)
			{
				foreach (var field in this.productConfig.Fields.Field)
				{
					sb.Append(field.Caption + " - " + field.Value + "\r\n");
				}
			}

            sb.Append("-------------------Package Content-------------------------\r\n");
            foreach (PackageGroup itemGroup in this.PackageGroups)
            {
                if (itemGroup.IsUpgradeProductApplicable)
                {
                    sb.Append("***** UPGRADE Product Id: " + itemGroup.UpgradedProduct.UpgradeProductID + Environment.NewLine);
                    sb.Append("***** UPGRADE Product Name: " + itemGroup.UpgradedProduct.ProductName + Environment.NewLine);

                    if (itemGroup.UpgradedProduct.ProductConfig != null)
                    {
                        foreach (var field in itemGroup.UpgradedProduct.ProductConfig.Fields.Field)
                        {
                            sb.Append("***** " + field.Caption + " - " + field.Value + "\r\n");
                        }
                    }

                    if (itemGroup.UpgradedProduct.SelectedDIYTemplateId > 0)
                    {
                        DIYTemplate tp = itemGroup.UpgradedProduct.DIYTemplates.Find(t => t.ProductTemplateId == itemGroup.UpgradedProduct.SelectedDIYTemplateId);
                        if (tp != null)
                            sb.Append("Client choose this template: " + tp.TemplateName + " -- " + tp.TemplateFormat + " -- " + tp.TemplateDescription + Environment.NewLine);
                    }
                }
                else
                {
                    foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                    {
                        if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                        {
                            if (contentProductItem.ProductId > 0)
                            {
                                sb.Append("***** Product Id: " + contentProductItem.ProductId + Environment.NewLine);
                            }

                            if (!string.IsNullOrEmpty(contentProductItem.ProductName))
                            {
                                sb.Append("***** Product Name: " + contentProductItem.ProductName + Environment.NewLine);
                            }

                            if (!string.IsNullOrEmpty(contentProductItem.ItemNotes))
                            {
                                sb.Append("***** Item Note: " + contentProductItem.ItemNotes + Environment.NewLine);
                            }

                            if (contentProductItem.ProductConfig != null)
                            {
                                foreach (var field in contentProductItem.ProductConfig.Fields.Field)
                                {
                                    sb.Append("***** " + field.Caption + " - " + field.Value + "\r\n");
                                }
                            }

                            if (contentProductItem.SelectedDIYTemplateId > 0)
                            {
                                DIYTemplate tp = contentProductItem.DIYTemplates.Find(t => t.ProductTemplateId == contentProductItem.SelectedDIYTemplateId);
                                if (tp != null)
                                    sb.Append("Client choose this template: " + tp.TemplateName + " -- " + tp.TemplateFormat + " -- " + tp.TemplateDescription + Environment.NewLine);
                            }
                        }
                    }
                }
            }

			sb.Append("------------------------------------------------------");

			return sb.ToString();
		}

		public string GetHTMLString()
		{
			StringBuilder sb = new StringBuilder();

			if (this.ProductId > 0)
			{
				sb.Append("Product Id: " + this.ProductId + "<BR>");
			}

			if (!string.IsNullOrEmpty(WebFriendlyName))
			{
				sb.Append("Web Product Name: " + WebFriendlyName + "<BR>");
			}
			else if (!string.IsNullOrEmpty(this.ProductName))
			{
				sb.Append("Product Name: " + this.ProductName + "<BR>");
			}

			//sb.Append("Product Price: " + this.PriceDisplay + "<BR>");

			//List of Attributes
			foreach (AbcKeyValuePair item in this.Attributes)
			{
                if (item.Key != "Qty")
                {
                    sb.Append(item.Key + ": " + item.Value + "<BR>");
                }
			}

			if (this.productConfig != null)
			{
				foreach (var field in this.productConfig.Fields.Field)
				{
					if(!string.IsNullOrEmpty(field.Value))
						sb.Append(field.Caption + ": " + field.Value.Replace("\n", " ") + "<BR>");
				}
			}

            if (this.TypeId == ProductTypes.Brochure)
            {
                sb.Append("Qty: " + this.ItemQty.ToString() + "<BR>");
            }

            if (this.TypeId == ProductTypes.Packages || this.TypeId == ProductTypes.BoardPackages)
            {
                if (this.PackageGroups.Count > 0)
                {
                    foreach (var pkg in this.PackageGroups)
                    {
                        if (pkg.SelectedUpgradeProductId > 0 && pkg.UpgradedProduct != null)
                        {
                            if (!string.IsNullOrEmpty(pkg.UpgradedProduct.FrameType) && pkg.UpgradedProduct.FrameType.ToLower() == "light board" && pkg.UpgradedProduct.ProductConfig != null
                                && pkg.UpgradedProduct.ProductConfig.Fields != null && pkg.UpgradedProduct.ProductConfig.Fields.Field != null && pkg.UpgradedProduct.ProductConfig.Fields.Field.Count > 0)
                            // && pkg.UpgradedProduct.ProductConfig.Fields.Field[2] != null)
                            {
                                int solarPanel = 0;
                                var solarPanelConfigField = pkg.UpgradedProduct.ProductConfig.Fields.Field.Where(f => f.FieldName == "SolarPanel").FirstOrDefault();
                                if (solarPanelConfigField != null && !string.IsNullOrEmpty(solarPanelConfigField.Value) && int.TryParse(solarPanelConfigField.Value, out solarPanel))
                                {
                                    if (solarPanel == 1)
                                    {
                                        sb.AppendFormat(" ** Upgraded to {0} for extra ${1} and Solar Panel for ${2}<br/>", pkg.UpgradedProduct.ProductName, pkg.UpgradedProduct.UpgradePrice, pkg.UpgradedProduct.SolarPanelPrice);
                                    }
                                    else
                                    {
                                        sb.AppendFormat(" ** Upgraded to {0} for extra ${1} <br/>", pkg.UpgradedProduct.ProductName, pkg.UpgradedProduct.UpgradePrice);
                                    }
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" ** Upgraded to {0} for extra ${1} <br/>", pkg.UpgradedProduct.ProductName, pkg.UpgradedProduct.UpgradePrice);
                            }
                        }
                        //Logic
                        else
                        {
                            foreach (var pr in pkg.Products)
                            {
                                if (pkg.SelectedUniqueId == pr.UniqueId)
                                {
                                    if (!string.IsNullOrEmpty(pr.FrameType) && pr.FrameType.ToLower() == "light board" && pr.ProductConfig != null && pr.ProductConfig.Fields != null
                                             && pr.ProductConfig.Fields.Field != null && pr.ProductConfig.Fields.Field.Count > 0)
                                    //&& pr.ProductConfig.Fields.Field.Count > 2)
                                    {
                                        int solarPanel = 0;
                                        var solarPanelConfigField = pr.ProductConfig.Fields.Field.Where(f => f.FieldName == "SolarPanel").FirstOrDefault();
                                        if (solarPanelConfigField != null && !string.IsNullOrEmpty(solarPanelConfigField.Value) && int.TryParse(solarPanelConfigField.Value, out solarPanel))
                                        {
                                            if (solarPanel == 1)
                                            {
                                                sb.AppendFormat(" ** Selected Solar Panel for ${0}<br/>", pr.SolarPanelPrice);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
			return sb.ToString();
		}

        public string GetPackageHTMLString()
        {
            StringBuilder sb = new StringBuilder();

            if (this.ProductId > 0)
            {
                sb.Append("Product Package Id: " + this.ProductId + "<BR>");
            }

            if (!string.IsNullOrEmpty(this.ProductName))
            {
                sb.Append("Product Name: " + this.ProductName + "<BR>");
            }

            if (this.productConfig != null)
            {
                foreach (var field in this.productConfig.Fields.Field)
                {
                    sb.Append(field.Caption + " - " + field.Value + "<BR>");
                }
            }

            sb.Append("-------------------Package Content-------------------------<BR>");
            foreach (PackageGroup itemGroup in this.PackageGroups)
            {
                if (itemGroup.IsUpgradeProductApplicable)
                {
                    sb.Append("***** UPGRADE Product Id: " + itemGroup.UpgradedProduct.UpgradeProductID + "<BR>");
                    sb.Append("***** UPGRADE Product Name: " + itemGroup.UpgradedProduct.ProductName + "<BR>");

                    if (itemGroup.UpgradedProduct.ProductConfig != null)
                    {
                        foreach (var field in itemGroup.UpgradedProduct.ProductConfig.Fields.Field)
                        {
                            sb.Append("***** " + field.Caption + " - " + field.Value + "<BR>");
                        }
                    }

                    if (itemGroup.UpgradedProduct.SelectedDIYTemplateId > 0)
                    {
                        DIYTemplate tp = itemGroup.UpgradedProduct.DIYTemplates.Find(t => t.ProductTemplateId == itemGroup.UpgradedProduct.SelectedDIYTemplateId);
                        if (tp != null)
                            sb.Append("Client choose this template: " + tp.TemplateName + " -- " + tp.TemplateFormat + " -- " + tp.TemplateDescription + "<BR>");
                    }
                }
                else
                {
                    foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                    {
                        if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                        {
                            if (contentProductItem.ProductId > 0)
                            {
                                sb.Append("***** Product Id: " + contentProductItem.ProductId + "<BR>");
                            }

                            if (!string.IsNullOrEmpty(contentProductItem.ProductName))
                            {
                                sb.Append("***** Product Name: " + contentProductItem.ProductName + "<BR>");
                            }

                            if (!string.IsNullOrEmpty(contentProductItem.ItemNotes))
                            {
                                sb.Append("***** Item Note: " + contentProductItem.ItemNotes + "<BR>");
                            }

                            if (contentProductItem.ProductConfig != null)
                            {
                                foreach (var field in contentProductItem.ProductConfig.Fields.Field)
                                {
                                    sb.Append("***** " + field.Caption + " - " + field.Value + "<BR>");
                                }
                            }

                            if (contentProductItem.SelectedDIYTemplateId > 0)
                            {
                                DIYTemplate tp = contentProductItem.DIYTemplates.Find(t => t.ProductTemplateId == contentProductItem.SelectedDIYTemplateId);
                                if (tp != null)
                                    sb.Append("Client choose this template: " + tp.TemplateName + " -- " + tp.TemplateFormat + " -- " + tp.TemplateDescription + "<BR>");
                            }
                        }
                    }
                }
            }

            sb.Append("------------------------------------------------------");

            return sb.ToString();
        }

		public string GetXml()
		{
			StringBuilder sb = new StringBuilder();

			if (this.ProductId > 0)
			{
				sb.Append("<ProductId>" + this.ProductId + "</ProductId>");
			}
			if (!string.IsNullOrEmpty(this.ProductName))
			{
				sb.Append("<ProductName>" + HttpUtility.HtmlEncode(this.ProductName != null ? this.ProductName : "") + "</ProductName>");
			}

			if (!string.IsNullOrEmpty(WebFriendlyName))
				sb.AppendFormat("<CustomName>{0}</CustomName>\r\n", HttpUtility.HtmlEncode(WebFriendlyName != null ? WebFriendlyName : ""));

			if (this.TypeId == ProductTypes.Packages || this.TypeId == ProductTypes.BoardPackages || this.TypeId == ProductTypes.OtherPackages)
				sb.Append("<Qty>" + ItemQty + "</Qty>\r\n");

			//List of Attributes
			foreach (AbcKeyValuePair item in this.Attributes)
			{
				sb.Append("<" + item.Key + ">" + HttpUtility.HtmlEncode(item.Value != null ? item.Value : "") + "</" + item.Key + ">\r\n");
			}

			if (this.productConfig != null)
			{
				foreach (var field in this.productConfig.Fields.Field)
				{
					sb.Append("<" + field.FieldName + ">" + HttpUtility.HtmlEncode(field.Value != null ? field.Value : "") + "</" + field.FieldName + ">\r\n");
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Syncs the item qty from product config fields whos value might be Qty and GetDefaultFromItemOrPkgQty is true.
		/// </summary>
		public void SyncItemQtyFromProductConfig()
		{
			if (this.ProductConfig != null)
			{
				var field = (from ff in this.ProductConfig.Fields.Field
							 where ff.GetDefaultFromItemOrPkgQty == true && (ff.FieldName.ToLower() == "qty" || ff.FieldName.ToLower() == "item qty")
							select ff).FirstOrDefault();

				if (field != null)
				{					
					int qty = 0;
					if (int.TryParse(field.Value, out qty))
					{
						if (qty>0)
						{
							this.itemQty = qty;
						}
					}
				}
			}
		}

		public void SyncItemQty()
		{
			if (this.ProductConfig != null)
			{
				var field = (from ff in this.ProductConfig.Fields.Field
							 where (ff.FieldName.ToLower() == "qty" || ff.FieldName.ToLower() == "item qty")
							 select ff).FirstOrDefault();

				if (field != null)
				{
					int qty = 0;
					if (int.TryParse(field.Value, out qty))
					{
						if (qty > 1)
						{
							this.itemQty = qty;
						}
					}
				}
			}
		}
        
	}
}
