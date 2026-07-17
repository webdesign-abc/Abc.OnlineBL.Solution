using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service.Implementation.Model
{
	public class OrderStateMapping : StateMapping
	{
		public OrderStateMapping() : base()
		{
			AddStateTransition(WorkflowStates.OrderReceived, WorkflowStates.OrderProcessing);
			AddStateTransition(WorkflowStates.OrderProcessing, WorkflowStates.OrderInProgress, WorkflowStates.WaitingForClientsInput);
			AddStateTransition(WorkflowStates.WaitingForClientsInput, WorkflowStates.OrderProcessing);
			AddStateTransition(WorkflowStates.OrderInProgress, WorkflowStates.Approved);
			AddStateTransition(WorkflowStates.Approved, WorkflowStates.Despatched);
			AddStateTransition(WorkflowStates.Despatched, WorkflowStates.OrderCompleted);
			AddStateTransition(WorkflowStates.OrderCompleted, WorkflowStates.OrderClosed);
		}
	}
}
