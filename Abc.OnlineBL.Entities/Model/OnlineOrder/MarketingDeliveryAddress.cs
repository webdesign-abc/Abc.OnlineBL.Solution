using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class MarketingDeliveryAddress
	{
		[DataMember]
		public string StreetNo { get; set; }
		[DataMember]
		public string StreetName { get; set; }
		[DataMember]
		public string Suburb { get; set; }
		[DataMember]
		public string PostCode { get; set; }
		[DataMember]
		public string State { get; set; }
	}
}
