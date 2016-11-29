using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using notes.Core.Services;
using notes.Helper;
using notes.Models;

namespace notes.Controllers
{
	[Authorize]
    public class HomeController: Controller
	{
		private readonly NoteService NoteService;
		private readonly UserService UserService;

		public HomeController(NoteService note, UserService user)
		{
			NoteService = note;
			UserService = user;
		}

		[HttpGet]
		public IActionResult Index(int? ofs)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _count = NoteService.Get(_user.Id, false, null, null).Count();
			var _notes = NoteService.Get(_user.Id, false, ofs ?? 0, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, _count);
			
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
			var _user = UserService.GetByName(User.GetUserName());
			var _count = NoteService.Search(_user.Id, q ?? String.Empty, false, null, null).Count();
			var _notes = NoteService.Search(_user.Id, q ?? String.Empty, false, ofs, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, _count);

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