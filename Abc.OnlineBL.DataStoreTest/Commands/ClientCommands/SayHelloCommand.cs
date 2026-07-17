using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.ClientCommands
{
	public class SayHelloCommand : BaseCommand
	{
		public override object Execute()
		{
			return ServiceFactory.ClientService.SayHello("ABC");
		}
	}
}
