using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;

namespace notes.Controllers
{
	[Authorize]
	public class KanbanController : BaseController
	{
		private readonly UserService UserService;

		public KanbanController(UserService user)
			: base(user)
		{
			UserService = user;
		}

		public ActionResult Index()
		{
			return View();
		}
	}
}