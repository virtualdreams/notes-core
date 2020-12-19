using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Interfaces;
using notes.Models;

namespace notes.ViewComponents
{
	public class NotebooksViewComponent : BaseViewComponent
	{
		private IMapper Mapper;
		private readonly INoteService NoteService;
		private readonly IUserService UserService;

		public NotebooksViewComponent(IMapper mapper, INoteService note, IUserService user)
			: base(user)
		{
			Mapper = mapper;
			NoteService = note;
			UserService = user;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var _notebooks = await NoteService.GetMostUsedNotebooksAsync(10);

			var notebooks = Mapper.Map<IEnumerable<DistinctAndCountModel>>(_notebooks);

			return View(notebooks);
		}
	}
}