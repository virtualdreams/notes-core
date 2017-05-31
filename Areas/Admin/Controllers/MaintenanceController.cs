using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using notes.Controllers;
using notes.Core.Services;

namespace notes.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Policy = "AdministratorOnly")]
	public class MaintenanceController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly Settings Options;
		private readonly UserService UserService;
		private readonly MaintenanceService MaintenanceService;

		public MaintenanceController(IMapper mapper, Settings settings, UserService user, MaintenanceService maintenance)
			: base(user)
		{
			Mapper = mapper;
			Options = settings;
			UserService = user;
			MaintenanceService = maintenance;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult RecreateIndexes()
		{
			MaintenanceService.CreateIndexes();

			return RedirectToAction("Index");
		}
	}
}