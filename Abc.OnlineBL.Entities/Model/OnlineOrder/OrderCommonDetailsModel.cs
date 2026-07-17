using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class OrderCommonDetailsModel
	{

        public OrderCommonDetailsModel()
        {
        }

        [DisplayName("Sale Type")]
		[DataMember]
		public string SaleType { get; set; }

		[DataMember]
		public string AuctionDate { get; set; }

		[DataMember]
		public string AuctionTime { get; set; }

		[DataMember]
		public string StandardAuctionDateTime { get; set; } //This is for Listing, dont take this out

		[DataMember]
		public string AuctionDetails { get; set; }

        [DisplayName("Please insert the ABCrealestate.com.au Link onto this Board")]
		[DataMember]
		public bool IsInsertAbcRealEstateLink { get; set; }

	}
}