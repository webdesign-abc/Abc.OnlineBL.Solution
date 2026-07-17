using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.DataStore
{
	public static class DataContextExtention
	{
		public static void SetDataLoadOptions(this AbcDataContext ctx, List<EntityRelations> loadRelations)
		{
			if (loadRelations == null)
				return;
			if (loadRelations.Count == 0)
				return;

			DataLoadOptions opt = new DataLoadOptions();
			foreach (var relation in loadRelations)
			{
				string relationStr = Enum.GetName(typeof(EntityRelations), relation);
				int index = relationStr.IndexOf("_To_");
				string leftTable = relationStr.Substring(0, index);
				string rightTable = relationStr.Substring(index + 4);
				Type entityType = typeof(LINQEntityBase);
				string ns = entityType.Namespace;
				Type leftType = entityType.Assembly.GetType(ns + "." + leftTable);
				ParameterExpression parent = Expression.Parameter(leftType, "p");

				MemberInfo mi = leftType.GetMember(rightTable).First();
				MemberExpression me = Expression.MakeMemberAccess(parent, mi);
				ParameterExpression childs = Expression.Parameter(leftType, "p");
				var lambda = LambdaExpression.Lambda(me, childs);

				opt.LoadWith(lambda);
			}
			ctx.LoadOptions = opt;
		}
	}
}
