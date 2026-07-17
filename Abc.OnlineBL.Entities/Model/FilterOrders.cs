using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	public class FilterOrders
	{
		[DataMember]
		public int OrderID { get; set; }
		[DataMember]
		public string PAddress { get; set; }
		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Products { get; set; }
		[DataMember]
		public int PropertyID { get; set; }
		[DataMember]
		public bool SMS_Allowed { get; set; }
		[DataMember]
		public bool HasProof { get; set; }
		[DataMember]
		public bool HasDIY { get; set; }
		[DataMember]
		public bool HasBoardErectionApplicable { get; set; }
		[DataMember]
		public bool HasBoardRemovalApplicable { get; set; }
		[DataMember]
		public string OrderStatus { get; set; }
		
		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetCurrentOrdersResult or)
        {
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof;
			this.HasDIY = or.HasDIY;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable;
			this.OrderStatus = or.OrderStatus;
        }

		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetAwaitingForErectionOrdersResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof.HasValue ? or.HasProof.Value : false;
			this.HasDIY = or.HasDIY.HasValue ? or.HasDIY.Value : false;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable.HasValue ? or.HasBoardRemovalApplicable.Value : false;
			this.OrderStatus = or.OrderStatus;
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetOnlineDesignOrdersResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof.HasValue ? or.HasProof.Value : false;
			this.HasDIY = or.HasDIY.HasValue ? or.HasDIY.Value : false;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable.HasValue ? or.HasBoardErectionApplicable.Value : false;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable.HasValue ? or.HasBoardRemovalApplicable.Value : false;
			this.OrderStatus = or.OrderStatus;
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetWaitingForApprovalOrdersResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof.HasValue ? or.HasProof.Value : false;
            this.HasDIY = or.HasDIY;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable.HasValue ? or.HasBoardRemovalApplicable.Value : false;
            this.OrderStatus = or.OrderStatus;
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetWaitingForRemovalOrdersResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof.HasValue ? or.HasProof.Value : false;
            this.HasDIY = or.HasDIY.HasValue ? or.HasDIY.Value : false;
            this.HasBoardErectionApplicable = or.HasBoardErectionApplicable.HasValue ? or.HasBoardErectionApplicable.Value : false;
            this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable;
			this.OrderStatus = or.OrderStatus;            
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetSMSOnDemandOrdersResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof;
			this.HasDIY = or.HasDIY;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable;
			this.OrderStatus = or.OrderStatus;
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_SearchByDigitResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof;
			this.HasDIY = or.HasDIY;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable;
			this.OrderStatus = or.OrderStatus;
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_SearchByAlphaNumericResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof;
			this.HasDIY = or.HasDIY;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable;
			this.OrderStatus = or.OrderStatus;
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetRecentPropertyResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof;
			this.HasDIY = or.HasDIY;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable;
			this.OrderStatus = or.OrderStatus;
		}

		public FilterOrders(Abc.OnlineBL.Entities.AIS_GetRegularOrdersResult or)
		{
			this.OrderID = or.OrderID;
			this.PAddress = or.PAddress;
			this.Caption = or.Caption;
			this.Products = or.Products;
			this.PropertyID = or.PropertyID;
			this.SMS_Allowed = or.SMS_Allowed;
			this.HasProof = or.HasProof;
			this.HasDIY = or.HasDIY;
			this.HasBoardErectionApplicable = or.HasBoardErectionApplicable;
			this.HasBoardRemovalApplicable = or.HasBoardRemovalApplicable;
			this.OrderStatus = or.OrderStatus;
		}
	}
}
