using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.ProductCommands
{
	public class GetProductByIdCommand : BaseCommand
	{
		public override object Execute()
		{
			var p = ServiceFactory.ProductService.GetOnlineProductsByCategoryId(3728, 3, null, true);
			return string.Format("Count:{0}", p.Count);
		}
	}
}
