using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.AOPCommands
{
	public class GetTemplatesByPathsCommand : BaseCommand
	{
		public override object Execute()
		{
			List<string> paths = new List<string>();
			paths.Add(@"\Bells\Sunshine_3728\Board.indt");
			paths.Add(@"\Bells\Sunshine_3728\My Brochure.indt");
			paths.Add(@"\Bells\Sunshine_3728\P Card TEST PLEASE DONT USE.indt");
			
			List<EntityRelations> loadOptions = new List<EntityRelations>();
			loadOptions.Add(EntityRelations.AOP_Template_To_AOP_TemplateProducts);
           
			List<AOP_Template> templates = ServiceFactory.AOPService.GetTemplatesByPaths(paths, true, loadOptions);
			return templates.Count.ToString();
		}
	}
}
