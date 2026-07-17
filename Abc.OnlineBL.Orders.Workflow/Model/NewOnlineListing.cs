using System;
using System.Collections.Generic;

namespace Abc.OnlineBL.Orders.Workflow.Model
{
	public class NewOnlineListing
	{
		public int ListingType { get; set; }
		public int ClientId { get; set; }
		public int Ptype { get; set; }
		public int LocationId { get; set; }
		public string PAddress { get; set; }
		public string StreetName { get; set; }
		public string StreetNo { get; set; }
		public string UnitNo { get; set; }
		public string Heading { get; set; }
		public string SubHeading { get; set; }
		public string BodyText { get; set; }
		public string InspectionDetails { get; set; }
		public DateTime? AuctionDate { get; set; }
		public string AuctionView { get; set; }
		public Decimal PriceFrom { get; set; }
		public Decimal PriceTo { get; set; }
		public bool HidePrice { get; set; }
		public string PriceView { get; set; }
		public Decimal Rent { get; set; }
		public Decimal Lease { get; set; }
		public int Bed { get; set; }
		public int Bath { get; set; }
		public int Car { get; set; }
		public int Land { get; set; }
		public DateTime? AvlFrom { get; set; }
		public bool Display { get; set; }
		public int ListingId { get; set; }
		public bool ShowAddress { get; set; }
		public Decimal Price { get; set; }
		public Decimal RentPerMonth { get; set; }
		public bool ShowRent { get; set; }
		public string Authority { get; set; }
		public string LandSizeUnit { get; set; }
		public int ListingAddedBy { get; set; }

		public List<InspectionDate> InspectionDates { get; set; }
	}
}
