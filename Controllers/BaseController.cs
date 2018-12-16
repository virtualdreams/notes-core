using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using notes.Core.Models;
using notes.Core.Services;
using notes.Helper;

namespace notes.Controllers
{
	public abstract class BaseController : Controller
	{
		private readonly UserService UserService;

		public ObjectId UserId => UserService.GetUserId(User.GetUserName()).Result;
		private UserSettings UserSettings => UserService.GetUserSettings(UserId).Result;
		public int PageSize => UserSettings.PageSize;

		public BaseController(UserService userService)
		{
			UserService = userService;
		}
	}
}