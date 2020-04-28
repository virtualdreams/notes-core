using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Models;

namespace notes.ViewComponents
{
	public class TagsViewComponent : BaseViewComponent
	{
		private IMapper Mapper;
		private readonly NoteService NoteService;
		private readonly UserService UserService;

		public TagsViewComponent(IMapper mapper, NoteService note, UserService user)
			: base(user)
		{
			Mapper = mapper;
			NoteService = note;
			UserService = user;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var _tags = await NoteService.GetMostUsedTagsAsync(10);

			var tags = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_tags);

			return View(tags);
		}
	}
}