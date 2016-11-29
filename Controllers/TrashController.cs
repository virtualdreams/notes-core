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
	public class TrashController : Controller
	{
		private readonly PostitService PostitService;
		private readonly UserService UserService;

		public TrashController(PostitService postit, UserService user)
		{
			PostitService = postit;
			UserService = user;
		}

		[HttpGet]
		public IActionResult Index(int? ofs)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _count = PostitService.Get(_user.Id, true, null, null).Count();
			var _postits = PostitService.Get(_user.Id, true, ofs ?? 0, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, _count);
			
			var postits = Mapper.Map<IEnumerable<PostitModel>>(_postits);
			
			var view = new PostitListContainer
			{
				Postits = postits,
				Offset = _pager
			};

			return View(view);
		}
	}
}