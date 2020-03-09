using Microsoft.AspNetCore.Mvc;
using notes.Core.Models;
using notes.Core.Services;
using notes.Extensions;

namespace notes.Controllers
{
	public abstract class BaseController : Controller
	{
		private readonly UserService UserService;

		public User CurrentUser => UserService.GetByName(User.GetUserName()).Result;

		public int PageSize => CurrentUser.Items;

		public BaseController(UserService userService)
		{
			UserService = userService;
		}
	}
}