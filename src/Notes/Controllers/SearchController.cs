using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notes.Core.Interfaces;
using Notes.Models;
using Notes.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Notes.Controllers
{
	[Authorize]
	public class SearchController : BaseController
	{
		private readonly ILogger<SearchController> Log;

		private readonly IMapper Mapper;

		private readonly AppSettings AppSettings;

		private readonly IUserService UserService;

		private readonly ISearchService SearchService;

		public SearchController(
			ILogger<SearchController> log,
			IMapper mapper,
			IOptionsSnapshot<AppSettings> settings,
			IUserService user,
			ISearchService search)
			: base(user)
		{
			Log = log;
			Mapper = mapper;
			AppSettings = settings.Value;
			UserService = user;
			SearchService = search;
		}

		[HttpGet]
		public async Task<IActionResult> Search(string q, int after)
		{
			var _notes = await SearchService.SearchAsync(q ?? String.Empty, after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? 0, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteSearchContainer
			{
				Notes = notes,
				Pager = _pager,
				Term = q?.Trim()
			};

			return View(view);
		}

		[HttpGet("search/tags")]
		public async Task<IActionResult> TagSuggestions(string term)
		{
			return Json(await SearchService.TagSuggestionsAsync(term));
		}

		[HttpGet("search/notebook")]
		public async Task<IActionResult> NotebookSuggestions(string term)
		{
			return Json(await SearchService.NotebookSuggestionsAsync(term));
		}
	}
}