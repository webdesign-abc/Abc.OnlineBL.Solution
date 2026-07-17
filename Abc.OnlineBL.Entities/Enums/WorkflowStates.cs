using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Enums
{
	[DataContract]
	[Serializable]
	public enum WorkflowStates
	{
		[EnumMember]
		Approved,
		[EnumMember]
		BoardErected,
		[EnumMember]
		BoardInConstruction,
		[EnumMember]
		BoardInRunsheet,
		[EnumMember]
		BoardInTransit,
		[EnumMember]
		BoardRemoved,
		[EnumMember]
		Completed,
		[EnumMember]
		GraphicFloorplanProcessing,
		[EnumMember]
		ImageExtracting,
		[EnumMember]
		ImageProcessed,
		[EnumMember]
		ImageProcessing,
		[EnumMember]
		LayoutProcessed,
		[EnumMember]
		LayoutProcessing,
		[EnumMember]
		OrderCompleted,
		[EnumMember]
		Despatched,
		[EnumMember]
		OrderInProgress,
		//[EnumMember]
		//OrderProcessed,
		[EnumMember]
		OrderClosed,
		[EnumMember]
		OrderProcessing,
		[EnumMember]
		OrderReceived,
		[EnumMember]
		ProofReading,
		[EnumMember]
		ProofReadingComplete,
		[EnumMember]
		RemovalRequested,
		[EnumMember]
		ScheduledForRemoval,
		[EnumMember]
		TemplateDesign,
		[EnumMember]
		TextProcessed,
		[EnumMember]
		TextProcessing,
		[EnumMember]
		WaitingForApproval,
		[EnumMember]
		WaitingForClientsInput,
		[EnumMember]
		WaitingForImage,
		[EnumMember]
		WaitingForImageToBeProcessed,
		[EnumMember]
		WaitingForLayoutChange,
		[EnumMember]
		WaitingForText,
		[EnumMember]
		WaitingForTextToBeProcessed,
		[EnumMember]
		WaitingForGraphic,
		[EnumMember]
		WaitingForTemplate,
		[EnumMember]
		TemplateProcessing
	}
}
