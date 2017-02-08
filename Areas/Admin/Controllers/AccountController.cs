using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System;
using notes.Controllers;
using notes.Core.Services;
using notes.Models;
using Microsoft.Extensions.Options;

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
            var _users = UserService.Get();

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
			if(!ModelState.IsValid)
			{
				var view = new UserEditContainer
				{
					User = new UserModel
					{
						Id = model.Id,
						Username = model.Username,
						Role = String.Empty	
					}
				};

				return View(view);
			}

			if(model.Id == ObjectId.Empty)
			{
				UserService.Create(model.Username, model.Password, "User");
			}

			return RedirectToAction("Index");
		}
    }
}