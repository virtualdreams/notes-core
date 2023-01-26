using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using Notes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.ViewComponents
{
	public class TagsViewComponent : ViewComponent
	{
		private IMapper Mapper;

		private readonly INoteService NoteService;

		public TagsViewComponent(
			IMapper mapper,
			INoteService note)
		{
			Mapper = mapper;
			NoteService = note;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var _tags = await NoteService.GetMostUsedTagsAsync(10);

			var tags = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_tags);

			return View(tags);
		}
	}
}