using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.AOPCommands
{
	public class DeleteTemplateCommand : BaseCommand
	{
		public override object Execute()
		{
			AOP_Template t = ServiceFactory.AOPService.GetTemplateById(33, true);
			t.SetAsChangeTrackingRoot();
			t.SetAsDeleteOnSubmit(true);
			ServiceFactory.AOPService.UpdateTemplate(t);

			return "OK";
		}
	}
}
