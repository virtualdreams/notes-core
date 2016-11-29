using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;
using notes.Helper;
using notes.Models;

namespace notes.Controllers
{
	[Authorize]
	public class NotebookController : Controller
	{
		private readonly NoteService NoteService;
		private readonly UserService UserService;

		public NotebookController(NoteService note, UserService user)
		{
			NoteService = note;
			UserService = user;
		}

		public IActionResult View(string id, int? ofs)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _count = NoteService.GetByNotebook(_user.Id, id, null, null).Count();
			var _notes = NoteService.GetByNotebook(_user.Id, id, ofs ?? 0, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, _count);
			
			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);
			
			var view = new NoteNotebookContainer
			{
				Notes = notes,
				Offset = _pager,
				Notebook = id?.Trim()
			};

			return View(view);
		}
	}
}