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

namespace Abc.OnlineBL.Orders.Workflow
{
    partial class OnlineBLSubmitOrder
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
            System.Workflow.Activities.CodeCondition codecondition9 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition10 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition11 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition12 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition13 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition14 = new System.Workflow.Activities.CodeCondition();
            this.GenerateManagerStockId = new System.Workflow.Activities.CodeActivity();
            this.GenerateNormalStockId = new System.Workflow.Activities.CodeActivity();
            this.GeneratePhotoOrderId = new System.Workflow.Activities.CodeActivity();
            this.CreateSpotlightFile = new System.Workflow.Activities.CodeActivity();
            this.GenerateOrderEvent = new System.Workflow.Activities.CodeActivity();
            this.GenerateB2BOrderEvent = new System.Workflow.Activities.CodeActivity();
            this.GenerateDIYStockEvent = new System.Workflow.Activities.CodeActivity();
            this.CreateDIYStockFile = new System.Workflow.Activities.CodeActivity();
            this.GenerateDIYManagerStockId = new System.Workflow.Activities.CodeActivity();
            this.GenerateDIYPhotoOrderID = new System.Workflow.Activities.CodeActivity();
            this.ifWorkshopClientForSBNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifWorkshopClientForSBYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifWorkshopClientNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifWorkshopClientYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifHasSpotlightNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifHasSpotlightYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifB2BOrderNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifB2BOrderYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYWorkshopClientForSBNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYWorkshopClientForSBYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYWorkshopClientNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYWorkshopClientYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.GenerateStockEvent = new System.Workflow.Activities.CodeActivity();
            this.CreateStockFile = new System.Workflow.Activities.CodeActivity();
            this.ifElseWorkshopClientForSB = new System.Workflow.Activities.IfElseActivity();
            this.GeneratePhotoEvent = new System.Workflow.Activities.CodeActivity();
            this.CreatePhotoFile = new System.Workflow.Activities.CodeActivity();
            this.ifElseWorkshopClient = new System.Workflow.Activities.IfElseActivity();
            this.ifElseHasSpotlight = new System.Workflow.Activities.IfElseActivity();
            this.ifB2BOrder = new System.Workflow.Activities.IfElseActivity();
            this.CreateOrderFile = new System.Workflow.Activities.CodeActivity();
            this.GenerateOrderID = new System.Workflow.Activities.CodeActivity();
            this.ifElseDIYWorkshopClientForSB = new System.Workflow.Activities.IfElseActivity();
            this.GenerateDIYPhotoEvent = new System.Workflow.Activities.CodeActivity();
            this.CreateDIYPhotoFile = new System.Workflow.Activities.CodeActivity();
            this.ifElseDIYWorkshopClient = new System.Workflow.Activities.IfElseActivity();
            this.GenerateDIYOrderEvent = new System.Workflow.Activities.CodeActivity();
            this.CreateDIYOrderFile = new System.Workflow.Activities.CodeActivity();
            this.GenerateDIYOrderID = new System.Workflow.Activities.CodeActivity();
            this.ifStockBoardNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifStockBoardYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifPhotographyNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifPhotographyYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifOtherProductNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifOtherProductYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYStockboardNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYStockboardYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYPhotographyNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYPhotographyYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYOtherProductNo = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYOtherProductYes = new System.Workflow.Activities.IfElseBranchActivity();
            this.NotifyClient = new System.Workflow.Activities.CodeActivity();
            this.ifElseStockBoard = new System.Workflow.Activities.IfElseActivity();
            this.ifElsePhotography = new System.Workflow.Activities.IfElseActivity();
            this.ifElseOtherProduct = new System.Workflow.Activities.IfElseActivity();
            this.NotifyClientDIYOrder = new System.Workflow.Activities.CodeActivity();
            this.ifElseDIYDStockboard = new System.Workflow.Activities.IfElseActivity();
            this.ifElseDIYPhotography = new System.Workflow.Activities.IfElseActivity();
            this.ifElseDIYOtherProduct = new System.Workflow.Activities.IfElseActivity();
            this.IfNotDIYOrder = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDIYOrder = new System.Workflow.Activities.IfElseBranchActivity();
            this.WFExceptionHandler = new System.Workflow.Activities.CodeActivity();
            this.PutOrderOnHold = new System.Workflow.Activities.CodeActivity();
            this.NotifyAccount = new System.Workflow.Activities.CodeActivity();
            this.UpdateAugment = new System.Workflow.Activities.CodeActivity();
            this.SaveB2BOrderInfo = new System.Workflow.Activities.CodeActivity();
            this.ApplySurcharge = new System.Workflow.Activities.CodeActivity();
            this.ifElseDIYOrder = new System.Workflow.Activities.IfElseActivity();
            this.wfFaultHandlerActivity = new System.Workflow.ComponentModel.FaultHandlerActivity();
            this.ifRegularOrder = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifPropertyOrder = new System.Workflow.Activities.IfElseBranchActivity();
            this.wfFaultHandlersActivity = new System.Workflow.ComponentModel.FaultHandlersActivity();
            this.ifElsePropertyOrderOrRegularOrder = new System.Workflow.Activities.IfElseActivity();
            this.CreateClientAccount = new System.Workflow.Activities.CodeActivity();
            this.TravelFeeCheck = new System.Workflow.Activities.CodeActivity();
            this.ErectionFeeCheck = new System.Workflow.Activities.CodeActivity();
            this.InitializeOrder = new System.Workflow.Activities.CodeActivity();
            // 
            // GenerateManagerStockId
            // 
            this.GenerateManagerStockId.Name = "GenerateManagerStockId";
            this.GenerateManagerStockId.ExecuteCode += new System.EventHandler(this.GenerateManagerStockId_ExecuteCode);
            // 
            // GenerateNormalStockId
            // 
            this.GenerateNormalStockId.Name = "GenerateNormalStockId";
            this.GenerateNormalStockId.ExecuteCode += new System.EventHandler(this.GenerateNormalStockId_ExecuteCode);
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
            // GenerateDIYStockEvent
            // 
            this.GenerateDIYStockEvent.Name = "GenerateDIYStockEvent";
            this.GenerateDIYStockEvent.ExecuteCode += new System.EventHandler(this.GenerateStockEvent_ExecuteCode);
            // 
            // CreateDIYStockFile
            // 
            this.CreateDIYStockFile.Name = "CreateDIYStockFile";
            this.CreateDIYStockFile.ExecuteCode += new System.EventHandler(this.CreateStockFile_ExecuteCode);
            // 
            // GenerateDIYManagerStockId
            // 
            this.GenerateDIYManagerStockId.Name = "GenerateDIYManagerStockId";
            this.GenerateDIYManagerStockId.ExecuteCode += new System.EventHandler(this.GenerateManagerStockId_ExecuteCode);
            // 
            // GenerateDIYPhotoOrderID
            // 
            this.GenerateDIYPhotoOrderID.Name = "GenerateDIYPhotoOrderID";
            this.GenerateDIYPhotoOrderID.ExecuteCode += new System.EventHandler(this.GeneratePhotoOrderId_ExecuteCode);
            // 
            // ifWorkshopClientForSBNo
            // 
            this.ifWorkshopClientForSBNo.Activities.Add(this.GenerateManagerStockId);
            this.ifWorkshopClientForSBNo.Name = "ifWorkshopClientForSBNo";
            // 
            // ifWorkshopClientForSBYes
            // 
            this.ifWorkshopClientForSBYes.Activities.Add(this.GenerateNormalStockId);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfWorkshopClientForSB);
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
            // ifB2BOrderYes
            // 
            this.ifB2BOrderYes.Activities.Add(this.GenerateB2BOrderEvent);
            codecondition4.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfB2BOrder);
            this.ifB2BOrderYes.Condition = codecondition4;
            this.ifB2BOrderYes.Name = "ifB2BOrderYes";
            // 
            // ifDIYWorkshopClientForSBNo
            // 
            this.ifDIYWorkshopClientForSBNo.Activities.Add(this.GenerateDIYManagerStockId);
            this.ifDIYWorkshopClientForSBNo.Activities.Add(this.CreateDIYStockFile);
            this.ifDIYWorkshopClientForSBNo.Activities.Add(this.GenerateDIYStockEvent);
            this.ifDIYWorkshopClientForSBNo.Name = "ifDIYWorkshopClientForSBNo";
            // 
            // ifDIYWorkshopClientForSBYes
            // 
            codecondition5.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfWorkshopClientForSB);
            this.ifDIYWorkshopClientForSBYes.Condition = codecondition5;
            this.ifDIYWorkshopClientForSBYes.Name = "ifDIYWorkshopClientForSBYes";
            // 
            // ifDIYWorkshopClientNo
            // 
            this.ifDIYWorkshopClientNo.Name = "ifDIYWorkshopClientNo";
            // 
            // ifDIYWorkshopClientYes
            // 
            this.ifDIYWorkshopClientYes.Activities.Add(this.GenerateDIYPhotoOrderID);
            codecondition6.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfWorkshopClient);
            this.ifDIYWorkshopClientYes.Condition = codecondition6;
            this.ifDIYWorkshopClientYes.Name = "ifDIYWorkshopClientYes";
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
            // ifElseWorkshopClientForSB
            // 
            this.ifElseWorkshopClientForSB.Activities.Add(this.ifWorkshopClientForSBYes);
            this.ifElseWorkshopClientForSB.Activities.Add(this.ifWorkshopClientForSBNo);
            this.ifElseWorkshopClientForSB.Name = "ifElseWorkshopClientForSB";
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
            // ifB2BOrder
            // 
            this.ifB2BOrder.Activities.Add(this.ifB2BOrderYes);
            this.ifB2BOrder.Activities.Add(this.ifB2BOrderNo);
            this.ifB2BOrder.Name = "ifB2BOrder";
            // 
            // CreateOrderFile
            // 
            this.CreateOrderFile.Name = "CreateOrderFile";
            this.CreateOrderFile.ExecuteCode += new System.EventHandler(this.CreateOrderFile_ExecuteCode);
            // 
            // GenerateOrderID
            // 
            this.GenerateOrderID.Name = "GenerateOrderID";
            this.GenerateOrderID.ExecuteCode += new System.EventHandler(this.GenerateOrderID_ExecuteCode);
            // 
            // ifElseDIYWorkshopClientForSB
            // 
            this.ifElseDIYWorkshopClientForSB.Activities.Add(this.ifDIYWorkshopClientForSBYes);
            this.ifElseDIYWorkshopClientForSB.Activities.Add(this.ifDIYWorkshopClientForSBNo);
            this.ifElseDIYWorkshopClientForSB.Name = "ifElseDIYWorkshopClientForSB";
            // 
            // GenerateDIYPhotoEvent
            // 
            this.GenerateDIYPhotoEvent.Name = "GenerateDIYPhotoEvent";
            this.GenerateDIYPhotoEvent.ExecuteCode += new System.EventHandler(this.GeneratePhotoEvent_ExecuteCode);
            // 
            // CreateDIYPhotoFile
            // 
            this.CreateDIYPhotoFile.Name = "CreateDIYPhotoFile";
            this.CreateDIYPhotoFile.ExecuteCode += new System.EventHandler(this.CreatePhotoFile_ExecuteCode);
            // 
            // ifElseDIYWorkshopClient
            // 
            this.ifElseDIYWorkshopClient.Activities.Add(this.ifDIYWorkshopClientYes);
            this.ifElseDIYWorkshopClient.Activities.Add(this.ifDIYWorkshopClientNo);
            this.ifElseDIYWorkshopClient.Name = "ifElseDIYWorkshopClient";
            // 
            // GenerateDIYOrderEvent
            // 
            this.GenerateDIYOrderEvent.Name = "GenerateDIYOrderEvent";
            this.GenerateDIYOrderEvent.ExecuteCode += new System.EventHandler(this.GenerateOrderEvent_ExecuteCode);
            // 
            // CreateDIYOrderFile
            // 
            this.CreateDIYOrderFile.Name = "CreateDIYOrderFile";
            this.CreateDIYOrderFile.ExecuteCode += new System.EventHandler(this.CreateOrderFile_ExecuteCode);
            // 
            // GenerateDIYOrderID
            // 
            this.GenerateDIYOrderID.Name = "GenerateDIYOrderID";
            this.GenerateDIYOrderID.ExecuteCode += new System.EventHandler(this.GenerateOrderID_ExecuteCode);
            // 
            // ifStockBoardNo
            // 
            this.ifStockBoardNo.Name = "ifStockBoardNo";
            // 
            // ifStockBoardYes
            // 
            this.ifStockBoardYes.Activities.Add(this.ifElseWorkshopClientForSB);
            this.ifStockBoardYes.Activities.Add(this.CreateStockFile);
            this.ifStockBoardYes.Activities.Add(this.GenerateStockEvent);
            codecondition7.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfStockBoard);
            this.ifStockBoardYes.Condition = codecondition7;
            this.ifStockBoardYes.Name = "ifStockBoardYes";
            // 
            // ifPhotographyNo
            // 
            this.ifPhotographyNo.Name = "ifPhotographyNo";
            // 
            // ifPhotographyYes
            // 
            this.ifPhotographyYes.Activities.Add(this.ifElseWorkshopClient);
            this.ifPhotographyYes.Activities.Add(this.CreatePhotoFile);
            this.ifPhotographyYes.Activities.Add(this.GeneratePhotoEvent);
            codecondition8.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfPhotography);
            this.ifPhotographyYes.Condition = codecondition8;
            this.ifPhotographyYes.Name = "ifPhotographyYes";
            // 
            // ifOtherProductNo
            // 
            this.ifOtherProductNo.Name = "ifOtherProductNo";
            // 
            // ifOtherProductYes
            // 
            this.ifOtherProductYes.Activities.Add(this.GenerateOrderID);
            this.ifOtherProductYes.Activities.Add(this.CreateOrderFile);
            this.ifOtherProductYes.Activities.Add(this.ifB2BOrder);
            this.ifOtherProductYes.Activities.Add(this.ifElseHasSpotlight);
            codecondition9.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfOtherProduct);
            this.ifOtherProductYes.Condition = codecondition9;
            this.ifOtherProductYes.Name = "ifOtherProductYes";
            // 
            // ifDIYStockboardNo
            // 
            this.ifDIYStockboardNo.Name = "ifDIYStockboardNo";
            // 
            // ifDIYStockboardYes
            // 
            this.ifDIYStockboardYes.Activities.Add(this.ifElseDIYWorkshopClientForSB);
            codecondition10.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfStockBoard);
            this.ifDIYStockboardYes.Condition = codecondition10;
            this.ifDIYStockboardYes.Name = "ifDIYStockboardYes";
            // 
            // ifDIYPhotographyNo
            // 
            this.ifDIYPhotographyNo.Name = "ifDIYPhotographyNo";
            // 
            // ifDIYPhotographyYes
            // 
            this.ifDIYPhotographyYes.Activities.Add(this.ifElseDIYWorkshopClient);
            this.ifDIYPhotographyYes.Activities.Add(this.CreateDIYPhotoFile);
            this.ifDIYPhotographyYes.Activities.Add(this.GenerateDIYPhotoEvent);
            codecondition11.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfPhotography);
            this.ifDIYPhotographyYes.Condition = codecondition11;
            this.ifDIYPhotographyYes.Name = "ifDIYPhotographyYes";
            // 
            // ifDIYOtherProductNo
            // 
            this.ifDIYOtherProductNo.Name = "ifDIYOtherProductNo";
            // 
            // ifDIYOtherProductYes
            // 
            this.ifDIYOtherProductYes.Activities.Add(this.GenerateDIYOrderID);
            this.ifDIYOtherProductYes.Activities.Add(this.CreateDIYOrderFile);
            this.ifDIYOtherProductYes.Activities.Add(this.GenerateDIYOrderEvent);
            codecondition12.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.DIYCheckIfOtherProduct);
            this.ifDIYOtherProductYes.Condition = codecondition12;
            this.ifDIYOtherProductYes.Name = "ifDIYOtherProductYes";
            // 
            // NotifyClient
            // 
            this.NotifyClient.Name = "NotifyClient";
            this.NotifyClient.ExecuteCode += new System.EventHandler(this.NotifyClient_ExecuteCode);
            // 
            // ifElseStockBoard
            // 
            this.ifElseStockBoard.Activities.Add(this.ifStockBoardYes);
            this.ifElseStockBoard.Activities.Add(this.ifStockBoardNo);
            this.ifElseStockBoard.Name = "ifElseStockBoard";
            // 
            // ifElsePhotography
            // 
            this.ifElsePhotography.Activities.Add(this.ifPhotographyYes);
            this.ifElsePhotography.Activities.Add(this.ifPhotographyNo);
            this.ifElsePhotography.Name = "ifElsePhotography";
            // 
            // ifElseOtherProduct
            // 
            this.ifElseOtherProduct.Activities.Add(this.ifOtherProductYes);
            this.ifElseOtherProduct.Activities.Add(this.ifOtherProductNo);
            this.ifElseOtherProduct.Name = "ifElseOtherProduct";
            // 
            // NotifyClientDIYOrder
            // 
            this.NotifyClientDIYOrder.Name = "NotifyClientDIYOrder";
            this.NotifyClientDIYOrder.ExecuteCode += new System.EventHandler(this.NotifyClient_ExecuteCode);
            // 
            // ifElseDIYDStockboard
            // 
            this.ifElseDIYDStockboard.Activities.Add(this.ifDIYStockboardYes);
            this.ifElseDIYDStockboard.Activities.Add(this.ifDIYStockboardNo);
            this.ifElseDIYDStockboard.Name = "ifElseDIYDStockboard";
            // 
            // ifElseDIYPhotography
            // 
            this.ifElseDIYPhotography.Activities.Add(this.ifDIYPhotographyYes);
            this.ifElseDIYPhotography.Activities.Add(this.ifDIYPhotographyNo);
            this.ifElseDIYPhotography.Name = "ifElseDIYPhotography";
            // 
            // ifElseDIYOtherProduct
            // 
            this.ifElseDIYOtherProduct.Activities.Add(this.ifDIYOtherProductYes);
            this.ifElseDIYOtherProduct.Activities.Add(this.ifDIYOtherProductNo);
            this.ifElseDIYOtherProduct.Name = "ifElseDIYOtherProduct";
            // 
            // IfNotDIYOrder
            // 
            this.IfNotDIYOrder.Activities.Add(this.ifElseOtherProduct);
            this.IfNotDIYOrder.Activities.Add(this.ifElsePhotography);
            this.IfNotDIYOrder.Activities.Add(this.ifElseStockBoard);
            this.IfNotDIYOrder.Activities.Add(this.NotifyClient);
            this.IfNotDIYOrder.Name = "IfNotDIYOrder";
            // 
            // ifDIYOrder
            // 
            this.ifDIYOrder.Activities.Add(this.ifElseDIYOtherProduct);
            this.ifDIYOrder.Activities.Add(this.ifElseDIYPhotography);
            this.ifDIYOrder.Activities.Add(this.ifElseDIYDStockboard);
            this.ifDIYOrder.Activities.Add(this.NotifyClientDIYOrder);
            codecondition13.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfDIYOrder);
            this.ifDIYOrder.Condition = codecondition13;
            this.ifDIYOrder.Name = "ifDIYOrder";
            // 
            // WFExceptionHandler
            // 
            this.WFExceptionHandler.Name = "WFExceptionHandler";
            this.WFExceptionHandler.ExecuteCode += new System.EventHandler(this.WFExceptionHandler_ExecuteCode);
            // 
            // PutOrderOnHold
            // 
            this.PutOrderOnHold.Name = "PutOrderOnHold";
            this.PutOrderOnHold.ExecuteCode += new System.EventHandler(this.PutOrderOnHold_ExecuteCode);
            // 
            // NotifyAccount
            // 
            this.NotifyAccount.Name = "NotifyAccount";
            this.NotifyAccount.ExecuteCode += new System.EventHandler(this.NotifyAccount_ExecuteCode);
            // 
            // UpdateAugment
            // 
            this.UpdateAugment.Name = "UpdateAugment";
            this.UpdateAugment.ExecuteCode += new System.EventHandler(this.UpdateAugment_ExecuteCode);
            // 
            // SaveB2BOrderInfo
            // 
            this.SaveB2BOrderInfo.Name = "SaveB2BOrderInfo";
            this.SaveB2BOrderInfo.ExecuteCode += new System.EventHandler(this.SaveB2BOrderInfo_ExecuteCode);
            // 
            // ApplySurcharge
            // 
            this.ApplySurcharge.Name = "ApplySurcharge";
            this.ApplySurcharge.ExecuteCode += new System.EventHandler(this.ApplySurcharge_ExecuteCode);
            // 
            // ifElseDIYOrder
            // 
            this.ifElseDIYOrder.Activities.Add(this.ifDIYOrder);
            this.ifElseDIYOrder.Activities.Add(this.IfNotDIYOrder);
            this.ifElseDIYOrder.Name = "ifElseDIYOrder";
            // 
            // wfFaultHandlerActivity
            // 
            this.wfFaultHandlerActivity.Activities.Add(this.WFExceptionHandler);
            this.wfFaultHandlerActivity.FaultType = typeof(System.Exception);
            this.wfFaultHandlerActivity.Name = "wfFaultHandlerActivity";
            // 
            // ifRegularOrder
            // 
            this.ifRegularOrder.Name = "ifRegularOrder";
            // 
            // ifPropertyOrder
            // 
            this.ifPropertyOrder.Activities.Add(this.ifElseDIYOrder);
            this.ifPropertyOrder.Activities.Add(this.ApplySurcharge);
            this.ifPropertyOrder.Activities.Add(this.SaveB2BOrderInfo);
            this.ifPropertyOrder.Activities.Add(this.UpdateAugment);
            this.ifPropertyOrder.Activities.Add(this.NotifyAccount);
            this.ifPropertyOrder.Activities.Add(this.PutOrderOnHold);
            codecondition14.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.CheckIfPropertyOrder);
            this.ifPropertyOrder.Condition = codecondition14;
            this.ifPropertyOrder.Name = "ifPropertyOrder";
            // 
            // wfFaultHandlersActivity
            // 
            this.wfFaultHandlersActivity.Activities.Add(this.wfFaultHandlerActivity);
            this.wfFaultHandlersActivity.Name = "wfFaultHandlersActivity";
            // 
            // ifElsePropertyOrderOrRegularOrder
            // 
            this.ifElsePropertyOrderOrRegularOrder.Activities.Add(this.ifPropertyOrder);
            this.ifElsePropertyOrderOrRegularOrder.Activities.Add(this.ifRegularOrder);
            this.ifElsePropertyOrderOrRegularOrder.Name = "ifElsePropertyOrderOrRegularOrder";
            // 
            // CreateClientAccount
            // 
            this.CreateClientAccount.Name = "CreateClientAccount";
            this.CreateClientAccount.ExecuteCode += new System.EventHandler(this.CreateClientAccount_ExecuteCode);
            // 
            // TravelFeeCheck
            // 
            this.TravelFeeCheck.Name = "TravelFeeCheck";
            this.TravelFeeCheck.ExecuteCode += new System.EventHandler(this.TravelFeeCheck_ExecuteCode);
            // 
            // ErectionFeeCheck
            // 
            this.ErectionFeeCheck.Name = "ErectionFeeCheck";
            this.ErectionFeeCheck.ExecuteCode += new System.EventHandler(this.ErectionFeeCheck_ExecuteCode);
            // 
            // InitializeOrder
            // 
            this.InitializeOrder.Name = "InitializeOrder";
            this.InitializeOrder.ExecuteCode += new System.EventHandler(this.InitializeOrder_ExecuteCode);
            // 
            // OnlineBLSubmitOrder
            // 
            this.Activities.Add(this.InitializeOrder);
            this.Activities.Add(this.ErectionFeeCheck);
            this.Activities.Add(this.TravelFeeCheck);
            this.Activities.Add(this.CreateClientAccount);
            this.Activities.Add(this.ifElsePropertyOrderOrRegularOrder);
            this.Activities.Add(this.wfFaultHandlersActivity);
            this.Name = "OnlineBLSubmitOrder";
            this.CanModifyActivities = false;

        }










































































        #endregion

        private CodeActivity SaveB2BOrderInfo;
        private CodeActivity PutOrderOnHold;
        private CodeActivity ApplySurcharge;
        private CodeActivity UpdateAugment;
        private CodeActivity NotifyAccount;
        private CodeActivity TravelFeeCheck;
        private CodeActivity GenerateDIYManagerStockId;
        private CodeActivity GenerateDIYPhotoOrderID;
        private CodeActivity GenerateDIYOrderEvent;
        private IfElseBranchActivity ifDIYWorkshopClientForSBNo;
        private IfElseBranchActivity ifDIYWorkshopClientForSBYes;
        private IfElseBranchActivity ifDIYWorkshopClientNo;
        private IfElseBranchActivity ifDIYWorkshopClientYes;
        private CodeActivity GenerateDIYStockEvent;
        private CodeActivity CreateDIYStockFile;
        private IfElseActivity ifElseDIYWorkshopClientForSB;
        private CodeActivity GenerateDIYPhotoEvent;
        private CodeActivity CreateDIYPhotoFile;
        private IfElseActivity ifElseDIYWorkshopClient;
        private CodeActivity CreateDIYOrderFile;
        private CodeActivity GenerateDIYOrderID;
        private IfElseBranchActivity ifDIYStockboardNo;
        private IfElseBranchActivity ifDIYStockboardYes;
        private IfElseBranchActivity ifDIYPhotographyNo;
        private IfElseBranchActivity ifDIYPhotographyYes;
        private IfElseBranchActivity ifDIYOtherProductNo;
        private IfElseBranchActivity ifDIYOtherProductYes;
        private CodeActivity NotifyClientDIYOrder;
        private IfElseActivity ifElseDIYDStockboard;
        private IfElseActivity ifElseDIYPhotography;
        private IfElseActivity ifElseDIYOtherProduct;
        private IfElseBranchActivity IfNotDIYOrder;
        private IfElseBranchActivity ifDIYOrder;
        private IfElseActivity ifElseDIYOrder;
        private CodeActivity CreateClientAccount;
        private CodeActivity WFExceptionHandler;
        private FaultHandlerActivity wfFaultHandlerActivity;
        private FaultHandlersActivity wfFaultHandlersActivity;
        private CodeActivity ErectionFeeCheck;
        private IfElseBranchActivity ifRegularOrder;
        private IfElseBranchActivity ifPropertyOrder;
        private IfElseActivity ifElsePropertyOrderOrRegularOrder;
        private CodeActivity GenerateOrderID;
        private CodeActivity CreateOrderFile;
        private IfElseBranchActivity ifOtherProductNo;
        private IfElseBranchActivity ifOtherProductYes;
        private IfElseActivity ifElseOtherProduct;
        private IfElseBranchActivity ifB2BOrderNo;
        private IfElseBranchActivity ifB2BOrderYes;
        private IfElseActivity ifB2BOrder;
        private CodeActivity GenerateB2BOrderEvent;
        private CodeActivity GenerateOrderEvent;
        private IfElseBranchActivity ifHasSpotlightNo;
        private IfElseBranchActivity ifHasSpotlightYes;
        private IfElseActivity ifElseHasSpotlight;
        private CodeActivity CreateSpotlightFile;
        private IfElseBranchActivity ifPhotographyNo;
        private IfElseBranchActivity ifPhotographyYes;
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
        private IfElseActivity ifElseWorkshopClientForSB;
        private CodeActivity GenerateNormalStockId;
        private CodeActivity GenerateManagerStockId;
        private CodeActivity CreateStockFile;
        private CodeActivity GenerateStockEvent;
        private CodeActivity NotifyClient;
        private CodeActivity InitializeOrder;
    }
}
