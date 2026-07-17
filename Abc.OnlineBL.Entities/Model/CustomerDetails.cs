using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class CustomerDetails
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Office { get; set; }
		[DataMember]
		public string Address { get; set; }
		[DataMember]
		public int OrderID { get; set; }
		[DataMember]
		public Decimal AmountPaid { get; set; }
	}
}
