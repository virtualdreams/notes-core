using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using notes.Core.Services;
using notes.Helper;
using notes.Models;
using Microsoft.Extensions.Options;

namespace notes.Controllers
{
	[Authorize]
    public class HomeController: Controller
	{
		private readonly NoteService NoteService;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Settings;

		public HomeController(NoteService note, UserService user, IOptions<Settings> settings)
		{
			NoteService = note;
			UserService = user;
			Settings = settings;
		}

		[HttpGet]
		public IActionResult Index(int? ofs)
		{
			var _pageSize = Settings.Value.PageSize;
			var _user = UserService.GetByName(User.GetUserName());
			var _count = NoteService.Get(_user.Id, false, null, null).Count();
			var _notes = NoteService.Get(_user.Id, false, ofs ?? 0, _pageSize);
			var _pager = new PageOffset(ofs ?? 0, _pageSize, _count);
			
			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);
			
			var view = new NoteListContainer
			{
				Notes = notes,
				Offset = _pager
			};

			return View(view);
		}

		[HttpGet]
		public IActionResult Search(string q, int? ofs)
		{
			var _pageSize = Settings.Value.PageSize;
			var _user = UserService.GetByName(User.GetUserName());
			var _count = NoteService.Search(_user.Id, q ?? String.Empty, false, null, null).Count();
			var _notes = NoteService.Search(_user.Id, q ?? String.Empty, false, ofs, _pageSize);
			var _pager = new PageOffset(ofs ?? 0, _pageSize, _count);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);
			
			var view = new NoteSearchContainer
			{
				Notes = notes,
				Offset = _pager,
				Term = q?.Trim()
			};

			return View(view);
		}
	}
}