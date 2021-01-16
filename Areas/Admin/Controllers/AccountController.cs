using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Areas.Admin.Models;
using notes.Controllers;
using notes.Core.Interfaces;
using notes.Core;
using notes.Options;

namespace notes.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Policy = "AdministratorOnly")]
	public class AccountController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly AppSettings AppSetings;
		private readonly IUserService UserService;

		public AccountController(IMapper mapper, IOptionsSnapshot<AppSettings> settings, IUserService user)
			: base(user)
		{
			Mapper = mapper;
			AppSetings = settings.Value;
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
						await UserService.CreateAsync(model.Username, model.Password, model.DisplayName, model.Role, model.Enabled, AppSetings.PageSize);
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