using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using notes.Core.Data;
using notes.Core.Interfaces;
using notes.Core.Models;

namespace notes.Core.Services
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
			{
				return new List<Note>();
			}

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
				);

			if (next > 0)
			{
				_query = _query
					.Where(f => f.Id < next);
			}

			_query = _query
				.OrderByDescending(o => o.Id)
				.Take(limit);

			return await _query.ToListAsync();
		}
	}
}