using System;
using Abc.OnlineBL.Entities.Model.OnlineOrder;

namespace Abc.OnlineBL.Orders.Workflow.Model
{
	public class NewOrder
	{
		public int ClientId { get; set; }
		public int LocId { get; set; }
		public string Property { get; set; }
		public string Caption { get; set; }
		public string Notes { get; set; }
		public bool NoBoards { get; set; }
		public string ErectionNotes { get; set; }
		public string SendBy { get; set; }
		public string SendTo { get; set; }
		public int OrderId { get; set; }
		public string OrderData { get; set; }
		public string RefNo { get; set; }
		public bool TransformListing { get; set; }
		public bool SendSms { get; set; }
		public string SmsText { get; set; }
		public string SmsAgentMobileNo { get; set; }
		public bool SmsNotifyAgent { get; set; }
		public bool SmsSendEmail { get; set; }
		public string SmsAgentEmailAdd { get; set; }
		public bool InddTemplatesAvail { get; set; }
		public bool HasCommunityBoard { get; set; }
		public bool MMS_Allowed { get; set; }
		public DateTime? PreferredErectionDate { get; set; }
		public int PreferredErectionType { get; set; }
		public DateTime? PreferredRemovalDate { get; set; }
		public int PreferredRemovalType { get; set; }
		public int PropertyId { get; set; }
		public bool IsStockBoardOrder { get; set; }
		public string InstallFile { get; set; }
		public bool IsSiteInpectionRequired { get; set; }
		public string SiteInspectionNotes { get; set; }

		public string ManagerId { get; set; }
		public bool HasTextReceived { get; set; }
		public OnlinePropertyOrder PropertyOrder { get; set; }

		public int? AgentContactId { get; set; }
		public bool IsDIYOrder { get; set; }
        public bool? IsExpressOrder { get; set; }

        public string DeliveryPreference { get; set; }
        public bool HasDeliveryDetails { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryEmail { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliverySuburb { get; set; }
        public string DeliveryPostCode { get; set; }
        public int DeliveryLocationId { get; set; }
        public string DeliveryState { get; set; }
        public string ManagerState { get; set; }
    }
}
