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
		public IActionResult Index()
		{
			var _postits = Postit.Get().ToArray();
			
			var postits = Mapper.Map<IEnumerable<PostitModel>>(_postits);
			
			var view = new PostitListContainer
			{
				Postits = postits 
			};

			return View(view);
		}
	}
}