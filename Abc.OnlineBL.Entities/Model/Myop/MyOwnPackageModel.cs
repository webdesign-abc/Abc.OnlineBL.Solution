using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.Myop
{
	[DataContract]
	[Serializable]
	public class MyOwnPackageModel
	{
		public MyOwnPackageModel()
		{
			this.packageItems = new List<MyopPackageProductModel>();
		}

		private List<MyopPackageProductModel> packageItems;

		[DataMember]
		public string PackageName { get; set; }
		[DataMember]
		public int ClientId { get; set; }
		[DataMember]
		public int ExistingMyopProductId { get; set; }

		[DataMember]
		public List<MyopPackageProductModel> PackageItems
		{
			get
			{
				return packageItems;
			}
			set
			{
				packageItems = value;
			}
		}
		
	}
}
