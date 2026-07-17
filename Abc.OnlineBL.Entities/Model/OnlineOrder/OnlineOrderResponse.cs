using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class OnlineOrderResponse
	{
		[DataMember]
		public int OrderId { get; set; }
		[DataMember]
		public int PhotoOrderId { get; set; }
		[DataMember]
		public int StockId { get; set; }
		[DataMember]
		public int ListingId { get; set; }
		[DataMember]
		public int PropertyId { get; set; }
		[DataMember]
		public string ErrorMessage { get; set; }
		[DataMember]
		public bool OrderHasError { get; set; }
	}
}
