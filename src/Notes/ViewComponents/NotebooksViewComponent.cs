using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using Notes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.ViewComponents
{
	public class NotebooksViewComponent : ViewComponent
	{
		private IMapper Mapper;

		private readonly INoteService NoteService;

		public NotebooksViewComponent(
			IMapper mapper,
			INoteService note)
		{
			Mapper = mapper;
			NoteService = note;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var _notebooks = await NoteService.GetMostUsedNotebooksAsync(10);

			var notebooks = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_notebooks);

			return View(notebooks);
		}
	}
}