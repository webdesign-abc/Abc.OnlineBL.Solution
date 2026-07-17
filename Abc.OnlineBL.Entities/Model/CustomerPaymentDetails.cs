using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class CustomerPaymentDetails
	{
		public CustomerPaymentDetails()
		{
			CustomerDetails = new List<CustomerDetails>();
		}

		[DataMember]
		public List<CustomerDetails> CustomerDetails { get; set; }
		[DataMember]
		public Decimal TotalAmount { get; set; }
		[DataMember]
		public Decimal Surcharge { get; set; }
		[DataMember]
		public int PaymentID { get; set; }
	}
}
