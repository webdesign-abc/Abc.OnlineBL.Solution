using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service.Implementation.Model
{
	public class BoardStateMapping : LayoutRequiredStateMapping
	{
		public BoardStateMapping() : base()
		{
			AddStateTransition(WorkflowStates.Approved, WorkflowStates.BoardInConstruction);
			// Despatched is same as BoardInConstruction
			AddStateTransition(WorkflowStates.Approved, WorkflowStates.Despatched);
			AddStateTransition(WorkflowStates.BoardInConstruction, WorkflowStates.BoardInRunsheet);
			AddStateTransition(WorkflowStates.BoardInRunsheet, WorkflowStates.BoardInTransit);
			AddStateTransition(WorkflowStates.RemovalRequested, WorkflowStates.ScheduledForRemoval);
			AddStateTransition(WorkflowStates.BoardErected, WorkflowStates.RemovalRequested);
			AddStateTransition(WorkflowStates.ScheduledForRemoval, WorkflowStates.BoardRemoved);
			AddStateTransition(WorkflowStates.BoardInTransit, WorkflowStates.BoardErected);
			AddStateTransition(WorkflowStates.BoardRemoved, WorkflowStates.Completed);
		}
	}
}
