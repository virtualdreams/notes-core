using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using Notes.Core.Models;
using Notes.Extensions;

namespace Notes.Controllers
{
	public abstract class BaseController : Controller
	{
		private readonly IUserService UserService;

		protected User CurrentUser => UserService.GetByNameAsync(User.GetUserName()).Result;

		protected int PageSize => CurrentUser.Items;

		public BaseController(IUserService userService)
		{
			UserService = userService;
		}
	}
}