using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service.Implementation.Model
{
	public class BrochureStateMapping : LayoutRequiredStateMapping
	{
		public BrochureStateMapping() : base()
		{
			AddStateTransition(WorkflowStates.Approved, WorkflowStates.Despatched);
			AddStateTransition(WorkflowStates.Approved, WorkflowStates.Completed);
		}
	}
}
