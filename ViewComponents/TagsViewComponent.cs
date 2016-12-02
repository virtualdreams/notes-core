using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;

namespace notes.ViewComponents
{
    public class TagsViewComponent : BaseViewComponent
    {
        public readonly NoteService NoteService;
        private readonly UserService UserService;

        public TagsViewComponent(NoteService note, UserService user)
            : base(user)
        {
            NoteService = note;
            UserService = user;
        }

        public IViewComponentResult Invoke()
        {
            var _tags = NoteService.Tags(UserId);

            return View(_tags);
        }
    }
}