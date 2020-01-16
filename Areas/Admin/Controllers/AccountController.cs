using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Areas.Admin.Models;
using notes.Controllers;
using notes.Core.Services;

namespace notes.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Policy = "AdministratorOnly")]
	public class AccountController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly Settings Options;
		private readonly UserService UserService;

		public AccountController(IMapper mapper, IOptionsSnapshot<Settings> settings, UserService user)
			: base(user)
		{
			Mapper = mapper;
			Options = settings.Value;
			UserService = user;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var _users = await UserService.GetUsers();

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
			var _user = await UserService.GetById(id);
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
						await UserService.Create(model.Username, model.Password, model.DisplayName, model.Role, model.Enabled, Options.PageSize);
					}
					else
					{
						await UserService.Update(model.Id, model.Username, model.Password, model.DisplayName, model.Role, model.Enabled);
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
			var _user = await UserService.GetById(id);
			if (_user == null)
				return NotFound();

			try
			{
				await UserService.Delete(id);
			}
			catch (NotesException ex)
			{
				return new JsonResult(new { Success = false, Error = ex.Message });
			}

			return new NoContentResult();
		}
	}
}