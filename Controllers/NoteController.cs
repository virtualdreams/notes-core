using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Mvc.RenderViewToString;
using System.Collections.Generic;
using System.Linq;
using notes.Core.Services;
using notes.Extensions;
using notes.Helper;
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
		private readonly RazorViewToStringRenderer ViewRenderService;

		private int PageSize => UserSettings?.PageSize ?? Options.PageSize;

		public NoteController(IMapper mapper, Settings settings, UserService user, NoteService note, RazorViewToStringRenderer render)
			: base(user)
		{
			Mapper = mapper;
			Options = settings;
			UserService = user;
			NoteService = note;
			ViewRenderService = render;
		}

		[HttpGet]
		public IActionResult View(ObjectId id)
		{
			var _note = NoteService.GetById(id, UserId);
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
		public IActionResult Print(ObjectId id)
		{
			var _note = NoteService.GetById(id, UserId);
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
		public IActionResult Edit(ObjectId id)
		{
			var _note = NoteService.GetById(id, UserId);
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
		public IActionResult Edit(NotePostModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var _id = ObjectId.Empty;
					if (model.Id == ObjectId.Empty)
					{
						_id = NoteService.Create(UserId, model.Title, model.Content, model.Notebook, model.Tags);
					}
					else
					{
						NoteService.Update(model.Id, UserId, model.Title, model.Content, model.Notebook, model.Tags);
						_id = model.Id;
					}

					if (Request.IsAjaxRequest())
					{
						return Json(new { Success = true, Id = _id.ToString(), Error = "" });
					}
					else
					{
						return RedirectToAction("view", "note", new { id = model.Title.ToSlug(_id) });
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
		public IActionResult Preview(NotePostModel model)
		{
			// remove title related error messages, because is not needed for preview mode
			ModelState.Remove("title");
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

				var _content = ViewRenderService.RenderViewToStringAsync("/Views/Shared/_Preview.cshtml", view).Result;

				return Json(new { Success = true, Content = _content });
			}

			return Json(new { Success = false, Content = string.Empty });
		}

		[HttpGet]
		public IActionResult Notebooks()
		{
			var _notebooks = NoteService.GetNotebooks(UserId);

			var notebooks = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_notebooks);

			return View(notebooks);
		}

		[HttpGet]
		public IActionResult Tags()
		{
			var _tags = NoteService.GetTags(UserId);

			var tags = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_tags);

			return View(tags);
		}

		[HttpGet]
		public IActionResult Notebook(string id, ObjectId after)
		{
			var _notes = NoteService.GetByNotebook(UserId, id, after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Count() >= PageSize);

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
		public IActionResult Tag(string id, ObjectId after)
		{
			var _notes = NoteService.GetByTag(UserId, id, after, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Count() >= PageSize);

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
		public IActionResult Remove(ObjectId id)
		{
			var _note = NoteService.GetById(id, UserId);
			if (_note == null)
				return NotFound();

			NoteService.Trash(id, !_note.Trash);

			return new NoContentResult();
		}

		[HttpGet]
		public IActionResult Trash(ObjectId after)
		{
			var _notes = NoteService.GetNotes(UserId, after, true, PageSize);
			var _pager = new Pager(_notes.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Count() >= PageSize);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes);

			var view = new NoteListContainer
			{
				Notes = notes,
				Pager = _pager
			};

			return View(view);
		}

		[HttpPost]
		public IActionResult Delete(NoteDeleteModel model)
		{
			if (ModelState.IsValid)
			{
				foreach (var note in model.Id)
				{
					NoteService.Delete(note, UserId);
				}
			}

			return RedirectToAction("trash");
		}

		[Route("search/tags")]
		public IActionResult TagSuggestions(string term)
		{
			return Json(NoteService.TagSuggestions(UserId, term).ToArray());
		}

		[Route("search/notebook")]
		public IActionResult NotebookSuggestions(string term)
		{
			return Json(NoteService.NotebookSuggestions(UserId, term).ToArray());
		}
	}
}