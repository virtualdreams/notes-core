using Microsoft.AspNetCore.Mvc;
using notes.Core.Models;
using notes.Core.Services;
using notes.Extensions;

namespace notes.Controllers
{
	public abstract class BaseController : Controller
	{
		private readonly UserService UserService;

		protected User CurrentUser => UserService.GetByNameAsync(User.GetUserName()).Result;

		protected int PageSize => CurrentUser.Items;

		public BaseController(UserService userService)
		{
			UserService = userService;
		}
	}
}