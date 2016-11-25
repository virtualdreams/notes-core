using Microsoft.AspNetCore.Mvc;
using postit.Core.Services;

namespace postit.ViewComponents
{
    public class NotebooksViewComponent : ViewComponent
    {
        public readonly PostitService PostitService;

        public NotebooksViewComponent(PostitService postit)
        {
            PostitService = postit;
        }

        public IViewComponentResult Invoke()
        {
            var _notebooks = PostitService.Notebooks();

            return View(_notebooks);
        }
    }
}