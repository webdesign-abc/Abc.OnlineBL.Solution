using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.ServiceProxy;
using System.Data;

namespace Abc.OnlineBL.DataStoreTest.Commands.TemplateManagerCommands
{
	public class GetClientsWhoCanOrderProductCommand : BaseCommand
	{
		public override object Execute()
		{
			//DataTable dt = srv.GetClients();
			DataTable dt = ServiceFactory.TemplateManagerService.GetClientsWhoCanOrderProduct("Bells", 1087);
			return dt.Rows.Count;
		}
	}
}
