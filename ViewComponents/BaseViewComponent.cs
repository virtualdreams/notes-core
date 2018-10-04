using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using notes.Core.Services;
using notes.Helper;

namespace notes.ViewComponents
{
	public abstract class BaseViewComponent : ViewComponent
	{
		private readonly UserService UserService;

		public ObjectId UserId => UserService.GetUserId((User as ClaimsPrincipal).GetUserName()).Result;

		public BaseViewComponent(UserService userService)
		{
			UserService = userService;
		}
	}
}