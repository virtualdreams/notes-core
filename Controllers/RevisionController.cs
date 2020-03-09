using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Extensions;
using notes.Models;

namespace notes.Controllers
{
	[Authorize]
	public class RevisionController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly Settings Options;
		private readonly UserService UserService;
		private readonly NoteService NoteService;
		private readonly RevisionService RevisionService;

		public RevisionController(IMapper mapper, IOptionsSnapshot<Settings> settings, UserService user, NoteService note, RevisionService revision)
			: base(user)
		{
			Mapper = mapper;
			Options = settings.Value;
			UserService = user;
			NoteService = note;
			RevisionService = revision;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int id)
		{
			var _note = await NoteService.GetById(id);
			if (_note == null)
				return NotFound();

			var _revisions = await RevisionService.GetRevisions(id);

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
			var _revision = await RevisionService.GetRevision(id);
			if (_revision == null)
				return NotFound();

			var _diff = await RevisionService.GetDiff(_revision.Id);

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
			var _revision = await RevisionService.GetRevision(id);
			if (_revision == null)
				return NotFound();

			await RevisionService.Restore(_revision.Id);

			return RedirectToAction("view", "note", new { id = _revision.NoteId, slug = _revision.Title.ToSlug() });
		}
	}
}