using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Abc.OnlineBL.Entities
{
	public class DataLoadOptionsUtility
	{
		public static string GetOptionsInString(List<EntityRelations> dataLoadOptions)
		{
			if (dataLoadOptions == null)
				return String.Empty;

			List<string> options = new List<string>();
			foreach (var relation in dataLoadOptions)
			{
				string relationStr = Enum.GetName(typeof(EntityRelations), relation);
				options.Add(relationStr);
			}

			if (options.Count > 0)
				return string.Join(", ", options.ToArray());
			else
				return string.Empty;
		}
	}
}
