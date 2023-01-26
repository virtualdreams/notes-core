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

namespace Notes.Controllers
{
	[Authorize]
	public class HomeController : BaseController
	{
		private readonly ILogger<HomeController> Log;

		private readonly IMapper Mapper;

		private readonly AppSettings AppSettings;

		private readonly IUserService UserService;

		private readonly INoteService NoteService;

		public HomeController(
			ILogger<HomeController> log,
			IMapper mapper,
			IOptionsSnapshot<AppSettings> appSettings,
			IUserService user,
			INoteService note)
			: base(user)
		{
			Log = log;
			Mapper = mapper;
			AppSettings = appSettings.Value;
			UserService = user;
			NoteService = note;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int after)
		{
			var _notes = await NoteService.GetNotesAsync(after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? 0, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteListContainer
			{
				Notes = notes,
				Pager = _pager
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