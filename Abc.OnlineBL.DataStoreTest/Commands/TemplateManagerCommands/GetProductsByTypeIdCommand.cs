using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using System.Data;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.TemplateManagerCommands
{
	public class GetProductsByTypeIdCommand : BaseCommand
	{
		public override object Execute()
		{
			DataTable products = ServiceFactory.TemplateManagerService.GetProductsByTypeId(18);
			return products.Rows.Count;
		}
	}
}
