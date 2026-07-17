using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Abc.AIS.Orders.Workflow
{
	partial class ProcessOrderWF
	{
		#region Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode]
		private void InitializeComponent()
		{
			this.CanModifyActivities = true;
			System.Workflow.Activities.CodeCondition codecondition1 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.Activities.CodeCondition codecondition2 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.Activities.CodeCondition codecondition3 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.Activities.CodeCondition codecondition4 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.Activities.CodeCondition codecondition5 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.Activities.CodeCondition codecondition6 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.Activities.CodeCondition codecondition7 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.Activities.CodeCondition codecondition8 = new System.Workflow.Activities.CodeCondition();
			this.GenerateManagerStockId = new System.Workflow.Activities.CodeActivity();
			this.GenerateNormalStockID = new System.Workflow.Activities.CodeActivity();
			this.GeneratePhotoOrderId = new System.Workflow.Activities.CodeActivity();
			this.CreateSpotlightFile = new System.Workflow.Activities.CodeActivity();
			this.GenerateOrderEvent = new System.Workflow.Activities.CodeActivity();
			this.GenerateB2BOrderEvent = new System.Workflow.Activities.CodeActivity();
			this.ifWorkshopClientForSBNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifWorkshopClientForSBYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifWorkshopClientNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifWorkshopClientYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifHasSpotlightNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifHasSpotlightYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifB2BOrderNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.IfB2BOrderYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.WFExceptionHandler = new System.Workflow.Activities.CodeActivity();
			this.ProcessOnlineListing = new System.Workflow.Activities.CodeActivity();
			this.GenerateStockEvent = new System.Workflow.Activities.CodeActivity();
			this.CreateStockFile = new System.Workflow.Activities.CodeActivity();
			this.IfElseWorkshopClientForSB = new System.Workflow.Activities.IfElseActivity();
			this.GeneratePhotoEvent = new System.Workflow.Activities.CodeActivity();
			this.CreatePhotoFile = new System.Workflow.Activities.CodeActivity();
			this.ifElseWorkshopClient = new System.Workflow.Activities.IfElseActivity();
			this.ifElseHasSpotlight = new System.Workflow.Activities.IfElseActivity();
			this.ProcessAOPOrder = new System.Workflow.Activities.CodeActivity();
			this.ifB2BOrder = new System.Workflow.Activities.IfElseActivity();
			this.CreateOrderFile = new System.Workflow.Activities.CodeActivity();
			this.GenerateOrderId = new System.Workflow.Activities.CodeActivity();
			this.wfFaultHandlerActivity = new System.Workflow.ComponentModel.FaultHandlerActivity();
			this.ifHasOnlineListingNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifHasOnlineListingYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifStockBoardNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifStockBoardYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifElsePhotographyNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifElsePhotographyYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifOtherProductsNo = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifOtherProductsYes = new System.Workflow.Activities.IfElseBranchActivity();
			this.wfFaultHandlersActivity = new System.Workflow.ComponentModel.FaultHandlersActivity();
			this.NotifyClient = new System.Workflow.Activities.CodeActivity();
			this.ifElseOrderHasOnlineListing = new System.Workflow.Activities.IfElseActivity();
			this.ifElseStockBoard = new System.Workflow.Activities.IfElseActivity();
			this.ifElsePhotography = new System.Workflow.Activities.IfElseActivity();
			this.ifElseOtherProducts = new System.Workflow.Activities.IfElseActivity();
			this.InitialzeOrder = new System.Workflow.Activities.CodeActivity();
			// 
			// GenerateManagerStockId
			// 
			this.GenerateManagerStockId.Name = "GenerateManagerStockId";
			this.GenerateManagerStockId.ExecuteCode += new System.EventHandler(this.GenerateManagerStockId_ExecuteCode);
			// 
			// GenerateNormalStockID
			// 
			this.GenerateNormalStockID.Name = "GenerateNormalStockID";
			this.GenerateNormalStockID.ExecuteCode += new System.EventHandler(this.GenerateNormalStockID_ExecuteCode);
			// 
			// GeneratePhotoOrderId
			// 
			this.GeneratePhotoOrderId.Name = "GeneratePhotoOrderId";
			this.GeneratePhotoOrderId.ExecuteCode += new System.EventHandler(this.GeneratePhotoOrderId_ExecuteCode);
			// 
			// CreateSpotlightFile
			// 
			this.CreateSpotlightFile.Name = "CreateSpotlightFile";
			this.CreateSpotlightFile.ExecuteCode += new System.EventHandler(this.CreateSpotlightFile_ExecuteCode);
			// 
			// GenerateOrderEvent
			// 
			this.GenerateOrderEvent.Name = "GenerateOrderEvent";
			this.GenerateOrderEvent.ExecuteCode += new System.EventHandler(this.GenerateOrderEvent_ExecuteCode);
			// 
			// GenerateB2BOrderEvent
			// 
			this.GenerateB2BOrderEvent.Name = "GenerateB2BOrderEvent";
			this.GenerateB2BOrderEvent.ExecuteCode += new System.EventHandler(this.GenerateB2BOrderEvent_ExecuteCode);
			// 
			// ifWorkshopClientForSBNo
			// 
			this.ifWorkshopClientForSBNo.Activities.Add(this.GenerateManagerStockId);
			this.ifWorkshopClientForSBNo.Name = "ifWorkshopClientForSBNo";
			// 
			// ifWorkshopClientForSBYes
			// 
			this.ifWorkshopClientForSBYes.Activities.Add(this.GenerateNormalStockID);
			codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfWorkshopClient);
			this.ifWorkshopClientForSBYes.Condition = codecondition1;
			this.ifWorkshopClientForSBYes.Name = "ifWorkshopClientForSBYes";
			// 
			// ifWorkshopClientNo
			// 
			this.ifWorkshopClientNo.Name = "ifWorkshopClientNo";
			// 
			// ifWorkshopClientYes
			// 
			this.ifWorkshopClientYes.Activities.Add(this.GeneratePhotoOrderId);
			codecondition2.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfWorkshopClient);
			this.ifWorkshopClientYes.Condition = codecondition2;
			this.ifWorkshopClientYes.Name = "ifWorkshopClientYes";
			// 
			// ifHasSpotlightNo
			// 
			this.ifHasSpotlightNo.Name = "ifHasSpotlightNo";
			// 
			// ifHasSpotlightYes
			// 
			this.ifHasSpotlightYes.Activities.Add(this.CreateSpotlightFile);
			codecondition3.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfHasSpotlight);
			this.ifHasSpotlightYes.Condition = codecondition3;
			this.ifHasSpotlightYes.Name = "ifHasSpotlightYes";
			// 
			// ifB2BOrderNo
			// 
			this.ifB2BOrderNo.Activities.Add(this.GenerateOrderEvent);
			this.ifB2BOrderNo.Name = "ifB2BOrderNo";
			// 
			// IfB2BOrderYes
			// 
			this.IfB2BOrderYes.Activities.Add(this.GenerateB2BOrderEvent);
			codecondition4.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfB2BOrder);
			this.IfB2BOrderYes.Condition = codecondition4;
			this.IfB2BOrderYes.Name = "IfB2BOrderYes";
			// 
			// WFExceptionHandler
			// 
			this.WFExceptionHandler.Name = "WFExceptionHandler";
			this.WFExceptionHandler.ExecuteCode += new System.EventHandler(this.WFExceptionHandler_ExecuteCode);
			// 
			// ProcessOnlineListing
			// 
			this.ProcessOnlineListing.Name = "ProcessOnlineListing";
			this.ProcessOnlineListing.ExecuteCode += new System.EventHandler(this.ProcessOnlineListing_ExecuteCode);
			// 
			// GenerateStockEvent
			// 
			this.GenerateStockEvent.Name = "GenerateStockEvent";
			this.GenerateStockEvent.ExecuteCode += new System.EventHandler(this.GenerateStockEvent_ExecuteCode);
			// 
			// CreateStockFile
			// 
			this.CreateStockFile.Name = "CreateStockFile";
			this.CreateStockFile.ExecuteCode += new System.EventHandler(this.CreateStockFile_ExecuteCode);
			// 
			// IfElseWorkshopClientForSB
			// 
			this.IfElseWorkshopClientForSB.Activities.Add(this.ifWorkshopClientForSBYes);
			this.IfElseWorkshopClientForSB.Activities.Add(this.ifWorkshopClientForSBNo);
			this.IfElseWorkshopClientForSB.Name = "IfElseWorkshopClientForSB";
			// 
			// GeneratePhotoEvent
			// 
			this.GeneratePhotoEvent.Name = "GeneratePhotoEvent";
			this.GeneratePhotoEvent.ExecuteCode += new System.EventHandler(this.GeneratePhotoEvent_ExecuteCode);
			// 
			// CreatePhotoFile
			// 
			this.CreatePhotoFile.Name = "CreatePhotoFile";
			this.CreatePhotoFile.ExecuteCode += new System.EventHandler(this.CreatePhotoFile_ExecuteCode);
			// 
			// ifElseWorkshopClient
			// 
			this.ifElseWorkshopClient.Activities.Add(this.ifWorkshopClientYes);
			this.ifElseWorkshopClient.Activities.Add(this.ifWorkshopClientNo);
			this.ifElseWorkshopClient.Name = "ifElseWorkshopClient";
			// 
			// ifElseHasSpotlight
			// 
			this.ifElseHasSpotlight.Activities.Add(this.ifHasSpotlightYes);
			this.ifElseHasSpotlight.Activities.Add(this.ifHasSpotlightNo);
			this.ifElseHasSpotlight.Name = "ifElseHasSpotlight";
			// 
			// ProcessAOPOrder
			// 
			this.ProcessAOPOrder.Name = "ProcessAOPOrder";
			this.ProcessAOPOrder.ExecuteCode += new System.EventHandler(this.ProcessAOPOrder_ExecuteCode);
			// 
			// ifB2BOrder
			// 
			this.ifB2BOrder.Activities.Add(this.IfB2BOrderYes);
			this.ifB2BOrder.Activities.Add(this.ifB2BOrderNo);
			this.ifB2BOrder.Name = "ifB2BOrder";
			// 
			// CreateOrderFile
			// 
			this.CreateOrderFile.Name = "CreateOrderFile";
			this.CreateOrderFile.ExecuteCode += new System.EventHandler(this.CreateOrderFile_ExecuteCode);
			// 
			// GenerateOrderId
			// 
			this.GenerateOrderId.Name = "GenerateOrderId";
			this.GenerateOrderId.ExecuteCode += new System.EventHandler(this.GenerateOrderId_ExecuteCode);
			// 
			// wfFaultHandlerActivity
			// 
			this.wfFaultHandlerActivity.Activities.Add(this.WFExceptionHandler);
			this.wfFaultHandlerActivity.FaultType = typeof(System.Exception);
			this.wfFaultHandlerActivity.Name = "wfFaultHandlerActivity";
			// 
			// ifHasOnlineListingNo
			// 
			this.ifHasOnlineListingNo.Name = "ifHasOnlineListingNo";
			// 
			// ifHasOnlineListingYes
			// 
			this.ifHasOnlineListingYes.Activities.Add(this.ProcessOnlineListing);
			codecondition5.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfOrderHasOnlineListing);
			this.ifHasOnlineListingYes.Condition = codecondition5;
			this.ifHasOnlineListingYes.Name = "ifHasOnlineListingYes";
			// 
			// ifStockBoardNo
			// 
			this.ifStockBoardNo.Name = "ifStockBoardNo";
			// 
			// ifStockBoardYes
			// 
			this.ifStockBoardYes.Activities.Add(this.IfElseWorkshopClientForSB);
			this.ifStockBoardYes.Activities.Add(this.CreateStockFile);
			this.ifStockBoardYes.Activities.Add(this.GenerateStockEvent);
			codecondition6.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfStockBoard);
			this.ifStockBoardYes.Condition = codecondition6;
			this.ifStockBoardYes.Name = "ifStockBoardYes";
			// 
			// ifElsePhotographyNo
			// 
			this.ifElsePhotographyNo.Name = "ifElsePhotographyNo";
			// 
			// ifElsePhotographyYes
			// 
			this.ifElsePhotographyYes.Activities.Add(this.ifElseWorkshopClient);
			this.ifElsePhotographyYes.Activities.Add(this.CreatePhotoFile);
			this.ifElsePhotographyYes.Activities.Add(this.GeneratePhotoEvent);
			codecondition7.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfPhotography);
			this.ifElsePhotographyYes.Condition = codecondition7;
			this.ifElsePhotographyYes.Name = "ifElsePhotographyYes";
			// 
			// ifOtherProductsNo
			// 
			this.ifOtherProductsNo.Name = "ifOtherProductsNo";
			// 
			// ifOtherProductsYes
			// 
			this.ifOtherProductsYes.Activities.Add(this.GenerateOrderId);
			this.ifOtherProductsYes.Activities.Add(this.CreateOrderFile);
			this.ifOtherProductsYes.Activities.Add(this.ifB2BOrder);
			this.ifOtherProductsYes.Activities.Add(this.ProcessAOPOrder);
			this.ifOtherProductsYes.Activities.Add(this.ifElseHasSpotlight);
			codecondition8.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfOtherProducts);
			this.ifOtherProductsYes.Condition = codecondition8;
			this.ifOtherProductsYes.Name = "ifOtherProductsYes";
			// 
			// wfFaultHandlersActivity
			// 
			this.wfFaultHandlersActivity.Activities.Add(this.wfFaultHandlerActivity);
			this.wfFaultHandlersActivity.Name = "wfFaultHandlersActivity";
			// 
			// NotifyClient
			// 
			this.NotifyClient.Name = "NotifyClient";
			this.NotifyClient.ExecuteCode += new System.EventHandler(this.NotifyClient_ExecuteCode);
			// 
			// ifElseOrderHasOnlineListing
			// 
			this.ifElseOrderHasOnlineListing.Activities.Add(this.ifHasOnlineListingYes);
			this.ifElseOrderHasOnlineListing.Activities.Add(this.ifHasOnlineListingNo);
			this.ifElseOrderHasOnlineListing.Name = "ifElseOrderHasOnlineListing";
			// 
			// ifElseStockBoard
			// 
			this.ifElseStockBoard.Activities.Add(this.ifStockBoardYes);
			this.ifElseStockBoard.Activities.Add(this.ifStockBoardNo);
			this.ifElseStockBoard.Name = "ifElseStockBoard";
			// 
			// ifElsePhotography
			// 
			this.ifElsePhotography.Activities.Add(this.ifElsePhotographyYes);
			this.ifElsePhotography.Activities.Add(this.ifElsePhotographyNo);
			this.ifElsePhotography.Name = "ifElsePhotography";
			// 
			// ifElseOtherProducts
			// 
			this.ifElseOtherProducts.Activities.Add(this.ifOtherProductsYes);
			this.ifElseOtherProducts.Activities.Add(this.ifOtherProductsNo);
			this.ifElseOtherProducts.Name = "ifElseOtherProducts";
			// 
			// InitialzeOrder
			// 
			this.InitialzeOrder.Name = "InitialzeOrder";
			this.InitialzeOrder.ExecuteCode += new System.EventHandler(this.InitialzeOrder_ExecuteCode);
			// 
			// ProcessOrderWF
			// 
			this.Activities.Add(this.InitialzeOrder);
			this.Activities.Add(this.ifElseOtherProducts);
			this.Activities.Add(this.ifElsePhotography);
			this.Activities.Add(this.ifElseStockBoard);
			this.Activities.Add(this.ifElseOrderHasOnlineListing);
			this.Activities.Add(this.NotifyClient);
			this.Activities.Add(this.wfFaultHandlersActivity);
			this.Name = "ProcessOrderWF";
			this.CanModifyActivities = false;

		}

		#endregion

		private FaultHandlerActivity wfFaultHandlerActivity;
		private FaultHandlersActivity wfFaultHandlersActivity;
		private CodeActivity WFExceptionHandler;
		private CodeActivity InitialzeOrder;
		private IfElseBranchActivity ifOtherProductsNo;
		private IfElseBranchActivity ifOtherProductsYes;
		private IfElseActivity ifElseOtherProducts;
		private CodeActivity GenerateOrderId;
		private CodeActivity CreateOrderFile;
		private IfElseBranchActivity ifB2BOrderNo;
		private IfElseBranchActivity IfB2BOrderYes;
		private IfElseActivity ifB2BOrder;
		private CodeActivity GenerateB2BOrderEvent;
		private CodeActivity GenerateOrderEvent;
		private CodeActivity ProcessAOPOrder;
		private IfElseBranchActivity ifHasSpotlightNo;
		private IfElseBranchActivity ifHasSpotlightYes;
		private IfElseActivity ifElseHasSpotlight;
		private CodeActivity CreateSpotlightFile;
		private IfElseBranchActivity ifElsePhotographyNo;
		private IfElseBranchActivity ifElsePhotographyYes;
		private IfElseActivity ifElsePhotography;
		private IfElseBranchActivity ifWorkshopClientNo;
		private IfElseBranchActivity ifWorkshopClientYes;
		private IfElseActivity ifElseWorkshopClient;
		private CodeActivity GeneratePhotoOrderId;
		private CodeActivity CreatePhotoFile;
		private CodeActivity GeneratePhotoEvent;
		private IfElseBranchActivity ifStockBoardNo;
		private IfElseBranchActivity ifStockBoardYes;
		private IfElseActivity ifElseStockBoard;
		private IfElseBranchActivity ifWorkshopClientForSBNo;
		private IfElseBranchActivity ifWorkshopClientForSBYes;
		private IfElseActivity IfElseWorkshopClientForSB;
		private CodeActivity GenerateNormalStockID;
		private CodeActivity GenerateManagerStockId;
		private CodeActivity CreateStockFile;
		private CodeActivity GenerateStockEvent;
		private IfElseBranchActivity ifHasOnlineListingNo;
		private IfElseBranchActivity ifHasOnlineListingYes;
		private IfElseActivity ifElseOrderHasOnlineListing;
		private CodeActivity ProcessOnlineListing;
		private CodeActivity NotifyClient;































































































	}
}
