using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[DataContract]
	[Serializable]
	public class PackageContentProduct : OnlineProduct
	{		
		private int pkgQty;
		private int uniqueId;

		[DataMember]
		public int UniqueId
		{
			get { return uniqueId; }
			set { uniqueId = value; }
		}

		[DataMember]
		public int PkgQty
		{
			get { return pkgQty; }
			set { pkgQty = value; }
		}

		private string itemNotes;

		[DataMember]
		public string ItemNotes
		{
			get { return itemNotes; }
			set { itemNotes = value; }
		}

		private string pkgFormat;

		[DataMember]
		public string PkgFormat
		{
			get { return pkgFormat; }
			set { pkgFormat = value; }
		}

        [DataMember]
        public int OrderDetailsID { get; set; }

        [DataMember]
        public List<UpgradeProduct> AvailableUpgradeProducts { get; set; }

        [DataMember]
        public decimal SolarPanelPrice { get; set; }

        //[DataMember]
        //public List<BrochureOption> BrochureOptions { get; set; }

		public PackageContentProduct() : base()
		{
		}

		public override string FindFormat(bool isDIYOrder)
		{
			if (!string.IsNullOrEmpty(PkgFormat))
				return PkgFormat;

			return base.FindFormat(isDIYOrder);
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
							this.pkgQty = qty;
						}
					}
				}
			}
		}
	}
}
