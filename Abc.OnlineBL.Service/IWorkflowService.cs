using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Enums;
using System.IO;

namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// IWorkflowService interface
	/// </summary>
	[ServiceContract]
	public interface IWorkflowService
	{
		/// <summary>
		/// SayHello returns what you pass  to it just to let you know that it is listening to you.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[OperationContract]
		string SayHello(string name);

		/// <summary>
		/// Images the not acceptable.
		/// </summary>
		/// <param name="ev">The ev.</param>
		[OperationContract]
		void ImageNotAcceptable(OrderTrackingEventParameter ev);

		/// <summary>
		/// News the details.
		/// </summary>
		[OperationContract]
		void NewDetails();

		/// <summary>
		/// Requests the new details.
		/// </summary>
		[OperationContract]
		void RequestNewDetails();

		/// <summary>
		/// Alls the OK.
		/// </summary>
		[OperationContract]
		void AllOK();

		/// <summary>
		/// News the image received.
		/// </summary>
		[OperationContract]
		void NewImageReceived();

		/// <summary>
		/// Reds the gray image received.
		/// </summary>
		[OperationContract]
		void RedGrayImageReceived();

		/// <summary>
		/// Greens the image received.
		/// </summary>
		[OperationContract]
		void GreenImageReceived();

		/// <summary>
		/// Processings the image.
		/// </summary>
		/// <param name="ev">The ev.</param>
		[OperationContract]
		void ProcessingImage(OrderTrackingEventParameter ev);

		/// <summary>
		/// Imageses the OK.
		/// </summary>
		/// <param name="ev">The ev.</param>
		[OperationContract]
		void ImagesOK(OrderTrackingEventParameter ev);

		/// <summary>
		/// Continues the with current image.
		/// </summary>
		/// <param name="ev">The ev.</param>
		/// <param name="imageFileList">The image file list.</param>
		[OperationContract]
		void ContinueWithCurrentImage(OrderTrackingEventParameter ev, string imageFileList);

		/// <summary>
		/// Processings the text.
		/// </summary>
		[OperationContract]
		void ProcessingText(OrderTrackingEventParameter ev);

		/// <summary>
		/// News the text received.
		/// </summary>
		[OperationContract]
		void NewTextReceived();

		/// <summary>
		/// Needs the text details.
		/// </summary>
		[OperationContract]
		void NeedTextDetails();

		/// <summary>
		/// Texts the processing needed.
		/// </summary>
		[OperationContract]
		void TextProcessingNeeded();

		/// <summary>
		/// Images the processing needed.
		/// </summary>
		[OperationContract]
		void ImageProcessingNeeded();

		/// <summary>
		/// Layouts the job.
		/// </summary>
		[OperationContract]
		void LayoutProcessing(OrderTrackingEventParameter ev);

		/// <summary>
		/// Needs the text from client.
		/// </summary>
		[OperationContract]
		void NeedTextFromClient(OrderTrackingEventParameter ev);

		/// <summary>
		/// Texts the processing complete.
		/// </summary>
		[OperationContract]
		void TextProcessingComplete(OrderTrackingEventParameter ev);

		/// <summary>
		/// Layouts the redesign.
		/// </summary>
		[OperationContract]
		void LayoutRedesign(OrderTrackingEventParameter ev);

		/// <summary>
		/// Layouts the done.
		/// </summary>
		[OperationContract]
		void LayoutDone(OrderTrackingEventParameter ev);

		/// <summary>
		/// Waitings for graphic.
		/// </summary>
		/// <param name="ev">The ev.</param>
		[OperationContract]
		void WaitingForGraphic(OrderTrackingEventParameter ev);

		/// <summary>
		/// Waitings for image.
		/// </summary>
		/// <param name="ev">The ev.</param>
		[OperationContract]
		void WaitingForImage(OrderTrackingEventParameter ev);

		/// <summary>
		/// Waitings for text.
		/// </summary>
		/// <param name="ev">The ev.</param>
		[OperationContract]
		void WaitingForText(OrderTrackingEventParameter ev);

		/// <summary>
		/// Waitings for template.
		/// </summary>
		/// <param name="ev">The ev.</param>
		[OperationContract]
		void WaitingForTemplate(OrderTrackingEventParameter ev);

		/// <summary>
		/// Proofs the reading.
		/// </summary>
		[OperationContract]
		void ProofReading();

		/// <summary>
		/// QAs the not passed.
		/// </summary>
		[OperationContract]
		void QANotPassed();

		/// <summary>
		/// QAs the passed.
		/// </summary>
		[OperationContract]
		void QAPassed();

		/// <summary>
		/// Proofs the sent.
		/// </summary>
		[OperationContract]
		void ProofSent();

		/// <summary>
		/// Changes the request received_ approve job.
		/// </summary>
		[OperationContract]
		void ChangeRequestReceived_ApproveJob();

		/// <summary>
		/// Changes the request received_ reproof.
		/// </summary>
		[OperationContract]
		void ChangeRequestReceived_Reproof();

		/// <summary>
		/// Previouses the approved job.
		/// </summary>
		[OperationContract]
		void PreviousApprovedJob();

		/// <summary>
		/// Jobs the approved.
		/// </summary>
		[OperationContract]
		void JobApproved();

		/// <summary>
		/// Prints the complete.
		/// </summary>
		[OperationContract]
		void PrintComplete();

		/// <summary>
		/// Boards the in run sheet.
		/// </summary>
		[OperationContract]
		void BoardInRunSheet();

		/// <summary>
		/// Boards the pick up in van.
		/// </summary>
		[OperationContract]
		void BoardPickUpInVan();

		/// <summary>
		/// Runsheets the marked as complete.
		/// </summary>
		[OperationContract]
		void RunsheetMarkedAsComplete();

		/// <summary>
		/// Removals the requested.
		/// </summary>
		[OperationContract]
		void RemovalRequested();

		/// <summary>
		/// Boards the in run sheet for removal.
		/// </summary>
		[OperationContract]
		void BoardInRunSheetForRemoval();

		/// <summary>
		/// Runs the sheet marked complete.
		/// </summary>
		[OperationContract]
		void RunSheetMarkedComplete();

	}
}
