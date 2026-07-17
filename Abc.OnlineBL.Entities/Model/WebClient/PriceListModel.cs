using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Abc.OnlineBL.Entities.Model.OnlineOrder;

namespace Abc.OnlineBL.Entities.Model.WebClient
{
	[DataContract]
	public class PriceListModel
	{
		[DataMember]
		public int ProductID { get; set; }

		[DataMember]
		public int ProductTypeID { get; set; }

		[DataMember]
		public string ProductTypeName { get; set; }

		[DataMember]
		public string ProductName { get; set; }

		[DataMember]
		public Decimal ProductPrice { get; set; }

        [DataMember]
        public Decimal ProductPriceIncludingGST { get; set; }

		[DataMember]
		public string CustomDescription { get; set; }

		[DataMember]
		public string SizeCodeOnWeb { get; set; }

		[DataMember]
		public List<PackageGroup> PackageGroups;

		public PriceListModel(Abc.OnlineBL.Entities.AIS_GetAllPriceListProductsResult result)
		{
			this.ProductID = result.ProductID;
			this.ProductTypeID = result.TypeID;
			this.ProductTypeName = result.Type;
			this.ProductName = !string.IsNullOrEmpty(result.WebFriendlyName) ? result.WebFriendlyName : result.Name;
			this.ProductPrice = result.ProductPrice;
            if (result.PriceWithGST.HasValue)
            {
                this.ProductPriceIncludingGST = result.PriceWithGST.Value;
            }
			this.CustomDescription = result.CustomDescription;
			this.SizeCodeOnWeb = !string.IsNullOrEmpty(result.SizeCodeOnWeb) ?  result.SizeCodeOnWeb : "N/A";
		}
	}
}
