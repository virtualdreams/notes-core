using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;
using notes.Helper;

namespace notes.ViewComponents
{
    public class TagsViewComponent : ViewComponent
    {
        public readonly NoteService NoteService;
        private readonly UserService UserService;

        public TagsViewComponent(NoteService note, UserService user)
        {
            NoteService = note;
            UserService = user;
        }

        public IViewComponentResult Invoke()
        {
            var _user = UserService.GetByName((User as ClaimsPrincipal).GetUserName());
            var _tags = NoteService.Tags(_user.Id);

            return View(_tags);
        }
    }
}