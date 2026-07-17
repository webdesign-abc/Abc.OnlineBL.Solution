using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using System.ServiceModel;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Service.Implementation.BusinessLogic;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Service.Implementation.Model;
using System.IO;
using Abc.OnlineBL.VirtualFileSystem;
using System.Threading;

namespace Abc.OnlineBL.Service.Implementation
{
	public partial class WorkflowService : IWorkflowService
	{
		#region SayHello
		public string SayHello(string name)
		{
			string sName = "Anonymous User";
			if (OperationContext.Current.ServiceSecurityContext != null)
			{
				if (OperationContext.Current.ServiceSecurityContext.WindowsIdentity != null)
				{
					sName = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
				}
				else if (OperationContext.Current.ServiceSecurityContext.IsAnonymous)
				{
					sName = "Anonymous User";
				}
			}
			string dbResult = string.Empty;
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					var userName = ctx.ExecuteQuery<string>("SELECT '' + SYSTEM_USER").FirstOrDefault();

					if (!string.IsNullOrEmpty(userName))
					{
						dbResult = "DB User is " + userName;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Tring to load Clients Count from SayHello");

			}
			return string.Format("Hello {0}. You are also {1}, My Identity Is {2}, DB Test {3}, Env Name: {4}", name, sName, Thread.CurrentPrincipal.Identity.Name, dbResult, Environment.UserName);
		}
		#endregion

		#region ProcessingImage
		public void ProcessingImage(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.ImageProcessing.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		}

		#endregion

		#region ImageNotAcceptable
		public void ImageNotAcceptable(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.WaitingForImage.ToString();
			string notes = "Image/s not acceptable - Waiting for New Image/s";

			MarkWorkflowState(ev, nextStateName, notes, true);
		}

		#endregion

		#region ImagesOK
		public void ImagesOK(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.ImageProcessed.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		}
		#endregion

		#region ContinueWithCurrentImage
		public void ContinueWithCurrentImage(OrderTrackingEventParameter ev, string imageFileList)
		{
			string nextStateName = WorkflowStates.ImageProcessed.ToString();
			string notes = "Image/s Processed - Images not ideal - Email sent - Job Progressing with available images";

			using (AbcDataContext ctx = new AbcDataContext())
			{
				var order = UpdateOrderStatusAndNotes(ev.OrderId, ev.LoggedBy, nextStateName, notes, ctx);

				// number 1 is to notify client
				ctx.Workflow_ROnlineBLeAlert(order.OrderID, null, null, 1, nextStateName, notes, ev.LoggedBy);

				//ROnlineBLe an Event to send email to Client
                int eventID = EventSettings.ImageNotAcceptable;
				string sub = "Image Files for Property: " + order.PropertyAddress + " - Order: " + order.OrderID;
				string xmlData = ConstructXMLData(imageFileList, ev.Message);
				string textData = ev.Message;
				int clientId = order.ClientID;
				string source = "OnlineBL_WorkflowService_ContinueWithCurrentImage";

				ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, ev.OrderId, clientId, null, null, source, String.Empty);

				ctx.SubmitChanges();
			}
		}
		#endregion

		#region ProcessingText
		public void ProcessingText(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.TextProcessing.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		} 
		#endregion

		#region NeedTextFromClient
		public void NeedTextFromClient(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.WaitingForText.ToString();
			string notes = "Waiting For More Text From Client";

			MarkWorkflowState(ev, nextStateName, notes, true);
		} 
		#endregion

		#region TextProcessingComplete
		public void TextProcessingComplete(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.TextProcessed.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		} 
		#endregion

		#region LayoutProcessing
		public void LayoutProcessing(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.LayoutProcessing.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		}		 
		#endregion

		#region LayoutRedesign
		public void LayoutRedesign(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.WaitingForLayoutChange.ToString();
			string notes = "Waiting for Template and Layout";

			MarkWorkflowState(ev, nextStateName, notes, true);
		} 
		#endregion

		#region LayoutDone
		public void LayoutDone(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.LayoutProcessed.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		} 
		#endregion

		#region WaitingForGraphic
		public void WaitingForGraphic(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.WaitingForGraphic.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		}
		#endregion

		#region WaitingForImage
		public void WaitingForImage(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.WaitingForImage.ToString();
			string notes = "Waiting For Image To Process Layout";

			MarkWorkflowState(ev, nextStateName, notes, true);
		}
		#endregion

		#region WaitingForText
		public void WaitingForText(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.WaitingForText.ToString();
			string notes = "Waiting For Text To Process Layout";

			MarkWorkflowState(ev, nextStateName, notes, true);
		}
		#endregion

		#region WaitingForTemplate
		public void WaitingForTemplate(OrderTrackingEventParameter ev)
		{
			string nextStateName = WorkflowStates.WaitingForTemplate.ToString();
			string notes = "";

			MarkWorkflowState(ev, nextStateName, notes, false);
		}
		#endregion

