using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Models;

namespace notes.ViewComponents
{
	public class NotebooksViewComponent : BaseViewComponent
	{
		private IMapper Mapper;
		private readonly NoteService NoteService;
		private readonly UserService UserService;

		public NotebooksViewComponent(IMapper mapper, NoteService note, UserService user)
			: base(user)
		{
			Mapper = mapper;
			NoteService = note;
			UserService = user;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var _notebooks = await NoteService.GetMostUsedNotebooks(UserId);

			var notebooks = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_notebooks);

			return View(notebooks);
		}
	}
}