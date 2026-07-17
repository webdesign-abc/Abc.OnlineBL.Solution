using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class PackageProduct
	{
		[DataMember]
		public int ProductId { get; set; }
		[DataMember]
		public string ProductName { get; set; }
		[DataMember]
		public int GroupId { get; set; }
		[DataMember]
		public string GroupName { get; set; }
		[DataMember]
		public int Qty { get; set; }
	}
}
