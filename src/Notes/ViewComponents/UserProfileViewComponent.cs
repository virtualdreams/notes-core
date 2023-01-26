using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using System;

namespace Notes.ViewComponents
{
	public class DisplayNameViewComponent : BaseViewComponent
	{
		public DisplayNameViewComponent(
			IUserService user)
			: base(user)
		{ }

		public IViewComponentResult Invoke()
		{
			var _displayName = String.IsNullOrEmpty(CurrentUser.DisplayName) ? CurrentUser.Username : CurrentUser.DisplayName;

			return Content($"{_displayName}");
		}
	}
}