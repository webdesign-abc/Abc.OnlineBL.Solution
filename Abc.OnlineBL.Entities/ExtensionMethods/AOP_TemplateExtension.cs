using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Abc.OnlineBL.Entities
{
	/// <summary>
	/// Partial Methods for AOP_template
	/// </summary>
	public partial class AOP_Template
	{
		// 500 templates in a folder.
		private const int folderSize = 500;

		/// <summary>
		/// Gets the name of the template folder. E.g. FolderName:'5001' for TemplateId: 5400
		/// </summary>
		/// <param name="templateId">The template id.</param>
		/// <returns></returns>
		public static string GetTemplateFolderName(int templateId)
		{
			double d = (double)templateId / (double)folderSize;
			double df = Math.Floor(d);
			if (templateId > folderSize)
			{
				if (d == df)
					df -= 1;

				df = df * folderSize + 1;
			}
			else
			{
				df = 1;
			}

			return df.ToString().PadLeft(5, '0');
		}

		/// <summary>
		/// Gets the name of the template folder. E.g. FolderName:'5001' for TemplateId: 5400
		/// </summary>
		/// <returns></returns>
		public string GetTemplateFolderName()
		{
			return AOP_Template.GetTemplateFolderName(this.TemplateId);
		}

		/// <summary>
		/// Gets the template path. E.g. TemplatePath:'Templates\5001\5400.indt' for TemplateId: 5400
		/// </summary>
		/// <param name="templateId">The template id.</param>
		/// <returns></returns>
		public static string GetTemplatePath(int templateId)
		{
			return "Templates\\" + GetTemplateFolderName(templateId) + "\\" + templateId + ".indt";
		}

		/// <summary>
		/// Gets the template path. E.g. TemplatePath:'Templates\5001\5400.indt' for TemplateId: 5400
		/// </summary>
		/// <returns></returns>
		public string GetTemplatePath()
		{
			return AOP_Template.GetTemplatePath(this.TemplateId);
		}
	}
}
