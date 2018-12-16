using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		public HomeController(IMapper mapper, Settings settings, UserService user, NoteService note)
			: base(user)
		{
			Mapper = mapper;
			Options = settings;
			UserService = user;
			NoteService = note;
		}

		[HttpGet]
		public async Task<IActionResult> Index(ObjectId after)
		{
			var _notes = await NoteService.GetNotes(after, false, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteListContainer
			{
				Notes = notes,
				Pager = _pager
			};

			return View(view);
		}

		[HttpGet]
		public async Task<IActionResult> Search(string q, ObjectId after)
		{
			var _notes = await NoteService.Search(q ?? String.Empty, after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Count() >= PageSize);

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