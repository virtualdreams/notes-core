using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Core;
using notes.Extensions;
using notes.Models;

namespace notes.Controllers
{
	[Authorize]
	public class NoteController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly Settings Options;
		private readonly UserService UserService;
		private readonly NoteService NoteService;

		public NoteController(IMapper mapper, IOptionsSnapshot<Settings> settings, UserService user, NoteService note)
			: base(user)
		{
			Mapper = mapper;
			Options = settings.Value;
			UserService = user;
			NoteService = note;
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
			if (ModelState.IsValid)
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

				return View(view);
			}
		}

		[HttpPost]
		public async Task<IActionResult> Preview(NotePostModel model)
		{
			// remove some error messages, because they are not needed for preview mode
			ModelState.Remove("title");
			ModelState.Remove("notebook");
			ModelState.Remove("tags");

			if (ModelState.IsValid)
			{
				var view = new NoteViewContainer
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
			var _notes = await NoteService.GetNotesAsync(after, true, PageSize);
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
			if (ModelState.IsValid)
			{
				foreach (var note in model.Id)
				{
					await NoteService.DeleteAsync(note);
				}
			}

			return RedirectToAction("trash");
		}

		[HttpPost]
		public async Task<IActionResult> Restore(NoteTrashPostModel model)
		{
			if (ModelState.IsValid)
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

			return RedirectToAction("trash");
		}

		[Route("search/tags")]
		public async Task<IActionResult> TagSuggestions(string term)
		{
			return Json((await NoteService.TagSuggestionsAsync(term)).ToArray());
		}

		[Route("search/notebook")]
		public async Task<IActionResult> NotebookSuggestions(string term)
		{
			return Json((await NoteService.NotebookSuggestionsAsync(term)).ToArray());
		}
	}
}