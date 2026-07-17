using System;
using Abc.OnlineBL.Entities.Model.OnlineOrder;

namespace Abc.OnlineBL.Orders.Workflow.Model
{
	public class NewStockBoardOrder
	{
		public int ClientId { get; set; }
		public string State { get; set; }
		public string Loc { get; set; }
		public string Property { get; set; }
		public string Caption { get; set; }
		public string Notes { get; set; }
		public int SBOrderId { get; set; }
		public DateTime? PreferredErectionDate { get; set; }
		public int? PreferredErectionType { get; set; }
		public int PropertyId { get; set; }
		public int? AgentContactId { get; set; }
        public string ManagerID { get; set; }

		public OnlinePropertyOrder PropertyOrder { get; set; }
	}
}
