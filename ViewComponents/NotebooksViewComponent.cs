using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;

namespace notes.ViewComponents
{
	public class NotebooksViewComponent : BaseViewComponent
	{
		public readonly NoteService NoteService;
		private readonly UserService UserService;

		public NotebooksViewComponent(NoteService note, UserService user)
			: base(user)
		{
			NoteService = note;
			UserService = user;
		}

		public IViewComponentResult Invoke()
		{
			var _notebooks = NoteService.GetMostlyUsedNotebooks(UserId);

			return View(_notebooks);
		}
	}
}