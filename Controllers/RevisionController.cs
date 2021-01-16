using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Notes.Core.Interfaces;
using Notes.Extensions;
using Notes.Models;
using Notes.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Controllers
{
	[Authorize]
	public class RevisionController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly AppSettings AppSettings;
		private readonly IUserService UserService;
		private readonly INoteService NoteService;
		private readonly IRevisionService RevisionService;

		public RevisionController(IMapper mapper, IOptionsSnapshot<AppSettings> settings, IUserService user, INoteService note, IRevisionService revision)
			: base(user)
		{
			Mapper = mapper;
			AppSettings = settings.Value;
			UserService = user;
			NoteService = note;
			RevisionService = revision;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int id)
		{
			var _note = await NoteService.GetByIdAsync(id);
			if (_note == null)
				return NotFound();

			var _revisions = await RevisionService.GetRevisionsAsync(id);

			var revisions = Mapper.Map<IEnumerable<RevisionModel>>(_revisions);

			var _view = new RevisionListContainer
			{
				Revisions = revisions
			};

			return View(_view);
		}

		[HttpGet]
		public async Task<IActionResult> View(int id, int diff)
		{
			var _revision = await RevisionService.GetRevisionAsync(id);
			if (_revision == null)
				return NotFound();

			var _diff = await RevisionService.GetDiffAsync(_revision.Id);

			var revision = Mapper.Map<RevisionModel>(_revision);

			var view = new RevisionViewContainer
			{
				Revision = revision,
				Diff = _diff
			};

			return View(view);
		}

		[HttpGet]
		[Authorize(Policy = "AdministratorOnly")]
		public async Task<IActionResult> Restore(int id)
		{
			var _revision = await RevisionService.GetRevisionAsync(id);
			if (_revision == null)
				return NotFound();

			await RevisionService.RestoreAsync(_revision.Id);

			return RedirectToAction("view", "note", new { id = _revision.NoteId, slug = _revision.Title.ToSlug() });
		}
	}
}