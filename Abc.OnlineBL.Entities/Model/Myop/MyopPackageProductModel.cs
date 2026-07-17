using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.Myop
{
	[DataContract]
	[Serializable]
	public class MyopPackageProductModel
	{
		[DataMember]
		public int ProductId { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int CategoryId { get; set; }
		[DataMember]
		public string CategoryName { get; set; }
	}
}
