using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class OnlineProduct
	{
		protected string guid;		
		protected int productId;
		protected string productName;
		protected int typeId;
		protected int onlineCategoryId;
		protected int? onlineSubCategoryId;
		protected string webFriendlyName;
		protected string description;
		protected List<AbcKeyValuePair> attributes;
		protected ProductConfig productConfig;
		private List<DIYTemplate> dIYTemplates;
		private string customDesc;
		private List<PackageGroup> packageGroups;
		private string xmlConfig;
		private int selectedDIYTemplateId;
		private int previousSelectedDIYTemplateId;
		private string categoryName = string.Empty;
		private string priceDisplay = "TBA";
		private decimal productPrice;
		private bool isMyop;
		private bool usePackageContentPrice;
        private int? favProductId;
        private string contentType;
        protected List<CorflutePricing> corflutePrices;


        [DataMember]
        public int? FavProductId
        {
            get { return favProductId; }
            set { favProductId = value; }
        }

		[DataMember]
		public string ProductGuid
		{
			get { return guid; }
			set { guid = value; }
		}

		[DataMember]
		public string CategoryName
		{
			get { return categoryName; }
			set { categoryName = value; }
		}

		[DataMember]
		public int SelectedDIYTemplateId
		{
			get { return selectedDIYTemplateId; }
			set { selectedDIYTemplateId = value; }
		}

		[DataMember]
		public int PreviousSelectedDIYTemplateId
		{
			get { return previousSelectedDIYTemplateId; }
			set { previousSelectedDIYTemplateId = value; }
		}

		[DataMember]		
		public string XmlConfig
		{
			get { return xmlConfig; }
			set { xmlConfig = value; }
		}

		public OnlineProduct()
		{
            this.attributes = new List<AbcKeyValuePair>();
            this.corflutePrices = new List<CorflutePricing>();
			this.dIYTemplates = new List<DIYTemplate>();
			this.packageGroups = new List<PackageGroup>();
			this.guid = Guid.NewGuid().ToString("N");
		}

		[DataMember]
		public List<PackageGroup> PackageGroups
		{
			get { return packageGroups; }
			set { packageGroups = value; }
		}		

		[DataMember]
		public List<DIYTemplate> DIYTemplates
		{
			get { return dIYTemplates; }
			set { dIYTemplates = value; }
		}

		[DataMember]
		public List<AbcKeyValuePair> Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		[DataMember]
		public ProductConfig ProductConfig
		{
			get { return productConfig; }
			set { productConfig = value; }
		}

		[DataMember]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		[DataMember]
		public string WebFriendlyName
		{
			get { return webFriendlyName; }
			set { webFriendlyName = value; }
		}

		[DataMember]
		[XmlElement(IsNullable = true)]
		public int? OnlineSubCategoryId
		{
			get { return onlineSubCategoryId; }
			set { onlineSubCategoryId = value; }
		}

		[DataMember]
		public int OnlineCategoryId
		{
			get { return onlineCategoryId; }
			set { onlineCategoryId = value; }
		}

		[DataMember]
		public int TypeId
		{
			get { return typeId; }
			set { typeId = value; }
		}

		[DataMember]
		public string ProductName
		{
			get { return productName; }
			set { productName = value; }
		}

		[DataMember]
		public int ProductId
		{
			get { return productId; }
			set { productId = value; }
		}

		[DataMember]
		public string CustomDesc
		{
			get { return customDesc; }
			set { customDesc = value; }
		}

		[DataMember]
		public bool ProductGroupsHaveAvailableTemplate { get; set; }

		[DataMember]
		public string PriceDisplay
		{
			get { return priceDisplay; }
			set { priceDisplay = value; }
		}

		[DataMember]
		public decimal ProductPrice
		{
			get { return productPrice; }
			set { productPrice = value; }
		}

		[DataMember]
		public bool IsMyop
		{
			get { return isMyop; }
			set { isMyop = value; }
		}

		[DataMember]
		public bool UsePackageContentPrice
		{
			get { return usePackageContentPrice; }
			set { usePackageContentPrice = value; }
		}

        [DataMember]
        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        [DataMember]
        public string SizeCode { get; set; }

        [DataMember]
        public string FrameType { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public string Qty { get; set; }

        [DataMember]
        public string CustomName { get; set; }

        [DataMember]
        public string Tags { get; set; }

        [DataMember]
        public List<CorflutePricing> CorflutePrices
        {
            get { return corflutePrices; }
            set { corflutePrices = value; }
        }

        [DataMember]
        public decimal TotalPrice { get; set; }

        [DataMember]
        public bool IsSBRelatedItem { get; set; }

        public void CopyTo(OnlineProduct dest)
		{
			dest.productId = this.productId;
			dest.productName = this.productName;
			dest.typeId = this.typeId;
			dest.onlineCategoryId = this.onlineCategoryId;
			dest.onlineSubCategoryId = this.onlineSubCategoryId;
			dest.webFriendlyName = this.webFriendlyName;
			dest.description = this.description;
			dest.attributes = this.attributes;			
			dest.productConfig = this.productConfig;
			dest.customDesc = this.customDesc;
			dest.categoryName = this.categoryName;
			dest.packageGroups = this.packageGroups;
			dest.xmlConfig = this.xmlConfig;
			dest.dIYTemplates = this.dIYTemplates;
			dest.PriceDisplay = this.PriceDisplay;
			dest.ProductPrice = this.ProductPrice;
			dest.IsMyop = this.IsMyop;
			dest.UsePackageContentPrice = this.UsePackageContentPrice;
            dest.ContentType = this.ContentType;
            dest.FavProductId = this.FavProductId;
            dest.FrameType = this.FrameType;
            dest.SizeCode = this.SizeCode;
            dest.Format = this.Format;
            dest.CustomName = this.CustomName;
            dest.Qty = this.Qty;
            dest.Tags = this.Tags;
            dest.ProductGroupsHaveAvailableTemplate = this.ProductGroupsHaveAvailableTemplate;
            dest.TotalPrice = this.TotalPrice;
            dest.corflutePrices = this.corflutePrices;
        }

		public bool ProductHasDIY()
		{
			if (this.dIYTemplates.Count > 0)
			{
				if (this.selectedDIYTemplateId>0)
					return true;
			}			
			return false;
		}

        public virtual bool MissingDIYTemplate()
        {
            if (this.PackageGroups.Count > 0)
            {
                //dont need to check for AvailableUpgradeProduct as we only display upgrade option for package that has DIY templates on all group
                return !this.ProductGroupsHaveAvailableTemplate;
            }
            else if (this.CategoryName == "Billboards" || this.CategoryName.IndexOf("Brochures") > -1 || this.CategoryName.IndexOf("Window") > -1 || this.CategoryName.IndexOf("Corflute") > -1)
            {
                return (this.DIYTemplates.Count == 0);
            }
            return false;
        }

        public virtual bool ItemMissingDIYTemplate()
        {
            if (this.PackageGroups.Count > 0)
            {
                foreach (PackageGroup itemGroup in this.packageGroups)
                {
                    if (itemGroup.IsUpgradeProductApplicable)
                    {
                        if (itemGroup.UpgradedProduct.SelectedDIYTemplateId <= 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                        {
                            if (contentProductItem.CategoryName == "Billboards" || contentProductItem.CategoryName.IndexOf("Brochures") > -1 || contentProductItem.CategoryName.IndexOf("Window") > -1 || contentProductItem.CategoryName.IndexOf("Corflute") > -1 || contentProductItem.CategoryName == "DIY Stickers")
                            {
                                if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                                {
                                    //Logger.Warn(contentProductItem.productName + ": " + contentProductItem.SelectedDIYTemplateId);
                                    if (contentProductItem.SelectedDIYTemplateId <= 0)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else if (this.CategoryName == "Billboards" || this.CategoryName.IndexOf("Brochures") > -1 || this.CategoryName.IndexOf("Window") > -1 || this.CategoryName.IndexOf("Corflute") > -1 || this.CategoryName == "DIY Stickers")
            {
                return (this.DIYTemplates.Count == 0);
            }
            return false;
        }

        public virtual bool ProductMissingDIYTemplate()
        {
            if (this.PackageGroups.Count > 0)
            {
                foreach (PackageGroup itemGroup in this.packageGroups)
                {
                    if (itemGroup.IsUpgradeProductApplicable)
                    {
                        if (itemGroup.UpgradedProduct.SelectedDIYTemplateId <= 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                        {
                            if (contentProductItem.CategoryName == "Billboards" || contentProductItem.CategoryName == "Stockboards" || contentProductItem.CategoryName == "Stockboard Overlays" || contentProductItem.CategoryName.IndexOf("Brochures") > -1 || contentProductItem.CategoryName.IndexOf("Window") > -1 || contentProductItem.CategoryName.IndexOf("Corflute") > -1 || contentProductItem.CategoryName == "DIY Stickers")
                            {
                                if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                                {
                                    //Logger.Warn(contentProductItem.productName + ": " + contentProductItem.SelectedDIYTemplateId);
                                    if (contentProductItem.SelectedDIYTemplateId <= 0)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else if (this.CategoryName.IndexOf("Billboard") > -1 || this.CategoryName.IndexOf("Stockboards") > -1 || this.CategoryName == "Stockboard Overlays" || this.CategoryName.IndexOf("Brochures") > -1 || this.CategoryName.IndexOf("Window") > -1 || this.CategoryName.IndexOf("Corflute") > -1 || this.CategoryName == "DIY Stickers")
            {
                return (this.DIYTemplates.Count == 0);
            }
            return false;
        }

		public virtual bool ProductHasAvailableDIYTemplates()
		{
			if (!ProductGroupsHaveAvailableTemplate)
			{
				ProductGroupsHaveAvailableTemplate = ProductHasDIYTemplateOnAllGroup();
			}

            if (this.dIYTemplates.Count > 0 || (this.TypeId == ProductTypes.BillBoard && this.Attributes.Any(a => a.Key == "ContentType" && a.Value.Contains("Community Board"))))
			{				
				return true;
			}
			else if (this.packageGroups.Count > 0)
			{
				var count = from pp in this.PackageGroups
							from pg in pp.Products
							where pg.DIYTemplates.Count > 0
							select pg;
				if (count != null)
				{
					if (count.Count() > 0)
					{
						return true;
					}
				}
			}
			return false;
		}


        public bool ProductHasDIYTemplateOnAllGroup()
        {
            if (this.packageGroups.Count > 0)
            {
                foreach (PackageGroup itemGroup in this.packageGroups)
                {
                    bool ret = false;
                    foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                    {
                        if (contentProductItem.TypeId == ProductTypes.BillBoard || contentProductItem.TypeId == ProductTypes.Brochure || contentProductItem.TypeId == ProductTypes.WindowCard || contentProductItem.TypeId == ProductTypes.DIYStickers)
                        {
                            if (contentProductItem.DIYTemplates.Count > 0)
                            {
                                ret = true;
                                break;
                            }
                        }
                        else
                        {
                            ret = true;
                            break;
                        }
                    }
                    if (ret == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

		public virtual string FindFormat(bool isDIYOrder)
		{
            string format = string.Empty;
            if (!isDIYOrder)
            {
                if (ProductConfig != null && ProductConfig.Fields != null && ProductConfig.Fields.Field != null && ProductConfig.Fields.Field.Count > 0)
                {
                    Field pair = (from f in ProductConfig.Fields.Field where f.FieldName == "Format" select f).FirstOrDefault();
                    if (pair != null)
                        format = pair.Value;
                }
                else if (Attributes != null && Attributes.Count > 0)
                {
                    AbcKeyValuePair pair = (from f in Attributes where f.Key == "Format" select f).FirstOrDefault();

                    if (pair != null)
                        format = pair.Value;
                }

                if (!string.IsNullOrEmpty(format))
                    return format;
                else if (OnlineCategoryId == 2)
                {
                    return Format;
                }
                else
                {

                    var count = (from t in this.DIYTemplates
                                 where t.ProductTemplateId == this.SelectedDIYTemplateId
                                 select t).FirstOrDefault();

                    if (count != null)
                    {
                        return count.TemplateFormat;
                    }
                }
                return string.Empty;
            }
			else
			{
				var count = (from t in this.DIYTemplates
							where t.ProductTemplateId == this.SelectedDIYTemplateId
							select t).FirstOrDefault();

				if (count != null)
				{
					return count.TemplateFormat;
				}
				return string.Empty;
			}
		}

		public string GetValueByFieldName(string fieldName)
		{
			if (ProductConfig != null && ProductConfig.Fields != null && ProductConfig.Fields.Field != null && ProductConfig.Fields.Field.Count > 0)
			{
				Field pair = (from f in ProductConfig.Fields.Field where f.FieldName == fieldName select f).FirstOrDefault();
				if (pair != null)
					return pair.Value;
			}
			return string.Empty;
		}
	}
}
