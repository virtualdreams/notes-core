using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using postit.Core.Services;
using postit.Helper;

namespace postit.ViewComponents
{
    public class NotebooksViewComponent : ViewComponent
    {
        public readonly PostitService PostitService;
        private readonly UserService UserService;

        public NotebooksViewComponent(PostitService postit, UserService user)
        {
            PostitService = postit;
            UserService = user;
        }

        public IViewComponentResult Invoke()
        {
            var _user = UserService.GetByName((User as ClaimsPrincipal).GetUserName());
            var _notebooks = PostitService.Notebooks(_user.Id);

            return View(_notebooks);
        }
    }
}