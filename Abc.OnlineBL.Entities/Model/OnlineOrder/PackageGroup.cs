using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[DataContract]
	[Serializable]
	public class PackageGroup
	{
		public PackageGroup()
		{
			this.products = new List<PackageContentProduct>();
            this.UpgradedProduct = new UpgradeProduct();
		}

		private int groupId;

		[DataMember]
		public int GroupId
		{
			get { return groupId; }
			set { groupId = value; }
		}

		private string groupName;

		[DataMember]
		public string GroupName
		{
			get { return groupName; }
			set { groupName = value; }
		}

		private List<PackageContentProduct> products;

		[DataMember]
		public List<PackageContentProduct> Products
		{
			get { return products; }
			set { products = value; }
		}

		private int selectedUniqueId;

		[DataMember]
		public int SelectedUniqueId
		{
			get { return selectedUniqueId; }
			set { selectedUniqueId = value; }
		}

        [DataMember]
        public bool IsUpgradeProductApplicable { get; set; }

        [DataMember]
        public int SelectedUpgradeProductId { get; set; }

        [DataMember]
        public UpgradeProduct UpgradedProduct { get; set; }
    }

    [DataContract]
    [Serializable]
    public class UpgradeProduct : PackageContentProduct
    {
        [DataMember]
        public int UpgradeProductID { get; set; }

        [DataMember]
        public decimal UpgradePrice { get; set; }
    }
}
