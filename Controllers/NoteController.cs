using System.Linq;
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
    public class NoteController : Controller
	{
		private readonly NoteService NoteService;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Settings;

		public NoteController(NoteService note, UserService user, IOptions<Settings> settings)
		{
			NoteService = note;
			UserService = user;
			Settings = settings;
		}

		[HttpGet]
		public IActionResult View(ObjectId id)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _note = NoteService.GetById(id, _user.Id);
			if(_note == null)
				return new StatusCodeResult(404);
			
			var tags = NoteService.Tags(_user.Id).ToArray();
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
			var _user = UserService.GetByName(User.GetUserName());

			var _note = NoteService.GetById(id, _user.Id);
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
				var _user = UserService.GetByName(User.GetUserName());
				_id = NoteService.Create(_user.Id, model.Title, model.Content);
			}
			else
			{
				var _user = UserService.GetByName(User.GetUserName());
				var _note = NoteService.GetById(model.Id, _user.Id);
				if(_note == null)
					return new StatusCodeResult(404);

				_id = NoteService.Update(model.Id, model.Title, model.Content);
			}

			return RedirectToAction("view", "note", new { id = _id, slug = model.Title.ToSlug() });
		}

		[HttpPost]
		public IActionResult Notebook(ObjectId id, string name)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _note = NoteService.GetById(id, _user.Id);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.SetNotebook(id, name);

			return Json(new { Success = true }, new JsonSerializerSettings { Formatting = Formatting.Indented });
		}

		[HttpPost]
		public IActionResult Tags(ObjectId id, string tags)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _note = NoteService.GetById(id, _user.Id);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.SetTags(id, tags);

			return Json(new { Success = true }, new JsonSerializerSettings { Formatting = Formatting.Indented });
		}

		[HttpPost]
		public IActionResult Trash(ObjectId id)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _note = NoteService.GetById(id, _user.Id);
			if(_note == null)
				return new StatusCodeResult(404);

			NoteService.Trash(id, !_note.Trash);

			return Json(new { Success = true }, new JsonSerializerSettings { Formatting = Formatting.Indented });
		}
	}
}