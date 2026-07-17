using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Abc.OnlineBL.Service.Implementation.BusinessLogic
{
	public static class Extension
	{
		/// <summary> 
		/// Method that provides the T-SQL EXISTS call for any IQueryable (thus extending Linq). 
		/// </summary> 
		/// <remarks>Returns whether or not the predicate conditions exists at least one time.</remarks> 
		public static bool Exists<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return source.Where(predicate).Any();
		}

		/// <summary> 
		/// Method that provides the T-SQL EXISTS call for any IQueryable (thus extending Linq). 
		/// </summary> 
		/// <remarks>Returns whether or not the predicate conditions exists at least one time.</remarks> 
		public static bool Exists<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
		{
			return source.Where(predicate).Any();
		} 
	}
}
