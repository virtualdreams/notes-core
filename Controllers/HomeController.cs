using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using postit.Core.Services;
using postit.Models;

namespace postit.Controllers
{
	public class HomeController: Controller
	{
		private readonly PostitService Postit;

		public HomeController(PostitService postit)
		{
			Postit = postit;
		}

		[HttpGet]
		public IActionResult Index(int? ofs)
		{
			var _postits = Postit.Get(ofs ?? 0, 10);
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