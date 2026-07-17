using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.WorkflowCommands
{
	public class SayHelloCommand : BaseCommand
	{
		public override object Execute()
		{
			return ServiceFactory.WorkflowService.SayHello(Environment.UserName);
		}
	}
}
