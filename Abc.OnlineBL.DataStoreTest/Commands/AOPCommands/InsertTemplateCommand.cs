using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.AOPCommands
{
	public class InsertTemplateCommand : BaseCommand
	{
		public override object Execute()
		{
			AOP_Template t = new AOP_Template();

			t.SetAsChangeTrackingRoot(EntityState.New);

			t.Description += "Tester Test at " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
			t.TemplatePath = @"C:\Test\Haha.indt";
			t.DateCreated = DateTime.Now;

			AOP_TemplateProduct product = new AOP_TemplateProduct()
			{
				PolicyId = 1,
				Type = "Billboard",
				Name = "A L - Adhesive & Laminate",
				SizeCode = "K Board 2400mm x 4800mm (8x16)",
				ContentType = "Photo Board",
				FrameType = "Frame",
				Format = "Portrait",
				MinimumMegaPixels = 1,
				RecommendedMegaPixels = 2,
				Description = "Ha Test at " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"),
				Active = true
			};

			t.AOP_TemplateProducts.Add(product);

			product = new AOP_TemplateProduct()
			{
				PolicyId = 1,
				Type = "Brochure",
				Name = "PC Colour Front",
				SizeCode = "P/Card (P Card) - 148mm x 104mm",
				ContentType = "Colour Front",
				Format = "Portrait",
				MinimumMegaPixels = 1,
				RecommendedMegaPixels = 2,
				Description = "Tester Test at " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"),
				Active = true
			};

			t.AOP_TemplateProducts.Add(product);

			t.SetAsInsertOnSubmit();

			ServiceFactory.AOPService.UpdateTemplate(t);

			return "OK";
		}
	}
}
