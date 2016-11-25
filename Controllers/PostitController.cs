using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using postit.Core.Services;
using postit.Helper;
using postit.Models;

namespace postit.Controllers
{
    public class PostitController : Controller
	{
		private readonly PostitService PostitService;

		public PostitController(PostitService postit)
		{
			PostitService = postit;
		}

		[HttpGet]
		public IActionResult View(ObjectId id)
		{
			var _postit = PostitService.GetById(id);
			if(_postit == null)
				return new StatusCodeResult(404);

			var postit = Mapper.Map<PostitModel>(_postit);

			var view = new PostitViewContainer
			{
				Postit = postit
			};

			return View(view);
		}

		[HttpGet]
		public IActionResult Edit(ObjectId id)
		{
			var _postit = PostitService.GetById(id);
			if(_postit == null)
				return new StatusCodeResult(404);

			var postit = Mapper.Map<PostitModel>(_postit);

			var view = new PostitEditContainer
			{
				Postit = postit
			};

			return View("Edit", view);
		}

		[HttpGet]
		public IActionResult Create()
		{
			var view = new PostitEditContainer
			{
				Postit = new PostitModel()
			};

			return View("Edit", view);
		}

		[HttpPost]
		public IActionResult Edit(PostitPostModel model)
		{
			if(model.Id == ObjectId.Empty)
			{
				var _result = PostitService.Create(ObjectId.Empty, model.Title, model.Content);
			}
			else
			{
				var _result = PostitService.Update(model.Id, model.Title, model.Content);
			}

			return RedirectToAction("index", "home");
		}
	}
}