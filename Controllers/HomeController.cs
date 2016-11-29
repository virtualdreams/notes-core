using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using postit.Core.Services;
using postit.Models;
using Microsoft.AspNetCore.Authorization;
using postit.Helper;
using System.Linq;
using System;

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
			var _count = PostitService.Get(_user.Id, false, null, null).Count();
			var _postits = PostitService.Get(_user.Id, false, ofs ?? 0, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, _count);
			
			var postits = Mapper.Map<IEnumerable<PostitModel>>(_postits);
			
			var view = new PostitListContainer
			{
				Postits = postits,
				Offset = _pager
			};

			return View(view);
		}

		[HttpGet]
		public IActionResult Search(string q, int? ofs)
		{
			var _user = UserService.GetByName(User.GetUserName());
			var _count = PostitService.Search(_user.Id, q ?? String.Empty, false, null, null).Count();
			var _postits = PostitService.Search(_user.Id, q ?? String.Empty, false, ofs, 10);
			var _pager = new PageOffset(ofs ?? 0, 10, _count);

			var postits = Mapper.Map<IEnumerable<PostitModel>>(_postits);
			
			var view = new PostitSearchContainer
			{
				Postits = postits,
				Offset = _pager,
				Term = q?.Trim()
			};

			return View(view);
		}
	}
}