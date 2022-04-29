using System.Linq.Expressions;
using System.Linq;
using System;

namespace Notes.Core.Extensions
{
	public static class IQueryableExtensions
	{
		public static IQueryable<T> WhereIf<T>(
			this IQueryable<T> source,
			Func<bool> condition,
			Expression<Func<T, bool>> predicate)
		{
			return source.WhereIf(condition(), predicate);
		}

		public static IQueryable<T> WhereIf<T>(
			this IQueryable<T> source,
			bool condition,
			Expression<Func<T, bool>> predicate)
		{
			return condition ? source.Where(predicate) : source;
		}

		public static IQueryable<T> WhereIfElse<T>(
			this IQueryable<T> source,
			Func<bool> condition,
			Expression<Func<T, bool>> left_predicate,
			Expression<Func<T, bool>> right_predicate
		)
		{
			return source.WhereIfElse(condition(), left_predicate, right_predicate);
		}

		public static IQueryable<T> WhereIfElse<T>(
			this IQueryable<T> source,
			bool condition,
			Expression<Func<T, bool>> left_predicate,
			Expression<Func<T, bool>> right_predicate
		)
		{
			return condition ? source.Where(left_predicate) : source.Where(right_predicate);
		}
	}
}