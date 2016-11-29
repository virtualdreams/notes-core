using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;
using notes.Helper;

namespace notes.ViewComponents
{
    public class NotebooksViewComponent : ViewComponent
    {
        public readonly NoteService NoteService;
        private readonly UserService UserService;

        public NotebooksViewComponent(NoteService note, UserService user)
        {
            NoteService = note;
            UserService = user;
        }

        public IViewComponentResult Invoke()
        {
            var _user = UserService.GetByName((User as ClaimsPrincipal).GetUserName());
            var _notebooks = NoteService.Notebooks(_user.Id);

            return View(_notebooks);
        }
    }
}