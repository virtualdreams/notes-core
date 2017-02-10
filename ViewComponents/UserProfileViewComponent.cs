using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;

namespace notes.ViewComponents
{
    public class UserProfileViewComponent : BaseViewComponent
    {
        private readonly UserService UserService;

		public UserProfileViewComponent(UserService user)
			: base(user)
		{
			UserService = user;
		}

        public IViewComponentResult Invoke()
		{
			var _user = UserService.GetUserById(UserId);
			if(_user == null)
				return Content("unknown");

			return Content(_user.Username);
		}
    }
}