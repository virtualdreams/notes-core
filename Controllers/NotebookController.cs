using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using postit.Core.Services;
using postit.Helper;
using postit.Models;

namespace postit.Controllers
{
	[Authorize]
	public class NotebookController : Controller
	{
		private readonly PostitService PostitService;
		private readonly UserService UserService;

		public NotebookController(PostitService postit, UserService user)
		{
			PostitService = postit;
			UserService = user;
		}

		public IActionResult View(string id, int? ofs)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _count = PostitService.GetByNotebook(_user.Id, id, null, null).Count();
			var _postits = PostitService.GetByNotebook(_user.Id, id, ofs ?? 0, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, _count);
			
			var postits = Mapper.Map<IEnumerable<PostitModel>>(_postits);
			
			var view = new PostitNotebookContainer
			{
				Postits = postits,
				Offset = _pager,
				Notebook = id?.Trim()
			};

			return View(view);
		}
	}
}