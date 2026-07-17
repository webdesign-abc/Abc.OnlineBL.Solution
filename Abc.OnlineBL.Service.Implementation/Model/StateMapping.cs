using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service.Implementation.Model
{
	public abstract class StateMapping
	{
		#region Constructor
		public StateMapping()
		{
			States = new Dictionary<WorkflowStates, List<WorkflowStates>>();
		}
		#endregion

		#region Public Properties
		public Dictionary<WorkflowStates, List<WorkflowStates>> States { get; set; }
		#endregion

		#region GetNextState
		public List<WorkflowStates> GetNextState(WorkflowStates currentState)
		{
			if (!States.ContainsKey(currentState))
				throw new Exception(string.Format("CurrentState:{0} does not exist in the map", currentState));

			return States[currentState];
		}
		#endregion

		#region CheckStateTransition
		public bool CheckStateTransition(string currentState, string intendedNextState)
		{
			// Bypass any check.
			return true;

			//if (intendedNextState == WorkflowStates.OrderReceived.ToString() ||
			//   intendedNextState == WorkflowStates.OrderProcessing.ToString())
			//{
			//   return true;
			//}

			//WorkflowStates myCurrentState = (WorkflowStates)Enum.Parse(typeof(WorkflowStates), currentState, true);
			//WorkflowStates myNextState = (WorkflowStates)Enum.Parse(typeof(WorkflowStates), intendedNextState, true);
			//return CheckStateTransition(myCurrentState, myNextState);
		}

		public bool CheckStateTransition(WorkflowStates currentState, WorkflowStates intendedNextState)
		{
			// Bypass any check.
			return true;

			//if (intendedNextState == WorkflowStates.OrderReceived ||
			//   intendedNextState == WorkflowStates.OrderProcessing)
			//{
			//   return true;
			//}

			//if (!States.ContainsKey(currentState))
			//   throw new Exception(string.Format("CurrentState:{0} does not exist in the map", currentState));

			//List<WorkflowStates> states = States[currentState];
			//if (states.Contains(intendedNextState))
			//{
			//   return true;
			//}
			//else
			//{
			//   return false;
			//}
		}
		#endregion

		#region AddStateTransition
		public void AddStateTransition(WorkflowStates state, params WorkflowStates[] outgoingStates)
		{
			if (States.ContainsKey(state))
			{
				if (outgoingStates != null)
					States[state].AddRange(outgoingStates);
			}
			else
			{
				States.Add(state, new List<WorkflowStates>());

				if (outgoingStates != null)
					States[state].AddRange(outgoingStates);
			}
		}
		#endregion
	}
}