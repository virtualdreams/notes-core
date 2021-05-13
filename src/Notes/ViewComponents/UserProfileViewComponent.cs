using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using System;

namespace Notes.ViewComponents
{
	public class DisplayNameViewComponent : BaseViewComponent
	{
		private readonly IUserService UserService;

		public DisplayNameViewComponent(IUserService user)
			: base(user)
		{
			UserService = user;
		}

		public IViewComponentResult Invoke()
		{
			var _displayName = String.IsNullOrEmpty(CurrentUser.DisplayName) ? CurrentUser.Username : CurrentUser.DisplayName;

			return Content($"{_displayName}");
		}
	}
}