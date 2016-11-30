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
	public class TrashController : Controller
	{
		private readonly NoteService NoteService;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Settings;

		public TrashController(NoteService note, UserService user, IOptions<Settings> settings)
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
			var _count = NoteService.Get(_user.Id, true, null, null).Count();
			var _notes = NoteService.Get(_user.Id, true, ofs ?? 0, _pageSize);
			var _pager = new PageOffset(ofs ?? 0, _pageSize, _count);
			
			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);
			
			var view = new NoteListContainer
			{
				Notes = notes,
				Offset = _pager
			};

			return View(view);
		}

		[HttpPost]
		public IActionResult Delete(NoteDeleteModel model)
		{
			if(model.Id != null)
			{
				var _user = UserService.GetByName(User.GetUserName());
				foreach(var note in model?.Id)
				{
					NoteService.Delete(note, _user.Id);
				}
			}

			return RedirectToAction("index");
		}
	}
}