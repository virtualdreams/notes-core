using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using notes.Core.Services;
using notes.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace notes.Controllers
{
    [Authorize]
    public class HomeController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly NoteService NoteService;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Settings;

		public HomeController(IMapper mapper, NoteService note, UserService user, IOptions<Settings> settings)
			: base(user)
		{
			Mapper = mapper;
			NoteService = note;
			UserService = user;
			Settings = settings;
		}

		[HttpGet]
		public IActionResult Index(ObjectId after)
		{
			var _pageSize = Settings.Value.PageSize;
			var _notes = NoteService.Get(UserId, after, false, _pageSize);
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
			var _pageSize = Settings.Value.PageSize;
			var _notes = NoteService.Search(UserId, q ?? String.Empty, after, _pageSize);
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

		public IActionResult Help()
		{
			return View();
		}
	}
}