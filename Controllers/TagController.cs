using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using notes.Core.Services;
using notes.Helper;
using notes.Models;

namespace notes.Controllers
{
	[Authorize]
	public class TagController : Controller
	{
		private readonly NoteService NoteService;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Settings;

		public TagController(NoteService note, UserService user, IOptions<Settings> settings)
		{
			NoteService = note;
			UserService = user;
			Settings = settings;
		}

		[HttpGet]
		public IActionResult View(string id, int? ofs)
		{
			var _pageSize = Settings.Value.PageSize;
			var _user = UserService.GetByName(User.GetUserName());
			var _count = NoteService.GetByTag(_user.Id, id, null, null).Count();
			var _notes = NoteService.GetByTag(_user.Id, id, ofs ?? 0, _pageSize);
			var _pager = new PageOffset(ofs ?? 0, _pageSize, _count);
			
			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);
			
			var view = new NoteTagContainer
			{
				Notes = notes,
				Offset = _pager,
				Tag = id?.Trim()
			};

			return View(view);
		}
	}
}