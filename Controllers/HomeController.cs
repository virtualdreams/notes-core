using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using postit.Core.Services;
using postit.Models;
using Microsoft.AspNetCore.Authorization;
using postit.Helper;

namespace postit.Controllers
{
	[Authorize]
    public class HomeController: Controller
	{
		private readonly PostitService PostitService;
		private readonly UserService UserService;

		public HomeController(PostitService postit, UserService user)
		{
			PostitService = postit;
			UserService = user;
		}

		[HttpGet]
		public IActionResult Index(int? ofs)
		{
			var _user = UserService.GetByName(User.GetUserName());

			var _postits = PostitService.Get(_user.Id, ofs ?? 0, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, 100);
			
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