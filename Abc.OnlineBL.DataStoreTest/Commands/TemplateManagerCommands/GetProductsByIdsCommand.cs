using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.TemplateManagerCommands
{
	public class GetProductsByIdsCommand : BaseCommand
	{
		public override object Execute()
		{
			int[] myIds = new int[] { 2, 7, 12, 13 };
			List<int> ids = new List<int>();
			ids.AddRange(myIds);
			DataTable dt = ServiceFactory.TemplateManagerService.GetProductsByIds(ids);
			return dt.Rows.Count;
		}
	}
}
