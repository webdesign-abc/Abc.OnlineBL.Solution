using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.AOPCommands
{
	public class GetTemplateProductsMatchingPriceListCommand : BaseCommand
	{
		public override object Execute()
		{
			int clientId = 3728;
			string type = "Billboard";

			List<AOP_TemplateProduct> tps = ServiceFactory.AOPService.GetTemplateProductsMatchingPriceList(clientId, type);
			string mess = string.Empty;
			tps.ForEach(delegate(AOP_TemplateProduct tp) { mess += string.Format("{0}, {1}, {2}, {3}\r\n", tp.SizeCode, tp.ContentType, tp.FrameType, tp.Format); });
			return mess;
		}
	}
}
