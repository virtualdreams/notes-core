using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using Notes.Core.Models;
using Notes.Extensions;

namespace Notes.ViewComponents
{
	public abstract class BaseViewComponent : ViewComponent
	{
		private readonly IUserService UserService;

		protected User CurrentUser => UserService.GetByNameAsync(UserClaimsPrincipal.GetUserName()).Result;

		public BaseViewComponent(IUserService user)
		{
			UserService = user;
		}
	}
}