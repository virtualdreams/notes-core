using Microsoft.AspNetCore.Mvc;
using notes.Core.Models;
using notes.Core.Services;
using notes.Extensions;

namespace notes.ViewComponents
{
	public abstract class BaseViewComponent : ViewComponent
	{
		private readonly UserService UserService;

		public User CurrentUser => UserService.GetByNameAsync(UserClaimsPrincipal.GetUserName()).Result;

		public BaseViewComponent(UserService userService)
		{
			UserService = userService;
		}
	}
}