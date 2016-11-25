using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using postit.Core.Services;
using postit.Helper;
using postit.Models;

namespace postit.Controllers
{
    [Authorize]
    public class PostitController : Controller
	{
		private readonly PostitService PostitService;
		private readonly UserService UserService;

		public PostitController(PostitService postit, UserService user)
		{
			PostitService = postit;
			UserService = user;
		}

		[HttpGet]
		public IActionResult View(ObjectId id)
		{
			var _user = UserService.GetByName(User.GetUserName());

			var _postit = PostitService.GetById(id, _user.Id);
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
			var _user = UserService.GetByName(User.GetUserName());

			var _postit = PostitService.GetById(id, _user.Id);
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
			if(!ModelState.IsValid)
			{
				var view = new PostitEditContainer
				{
					Postit = new PostitModel
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
				_id = PostitService.Create(_user.Id, model.Title, model.Content);
			}
			else
			{
				_id = PostitService.Update(model.Id, model.Title, model.Content);
			}

			return RedirectToAction("view", "postit", new { id = _id, slug = model.Title.ToSlug() });
		}

		[HttpPost]
		public IActionResult Trash(ObjectId id)
		{
			var _user = UserService.GetByName(User.GetUserName());

			var _postit = PostitService.GetById(id, _user.Id);
			if(_postit == null)
				return new StatusCodeResult(404);

			PostitService.Trash(id, _user.Id);

			return RedirectToAction("index", "home");
		}

		public IActionResult Restore(ObjectId id)
		{
			var _user = UserService.GetByName(User.GetUserName());

			var _postit = PostitService.GetById(id, _user.Id);
			if(_postit == null)
				return new StatusCodeResult(404);

			PostitService.Restore(id, _user.Id);

			return RedirectToAction("index", "home");
		}
	}
}