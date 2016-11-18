using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using postit.Core.Services;
using postit.Models;

namespace postit.Controllers
{
    public class PostitController : Controller
	{
		private readonly PostitService Postit;
		private readonly CommentService Comment;

		public PostitController(PostitService postit, CommentService comment)
		{
			Postit = postit;
			Comment = comment;
		}

		[HttpGet]
		public IActionResult View(ObjectId id)
		{
			var _postit = Postit.GetById(id);
			if(_postit == null)
				return new StatusCodeResult(404);

			var _comments = Comment.GetByPost(id).ToArray();

			var postit = Mapper.Map<PostitModel>(_postit);
			var comments = Mapper.Map<IEnumerable<CommentModel>>(_comments);

			var view = new PostitViewContainer
			{
				Postit = postit,
				Comments = comments
			};

			return View(view);
		}
	}
}