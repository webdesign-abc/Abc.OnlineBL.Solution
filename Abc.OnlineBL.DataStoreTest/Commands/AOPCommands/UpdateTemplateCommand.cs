using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.AOPCommands
{
	public class UpdateTemplateCommand : BaseCommand
	{
		public override object Execute()
		{
			List<EntityRelations> loadOptions = new List<EntityRelations>();
			loadOptions.Add(EntityRelations.AOP_Template_To_AOP_TemplateProducts);

			AOP_Template t = ServiceFactory.AOPService.GetTemplateById(33, true, loadOptions);

			t.SetAsChangeTrackingRoot();
			t.SetAsUpdateOnSubmit();

			t.Description = " THIS IS WORKING";
			t.DateModified = DateTime.Now;
			foreach (AOP_TemplateProduct tp in t.AOP_TemplateProducts)
			{
				tp.Description = " THIS IS WORKING";
			}

			ServiceFactory.AOPService.UpdateTemplate(t);

			return "OK";
		}
	}
}
