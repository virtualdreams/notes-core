using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using notes.Core.Services;
using notes.Models;

namespace notes.Controllers
{
	[Authorize]
	public class HomeController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly Settings Options;
		private readonly UserService UserService;
		private readonly NoteService NoteService;

		public HomeController(IMapper mapper, IOptionsSnapshot<Settings> settings, UserService user, NoteService note)
			: base(user)
		{
			Mapper = mapper;
			Options = settings.Value;
			UserService = user;
			NoteService = note;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int after)
		{
			var _notes = await NoteService.GetNotesAsync(after, false, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? 0, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteListContainer
			{
				Notes = notes,
				Pager = _pager
			};

			return View(view);
		}

		[HttpGet]
		public async Task<IActionResult> Search(string q, int after)
		{
			var _notes = await NoteService.SearchAsync(q ?? String.Empty, after, PageSize);
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

		[AllowAnonymous]
		public IActionResult Error(int? code)
		{
			switch (code ?? 0)
			{
				case 400:
					return View("BadRequest");

				case 404:
					return View("PageNotFound");

				default:
					return View();
			}
		}
	}
}