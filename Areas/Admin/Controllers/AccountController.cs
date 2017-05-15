using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using notes.Areas.Admin.Models;
using notes.Controllers;
using notes.Core.Services;
using notes.Models;

namespace notes.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Policy = "AdministratorOnly")]
    public class AccountController : BaseController
    {
		private readonly IMapper Mapper;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Options;

        public AccountController(IMapper mapper, UserService user, IOptions<Settings> options)
			: base(user)
        {
			Mapper = mapper;
			UserService = user;
			Options = options;
        }

		[HttpGet]
        public IActionResult Index()
        {
            var _users = UserService.GetUsers();

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
		public IActionResult Edit(ObjectId id)
		{
			var _user = UserService.GetById(id);
			if(_user == null)
				return NotFound();

			var user = Mapper.Map<UserModel>(_user);

			var view = new UserEditContainer
			{
				User = user
			};

			return View(view);
		}

		[HttpPost]
		public IActionResult Edit(UserPostModel model)
		{
			if(ModelState.IsValid)
			{
				try
				{
					if(model.Id == ObjectId.Empty)
					{
						UserService.Create(model.Username, model.Password, model.DisplayName, model.Role, model.Enabled);
					}
					else
					{
						UserService.Update(model.Id, model.Username, model.Password, model.DisplayName, model.Role, model.Enabled);
					}

					return RedirectToAction("Index");
				}
				catch(NotesException ex)
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
		public IActionResult Delete(ObjectId id)
		{
			var _user = UserService.GetById(id);
			if(_user == null)
				return NotFound();
			
			try
			{
				UserService.Delete(id);
			}
			catch(NotesException ex)
			{
				return new JsonResult(new { Success = false, Error = ex.Message});
			}

			return new NoContentResult();
		}
    }
}