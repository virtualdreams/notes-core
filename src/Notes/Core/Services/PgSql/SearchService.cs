using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Core.Data;
using Notes.Core.Extensions;
using Notes.Core.Interfaces;
using Notes.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Notes.Core.Services.PgSql
{
	public class SearchService : ISearchService
	{
		private readonly ILogger<SearchService> Log;
		private readonly DataContext Context;

		public SearchService(ILogger<SearchService> log, DataContext context)
		{
			Log = log;
			Context = context;
		}

		/// <summary>
		/// Search for a term.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <param name="next">The next cursor.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<List<Note>> SearchAsync(string term, int next, int limit)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term))
				return new List<Note>();

			Log.LogDebug($"Search for note by term '{term}'.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(f =>
					f.Trash == false &&
					(
						EF.Functions.ToTsVector(f.Title).Matches(term) ||
						EF.Functions.ToTsVector(f.Content).Matches(term) ||
						EF.Functions.ToTsVector(f.Notebook).Matches(term) ||
						f.Tags.Any(a => EF.Functions.ToTsVector(a.Name).Matches(term))
					)
				)
				.WhereIf(next > 0, f => f.Id < next)
				.OrderByDescending(o => o.Id)
				.Take(limit);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get suggestions for notebooks.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <returns></returns>
		public async Task<List<string>> NotebookSuggestionsAsync(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return new List<string>();

			Log.LogDebug($"Request suggestions for notebooks by term '{term}'.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(
					f => f.Trash == false &&
					EF.Functions.ILike(f.Notebook, $"%{term}%")
				)
				.Select(s => s.Notebook)
				.Distinct()
				.OrderBy(o => o);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get suggestions for tags.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <returns></returns>
		public async Task<List<string>> TagSuggestionsAsync(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return new List<string>();

			Log.LogDebug($"Request suggestions for tags by term '{term}'.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(f => f.Trash == false)
				.SelectMany(s => s.Tags
					.Where(f => EF.Functions.ILike(f.Name, $"%{term}%"))
					.Select(s => s.Name)
				)
				.Distinct()
				.OrderBy(o => o);

			return await _query.ToListAsync();
		}
	}
}