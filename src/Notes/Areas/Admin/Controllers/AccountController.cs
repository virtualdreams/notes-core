using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notes.Areas.Admin.Models;
using Notes.Controllers;
using Notes.Core.Interfaces;
using Notes.Core;
using Notes.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Policy = "AdministratorOnly")]
	public class AccountController : BaseController
	{
		private readonly ILogger<AccountController> Log;

		private readonly IMapper Mapper;

		private readonly AppSettings AppSettings;

		private readonly IUserService UserService;

		public AccountController(
			ILogger<AccountController> log,
			IMapper mapper,
			IOptionsSnapshot<AppSettings> appSettings,
			IUserService user)
			: base(user)
		{
			Log = log;
			Mapper = mapper;
			AppSettings = appSettings.Value;
			UserService = user;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var _users = await UserService.GetUsersAsync();

			var users = Mapper.Map<IEnumerable<UserModel>>(_users);

			var view = new UserListContainer
			{
				Users = users
			};

			return View(view);
		}

		[HttpGet]
		public IActionResult Create()
		{
			var view = new UserEditContainer
			{
				User = new UserModel()
			};

			return View("Edit", view);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var _user = await UserService.GetByIdAsync(id);
			if (_user == null)
				return NotFound();

			var user = Mapper.Map<UserModel>(_user);

			var view = new UserEditContainer
			{
				User = user
			};

			return View(view);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(UserPostModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (model.Id == 0)
					{
						await UserService.CreateAsync(model.Username, model.Password, model.DisplayName, model.Role, model.Enabled, AppSettings.PageSize);
					}
					else
					{
						await UserService.UpdateAsync(model.Id, model.Username, model.Password, model.DisplayName, model.Role, model.Enabled);
					}

					return RedirectToAction("Index");
				}
				catch (NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			// validation failed
			var view = new UserEditContainer
			{
				User = new UserModel
				{
					Id = model.Id,
					Username = model.Username,
					DisplayName = model.DisplayName,
					Role = model.Role,
					Enabled = model.Enabled
				}
			};

			return View(view);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var _user = await UserService.GetByIdAsync(id);
			if (_user == null)
				return NotFound();

			try
			{
				await UserService.DeleteAsync(id);
			}
			catch (NotesException ex)
			{
				return new JsonResult(new { Success = false, Error = ex.Message });
			}

			return new NoContentResult();
		}
	}
}