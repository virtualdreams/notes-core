using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var _user = await UserService.GetById(UserId);
			if (_user == null)
				return Content("unknown");

			var _displayName = String.IsNullOrEmpty(_user.DisplayName) ? _user.Username : _user.DisplayName;

			return Content($"{_displayName}");
		}
	}
}