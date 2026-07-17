using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Orders.Workflow;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Web;

namespace Abc.OnlineBL.Service.Implementation
{
	public class WFRuntimeHelper
	{
		private OnlinePropertyOrder propertyOrder;

		#region Constructor
		public WFRuntimeHelper(OnlinePropertyOrder propertyOrder)
		{
			this.propertyOrder = propertyOrder;
		}
		#endregion

		#region ExecuteWorkflow
		public OrderDataExchange ExecuteWorkflow()
		{
			//TODO: Confirm the initialize of workflow runtime
			WorkflowRuntime workflowRuntime = new WorkflowRuntime();

			workflowRuntime.WorkflowTerminated += delegate(object wsender, WorkflowTerminatedEventArgs we)
			{
				if (we.Exception != null)
					Abc.Util2.Log.Logger.LogException(we.Exception);
			};

			ManualWorkflowSchedulerService manualService = new ManualWorkflowSchedulerService();
			workflowRuntime.AddService(manualService);

			workflowRuntime.StartRuntime();
			
			ManualWorkflowSchedulerService manualScheduler =
					workflowRuntime.GetService(typeof(ManualWorkflowSchedulerService)) as ManualWorkflowSchedulerService;

			OrderDataExchange orderDataExchange = new OrderDataExchange();
			orderDataExchange.PropertyOrder = propertyOrder;

			orderDataExchange.OrderDir = OnlineBLConfig.ORDER_FILE_DIR;

			Dictionary<string, object> parameters = new Dictionary<string, object>();

			parameters.Add("OrderDataExchange", orderDataExchange);

			WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof(OnlineBLProcessOrder), parameters);
			instance.Start();
			manualScheduler.RunWorkflow(instance.InstanceId);

			if (workflowRuntime != null)
				workflowRuntime.StopRuntime();

			return orderDataExchange;
		}
		#endregion

		#region StartWorflowRuntime
		public static WorkflowRuntime StartWorflowRuntime()
		{
			WorkflowRuntime workflowRuntime = new WorkflowRuntime();

			workflowRuntime.WorkflowTerminated += delegate(object wsender, WorkflowTerminatedEventArgs we)
			{
				if (we.Exception != null)
					Abc.Util2.Log.Logger.LogException(we.Exception);
			};

			ManualWorkflowSchedulerService manualService = new ManualWorkflowSchedulerService();
			workflowRuntime.AddService(manualService);

			workflowRuntime.StartRuntime();

			HttpContext.Current.Application["WorkflowRuntime"] = workflowRuntime;
			return workflowRuntime;
		}
		#endregion

		#region StopWorflowRuntime
		public static void StopWorflowRuntime()
		{
			WorkflowRuntime workflowRuntime = HttpContext.Current.Application["WorkflowRuntime"] as WorkflowRuntime;
			if (workflowRuntime != null)
				workflowRuntime.StopRuntime();
		}
		#endregion

		#region GetWorkflowRuntime
		public static WorkflowRuntime GetWorkflowRuntime()
		{
			WorkflowRuntime workflowRuntime = null;

			if (HttpContext.Current != null && HttpContext.Current.Application["WorkflowRuntime"] != null)
				workflowRuntime = HttpContext.Current.Application["WorkflowRuntime"] as WorkflowRuntime;

			if (workflowRuntime == null)
				workflowRuntime = StartWorflowRuntime();

			return workflowRuntime;
		}
		#endregion
	}
}
