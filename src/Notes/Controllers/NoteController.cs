using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notes.Core.Interfaces;
using Notes.Core;
using Notes.Extensions;
using Notes.FluentValidation;
using Notes.Models;
using Notes.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Controllers
{
	[Authorize]
	public class NoteController : BaseController
	{
		private readonly ILogger<NoteController> Log;

		private readonly IMapper Mapper;

		private readonly AppSettings AppSettings;

		private readonly IUserService UserService;

		private readonly INoteService NoteService;

		private readonly IValidator<NotePostModel> NotePostModelValidator;

		private readonly IValidator<NotePreviewPostModel> NotePreviewPostModelValidator;

		private readonly IValidator<NoteTrashPostModel> NoteTrashPostModelValidator;

		public NoteController(
			ILogger<NoteController> log,
			IMapper mapper,
			IOptionsSnapshot<AppSettings> appSettings,
			IUserService user,
			INoteService note,
			IValidator<NotePostModel> notePostModelValidator,
			IValidator<NotePreviewPostModel> notePreviewPostModelValidator,
			IValidator<NoteTrashPostModel> noteTrashPostModelValidator)
			: base(user)
		{
			Log = log;
			Mapper = mapper;
			AppSettings = appSettings.Value;
			UserService = user;
			NoteService = note;
			NotePostModelValidator = notePostModelValidator;
			NotePreviewPostModelValidator = notePreviewPostModelValidator;
			NoteTrashPostModelValidator = noteTrashPostModelValidator;
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
		public async Task<IActionResult> Print(int id)
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
		public IActionResult Create()
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

		[HttpPost]
		public async Task<IActionResult> Edit(NotePostModel model)
		{
			var _result = await NotePostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				try
				{
					var _id = 0;
					if (model.Id == 0)
					{
						var _note = await NoteService.CreateAsync(model.Title, model.Content, model.Notebook, model.Tags);
						_id = _note.Id;
					}
					else
					{
						await NoteService.UpdateAsync(model.Id, model.Title, model.Content, model.Notebook, model.Tags);
						_id = model.Id;
					}

					if (Request.IsAjaxRequest())
					{
						return Json(new { Success = true, Id = _id, Error = "" });
					}
					else
					{
						return RedirectToAction("view", "note", new { id = _id, slug = model.Title.ToSlug() });
					}
				}
				catch (NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			// validation failed
			if (Request.IsAjaxRequest())
			{
				return Json(new { Success = false, Id = model.Id.ToString(), Error = "" });
			}
			else
			{
				var view = new NoteEditContainer
				{
					Note = new NoteModel
					{
						Id = model.Id,
						Title = model.Title,
						Content = model.Content,
						Notebook = model.Notebook,
						TagsString = model.Tags
					}
				};

				_result.AddToModelState(ModelState);
				return View(view);
			}
		}

		[HttpPost]
		public async Task<IActionResult> Preview(NotePreviewPostModel model)
		{
			var _result = await NotePreviewPostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				var view = new NoteViewContainer
				{
					Note = new NoteModel
					{
						Content = model.Content,
					}
				};

				var _content = await Task.Run(() => view.Note.Content.ToMarkdown());

				return Json(new { Success = true, Content = _content });
			}

			return Json(new { Success = false, Content = string.Empty });
		}

		[HttpGet]
		public async Task<IActionResult> Notebooks()
		{
			var _notebooks = await NoteService.GetNotebooksAsync();

			var notebooks = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_notebooks);

			return View(notebooks);
		}

		[HttpGet]
		public async Task<IActionResult> Tags()
		{
			var _tags = await NoteService.GetTagsAsync();

			var tags = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_tags);

			return View(tags);
		}

		[HttpGet]
		public async Task<IActionResult> Notebook(string id, int after)
		{
			var _notes = await NoteService.GetByNotebookAsync(id, after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? 0, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteNotebookContainer
			{
				Notes = notes,
				Pager = _pager,
				Notebook = id?.Trim()
			};

			return View(view);
		}

		[HttpGet]
		public async Task<IActionResult> Tag(string id, int after)
		{
			var _notes = await NoteService.GetByTagAsync(id, after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? 0, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteTagContainer
			{
				Notes = notes,
				Pager = _pager,
				Tag = id?.Trim()
			};

			return View(view);
		}

		[HttpPost]
		[SkipStatusCodePages]
		public async Task<IActionResult> Remove(int id)
		{
			var _note = await NoteService.GetByIdAsync(id);
			if (_note == null || _note.Trash == true)
				return NotFound();

			await NoteService.TrashAsync(id, true);

			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> Trash(int after)
		{
			var _notes = await NoteService.GetDeletedNotes(after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? 0, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteListContainer
			{
				Notes = notes,
				Pager = _pager
			};

			return View(view);
		}

		[HttpPost]
		[Authorize(Policy = "AdministratorOnly")]
		public async Task<IActionResult> Delete(NoteTrashPostModel model)
		{
			var _result = await NoteTrashPostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				foreach (var note in model.Id)
				{
					await NoteService.DeleteAsync(note);
				}
			}

			_result.AddToModelState(ModelState);
			return RedirectToAction("trash");
		}

		[HttpPost]
		public async Task<IActionResult> Restore(NoteTrashPostModel model)
		{
			var _result = await NoteTrashPostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				foreach (var note in model.Id)
				{
					var _note = await NoteService.GetByIdAsync(note);
					if (_note != null)
					{
						await NoteService.TrashAsync(note, false);
					}
				}
			}

			_result.AddToModelState(ModelState);
			return RedirectToAction("trash");
		}
	}
}