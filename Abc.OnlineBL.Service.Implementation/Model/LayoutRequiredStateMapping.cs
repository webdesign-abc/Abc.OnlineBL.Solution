using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service.Implementation.Model
{
	public abstract class LayoutRequiredStateMapping : StateMapping
	{
		public LayoutRequiredStateMapping() : base()
		{
			AddStateTransition(WorkflowStates.OrderInProgress, WorkflowStates.TextProcessing, WorkflowStates.LayoutProcessing, WorkflowStates.ImageExtracting);
			//AddStateTransition(WorkflowStates.WaitingForImage, WorkflowStates.ImageExtracting);
			AddStateTransition(WorkflowStates.ImageExtracting, WorkflowStates.WaitingForImageToBeProcessed, WorkflowStates.ImageProcessed);
			//AddStateTransition(WorkflowStates.WaitingForText, WorkflowStates.TextProcessing);
			AddStateTransition(WorkflowStates.ImageProcessing, WorkflowStates.ImageProcessed, WorkflowStates.TextProcessing);
			AddStateTransition(WorkflowStates.WaitingForImageToBeProcessed, WorkflowStates.ImageProcessing);
			AddStateTransition(WorkflowStates.TextProcessing, WorkflowStates.ImageProcessing, WorkflowStates.TextProcessed);
			AddStateTransition(WorkflowStates.WaitingForTextToBeProcessed, WorkflowStates.TextProcessing);
			AddStateTransition(WorkflowStates.ImageProcessed, WorkflowStates.LayoutProcessing, WorkflowStates.TextProcessing);
			AddStateTransition(WorkflowStates.TextProcessed, WorkflowStates.LayoutProcessing);
			AddStateTransition(WorkflowStates.TemplateDesign, WorkflowStates.LayoutProcessing);
			AddStateTransition(WorkflowStates.GraphicFloorplanProcessing, WorkflowStates.LayoutProcessing);
			AddStateTransition(WorkflowStates.LayoutProcessing, WorkflowStates.WaitingForTextToBeProcessed, WorkflowStates.WaitingForImageToBeProcessed, WorkflowStates.WaitingForText, WorkflowStates.GraphicFloorplanProcessing, WorkflowStates.TemplateDesign, WorkflowStates.LayoutProcessed);
			AddStateTransition(WorkflowStates.WaitingForLayoutChange, WorkflowStates.LayoutProcessing);
			AddStateTransition(WorkflowStates.LayoutProcessed, WorkflowStates.ProofReading);
			AddStateTransition(WorkflowStates.ProofReading, WorkflowStates.WaitingForLayoutChange, WorkflowStates.ProofReadingComplete);
			AddStateTransition(WorkflowStates.ProofReadingComplete, WorkflowStates.WaitingForApproval, WorkflowStates.Approved);
			AddStateTransition(WorkflowStates.WaitingForApproval, WorkflowStates.WaitingForLayoutChange, WorkflowStates.Approved);
			AddStateTransition(WorkflowStates.Approved, null);
		}
	}
}
