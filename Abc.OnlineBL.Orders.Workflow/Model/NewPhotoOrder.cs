using Abc.OnlineBL.Entities.Model.OnlineOrder;
namespace Abc.OnlineBL.Orders.Workflow.Model
{
	public class NewPhotoOrder
	{
		public int ClientId { get; set; }
		public int LocId { get; set; }
		public string Property { get; set; }
		public string Caption { get; set; }
		public string Notes { get; set; }
		public string ErectionNotes { get; set; }
		public string OrderData { get; set; }
		public string RefNo { get; set; }
		public int OrderId { get; set; }

		public string Instructions { get; set; }
		public string VendorName { get; set; }
		public string VendorPhone { get; set; }
		public bool IsKeySafe { get; set; }
		public bool IsPickupKeys { get; set; }
		public string PhotoContact { get; set; }
		public string HouseFaces { get; set; }
		public string Melway { get; set; }
		public string SendBy { get; set; }
        public string SendTo { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
		public int PropertyId { get; set; }
        public string PhotographyFile { get; set; }
        public string SitePlanInstructionFile { get; set; }

        public OnlinePropertyOrder PropertyOrder { get; set; }

	}
}
