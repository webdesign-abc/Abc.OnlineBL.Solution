using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.ClientCommands
{
	public class UpdateClientsPrefCommand : BaseCommand
	{
		public override object Execute()
		{
			ClientsPref cp = ServiceFactory.ClientService.GetClientsPref(3728, 6);

			cp.SetAsChangeTrackingRoot();
			cp.BitValue = true;
			cp.SetAsUpdateOnSubmit();
			ServiceFactory.ClientService.UpdateClientsPref(cp);
			return "OK";
		}
	}
}
