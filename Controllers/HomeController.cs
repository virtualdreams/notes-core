using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
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

		private int PageSize => UserSettings?.PageSize ?? Options.PageSize;

		public HomeController(IMapper mapper, Settings settings, UserService user, NoteService note)
			: base(user)
		{
			Mapper = mapper;
			Options = settings;
			UserService = user;
			NoteService = note;
		}

		[HttpGet]
		public IActionResult Index(ObjectId after)
		{
			var _notes = NoteService.GetNotes(UserId, after, false, PageSize);
			var _pager = new Pager(_notes.Item1.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Item2);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes.Item1);

			var view = new NoteListContainer
			{
				Notes = notes,
				Pager = _pager
			};

			return View(view);
		}

		[HttpGet]
		public IActionResult Search(string q, ObjectId after)
		{
			var _notes = NoteService.Search(UserId, q ?? String.Empty, after, PageSize);
			var _pager = new Pager(_notes.Item1.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Item2);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes.Item1);

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