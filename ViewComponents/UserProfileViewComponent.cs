using Microsoft.AspNetCore.Mvc;
using System;
using notes.Core.Services;

namespace notes.ViewComponents
{
	public class DisplayNameViewComponent : BaseViewComponent
	{
		private readonly UserService UserService;

		public DisplayNameViewComponent(UserService user)
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