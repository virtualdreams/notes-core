using notes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace notes.ViewComponents
{
    public class NotesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<NoteModel> model)
		{
			return View(model);
		}
    }
}