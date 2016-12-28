using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using notes.Core.Services;
using notes.Helper;
using notes.Models;

namespace notes.Controllers
{
    [Authorize]
    public class NoteController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly NoteService NoteService;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Options;

		public NoteController(IMapper mapper, NoteService note, UserService user, IOptions<Settings> options)
			: base(user)
		{
			Mapper = mapper;
			NoteService = note;
			UserService = user;
			Options = options;
		}

		[HttpGet]
		public IActionResult View(ObjectId id)
		{
			var _note = NoteService.GetById(id, UserId);
			if(_note == null)
				return new StatusCodeResult(404);
			
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
			if(_note == null)
				return new StatusCodeResult(404);

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
			if(!ModelState.IsValid)
			{
				var view = new NoteEditContainer
				{
					Note = new NoteModel
					{
						Id = model.Id,
						Title = model.Title,
						Content = model.Content
					}
				};

				return View(view);
			}
			
			var _id = ObjectId.Empty;
			if(model.Id == ObjectId.Empty)
			{
				_id = NoteService.Create(UserId, model.Title, model.Content);
			}
			else
			{
				var _note = NoteService.GetById(model.Id, UserId);
				if(_note == null)
					return new StatusCodeResult(404);

				_id = NoteService.Update(model.Id, model.Title, model.Content);
			}

			return RedirectToAction("view", "note", new { id = _id, slug = model.Title.ToSlug() });
		}

		[HttpGet]
		public IActionResult Notebook(string id, ObjectId after)
		{
			var _pageSize = UserSettings?.PageSize ?? Options.Value.PageSize;
			var _notes = NoteService.GetByNotebook(UserId, id ?? string.Empty, after, _pageSize);
			var _pager = new Pager(_notes.Item1.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Item2);

			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes.Item1);
			
			var view = new NoteNotebookContainer
			{
				Notes = notes,
				Pager = _pager,
				Notebook = id?.Trim()
			};

			return View(view);
		}

		[HttpPost]
		public IActionResult Notebook(NotebookPostModel model)
		{
			var _note = NoteService.GetById(model.Id, UserId);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.SetNotebook(model.Id, model.Notebook);
			
			return new NoContentResult();
		}

		[HttpGet]
		public IActionResult Tag(string id, ObjectId after)
		{
			var _pageSize = UserSettings?.PageSize ?? Options.Value.PageSize;
			var _notes = NoteService.GetByTag(UserId, id ?? string.Empty, after, _pageSize);
			var _pager = new Pager(_notes.Item1.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Item2);
			
			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes.Item1);
			
			var view = new NoteTagContainer
			{
				Notes = notes,
				Pager = _pager,
				Tag = id?.Trim()
			};

			return View(view);
		}

		[HttpPost]
		public IActionResult Tags(TagsPostModel model)
		{
			var _note = NoteService.GetById(model.Id, UserId);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.SetTags(model.Id, model.Tags);

			return new NoContentResult();
		}

		[HttpPost]
		public IActionResult Remove(ObjectId id)
		{
			var _note = NoteService.GetById(id, UserId);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.Trash(id, !_note.Trash);

			return new NoContentResult();
		}

		[HttpGet]
		public IActionResult Trash(ObjectId after)
		{
			var _pageSize = UserSettings?.PageSize ?? Options.Value.PageSize;
			var _notes = NoteService.Get(UserId, after, true, _pageSize);
			var _pager = new Pager(_notes.Item1.LastOrDefault()?.Id ?? ObjectId.Empty, _notes.Item2);
			
			var notes = Mapper.Map<IEnumerable<NoteModel>>(_notes.Item1);
			
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
			if(model.Id != null)
			{
				foreach(var note in model?.Id)
				{
					NoteService.Delete(note, UserId);
				}
			}

			return RedirectToAction("trash");
		}
	}
}