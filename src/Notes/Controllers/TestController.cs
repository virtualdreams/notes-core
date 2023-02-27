using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Notes.Core.Interfaces;
using Notes.Models;
using Notes.Options;
using System.Threading.Tasks;

namespace Notes.Controllers
{
	public class TestController : BaseController
	{
		private readonly IMapper Mapper;

		private readonly AppSettings AppSettings;

		private readonly IUserService UserService;

		private readonly INoteService NoteService;

		public TestController(IMapper mapper, IOptionsSnapshot<AppSettings> settings, IUserService user, INoteService note)
			: base(user)
		{
			Mapper = mapper;
			AppSettings = settings.Value;
			UserService = user;
			NoteService = note;
		}

		[HttpGet]
		public async Task<IActionResult> List()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> View(int id)
		{
			var _note = await NoteService.GetByIdAsync(id);
			if (_note == null)
				return NotFound();

			var note = Mapper.Map<NoteModel>(_note);

			var view = new NoteViewContainer
			{
				Note = note
			};

			return View(view);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var view = new NoteEditContainer
			{
				Note = new NoteModel()
			};

			return View("Edit", view);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var _note = await NoteService.GetByIdAsync(id);
			if (_note == null)
				return NotFound();

			var note = Mapper.Map<NoteModel>(_note);

			var view = new NoteEditContainer
			{
				Note = note
			};

			return View("Edit", view);
		}
	}
}