		#region Private Methods
		private Order UpdateOrderStatusAndNotes(int orderId, string loggedBy, string nextStateName, string notes, AbcDataContext ctx)
		{
			var order = (from o in ctx.Orders
						 where o.OrderID == orderId
						 select o).FirstOrDefault();

			bool isTransitionValid = StateMappingFactory.Current.GetStateMapping(0).CheckStateTransition(order.CurrentStatus, nextStateName);
			if (!isTransitionValid)
				throw new Exception(GetInvalidStateMessage(order.CurrentStatus, nextStateName, "Orders.OrderID", order.OrderID, -1));

			order.CurrentStatus = nextStateName;
			if (!string.IsNullOrEmpty(notes))
			{
				if (string.IsNullOrEmpty(order.Notes))
				{
					order.Notes = DateTime.Now.ToString("dd/M (hh:mmtt)") + " - " + notes;
				}
				else
				{
					order.Notes += Environment.NewLine + DateTime.Now.ToString("dd/M (hh:mmtt)") + " - " + notes;
				}
			}

			ctx.Workflow_LogHistory(order.OrderID, null, null, nextStateName, notes, loggedBy);
			return order;
		}
		private string ConstructXMLData(string imageFileList, string message)
		{
			string xmlData = @"<EVENT><ImageNames>" + "<![CDATA[" + imageFileList + "]]>" + "</ImageNames>" + "<Message>" + "<![CDATA[" + message + "]]>" + "</Message></EVENT>";
			return xmlData;
		}

		private void MarkWorkflowState(OrderTrackingEventParameter ev, string nextStateName, string notes, bool rOnlineBLeAlert)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				var order = UpdateOrderStatusAndNotes(ev.OrderId, ev.LoggedBy, nextStateName, notes, ctx);

				if (rOnlineBLeAlert)
				{
					// number 1 is to notify client
					ctx.Workflow_ROnlineBLeAlert(order.OrderID, null, null, 1, nextStateName, notes, ev.LoggedBy);
				}
				ctx.SubmitChanges();
			}
		}

		#region GetInvalidStateMessage
		private string GetInvalidStateMessage(string currentState, string nextStateName, string itemType, int itemId, int productTypeId)
		{
			string mess = string.Format("Invalid State Transition from '{0}' to '{1}'. ItemType:{2} ItemId:{3}, ProductTypeId:{4}",
				currentState, nextStateName, itemType, itemId, productTypeId);
			return mess;
		}
		#endregion 
		#endregion

		#region IWorkflowService Members

		public void NewDetails()
		{
			throw new NotImplementedException();
		}

		public void RequestNewDetails()
		{
			throw new NotImplementedException();
		}

		public void AllOK()
		{
			throw new NotImplementedException();
		}

		public void NewImageReceived()
		{
			throw new NotImplementedException();
		}

		public void RedGrayImageReceived()
		{
			throw new NotImplementedException();
		}

		public void GreenImageReceived()
		{
			throw new NotImplementedException();
		}

		public void ProcessingImage()
		{
			throw new NotImplementedException();
		}
		
		public void NewTextReceived()
		{
			throw new NotImplementedException();
		}

		public void NeedTextDetails()
		{
			throw new NotImplementedException();
		}

		public void TextProcessingNeeded()
		{
			throw new NotImplementedException();
		}

		public void ImageProcessingNeeded()
		{
			throw new NotImplementedException();
		}
		
		public void ProofReading()
		{
			throw new NotImplementedException();
		}

		public void QANotPassed()
		{
			throw new NotImplementedException();
		}

		public void QAPassed()
		{
			throw new NotImplementedException();
		}

		public void ProofSent()
		{
			throw new NotImplementedException();
		}

		public void ChangeRequestReceived_ApproveJob()
		{
			throw new NotImplementedException();
		}

		public void ChangeRequestReceived_Reproof()
		{
			throw new NotImplementedException();
		}

		public void PreviousApprovedJob()
		{
			throw new NotImplementedException();
		}

		public void JobApproved()
		{
			throw new NotImplementedException();
		}

		public void PrintComplete()
		{
			throw new NotImplementedException();
		}

		public void BoardInRunSheet()
		{
			throw new NotImplementedException();
		}

		public void BoardPickUpInVan()
		{
			throw new NotImplementedException();
		}

		public void RunsheetMarkedAsComplete()
		{
			throw new NotImplementedException();
		}

		public void RemovalRequested()
		{
			throw new NotImplementedException();
		}

		public void BoardInRunSheetForRemoval()
		{
			throw new NotImplementedException();
		}

		public void RunSheetMarkedComplete()
		{
			throw new NotImplementedException();
		}

		#endregion


	}
}
