using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using Notes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.ViewComponents
{
	public class NotebooksViewComponent : BaseViewComponent
	{
		private IMapper Mapper;

		private readonly INoteService NoteService;

		private readonly IUserService UserService;

		public NotebooksViewComponent(
			IMapper mapper,
			INoteService note,
			IUserService user)
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