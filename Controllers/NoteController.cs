using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Newtonsoft.Json;
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
		private readonly IOptions<Settings> Settings;

		public NoteController(IMapper mapper, NoteService note, UserService user, IOptions<Settings> settings)
			: base(user)
		{
			Mapper = mapper;
			NoteService = note;
			UserService = user;
			Settings = settings;
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

		[HttpPost]
		public IActionResult Notebook(NotebookPostModel model)
		{
			var _note = NoteService.GetById(model.Id, UserId);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.SetNotebook(model.Id, model.Notebook);

			return Json(new { Success = true }, new JsonSerializerSettings { Formatting = Formatting.Indented });
		}

		[HttpPost]
		public IActionResult Tags(TagsPostModel model)
		{
			var _note = NoteService.GetById(model.Id, UserId);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.SetTags(model.Id, model.Tags);

			return Json(new { Success = true }, new JsonSerializerSettings { Formatting = Formatting.Indented });
		}

		[HttpPost]
		public IActionResult Trash(ObjectId id)
		{
			var _note = NoteService.GetById(id, UserId);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.Trash(id, !_note.Trash);

			return Json(new { Success = true }, new JsonSerializerSettings { Formatting = Formatting.Indented });
		}
	}
}