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

namespace Notes.Core.Services.MySql
{
	public class SearchService : ISearchService
	{
		private readonly ILogger<SearchService> Log;

		private readonly DatabaseContext Context;

		public SearchService(
			ILogger<SearchService> log,
			DatabaseContext context)
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
		public async Task<IList<Note>> SearchAsync(string term, int next, int limit)
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
						EF.Functions.Match(f.Title, term, MySqlMatchSearchMode.NaturalLanguage) ||
						EF.Functions.Match(f.Content, term, MySqlMatchSearchMode.NaturalLanguage) ||
						EF.Functions.Match(f.Notebook, term, MySqlMatchSearchMode.NaturalLanguage) ||
						f.Tags.Any(a => EF.Functions.Match(a.Name, term, MySqlMatchSearchMode.NaturalLanguage))
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
		public async Task<IList<string>> NotebookSuggestionsAsync(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return new List<string>();

			Log.LogDebug($"Request suggestions for notebooks by term '{term}'.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(
					f => f.Trash == false &&
					EF.Functions.Like(f.Notebook, $"%{term}%")
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
		public async Task<IList<string>> TagSuggestionsAsync(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return new List<string>();

			Log.LogDebug($"Request suggestions for tags by term '{term}'.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(f => f.Trash == false)
				.SelectMany(s => s.Tags
					.Where(f => EF.Functions.Like(f.Name, $"%{term}%"))
					.Select(s => s.Name)
				)
				.Distinct()
				.OrderBy(o => o);

			return await _query.ToListAsync();
		}
	}
}