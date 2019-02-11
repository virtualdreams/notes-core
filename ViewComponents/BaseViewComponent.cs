using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using notes.Core.Models;
using notes.Core.Services;
using notes.Helper;

namespace notes.ViewComponents
{
	public abstract class BaseViewComponent : ViewComponent
	{
		private readonly UserService UserService;

		public User CurrentUser => UserService.GetByName((User as ClaimsPrincipal).GetUserName()).Result;

		public BaseViewComponent(UserService userService)
		{
			UserService = userService;
		}
	}
}