using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using notes.Core.Services;
using notes.Models;

namespace notes.Controllers
{
    [Authorize]
	public class AccountController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Settings;

		public AccountController(IMapper mapper, UserService user, IOptions<Settings> settings)
			: base(user)
		{
			Mapper = mapper;
			UserService = user;
			Settings = settings;
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login()
		{
			var view = new LoginModel();

			return View(view);
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult Login(LoginModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View("Login", model);
			}

			if(!UserService.HasUsers())
			{
				UserService.Create(model.Username, model.Password, "Administrator");
			}

			var _user = UserService.Login(model.Username, model.Password);
			if(_user != null && _user.Enabled)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, _user.Username, ClaimValueTypes.String),
					new Claim(ClaimTypes.Role, _user.Role, ClaimValueTypes.String)
				};

				var _identity = new ClaimsIdentity(claims, "local");
				var _principal = new ClaimsPrincipal(_identity);

				HttpContext.Authentication.SignInAsync("notes", _principal, 
					new AuthenticationProperties {
						IsPersistent = model.Remember,
						AllowRefresh = model.Remember
					}
				).Wait();
			}
			else
			{
				ModelState.AddModelError("failed", "Benutzername oder Passwort falsch.");
				return View("Login", model);
			}

			// return to target page.
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("index", "home");
			}
		}

		[Authorize]
		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Authentication.SignOutAsync("notes");

			return RedirectToAction("index", "home");
		}

		[Authorize(Policy = "AdministratorOnly")]
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

		[Authorize(Policy = "AdministratorOnly")]
		[HttpGet]
		public IActionResult Create()
		{
			var view = new UserEditContainer
			{
				User = new UserModel()
			};
			
			return View("Edit", view);
		}

		[Authorize(Policy = "AdministratorOnly")]
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

		[Authorize(Policy = "AdministratorOnly")]
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

			return RedirectToAction("index");
		}

		[HttpGet]
		public IActionResult ChangePasswd(ObjectId id)
		{
			return View();
		}

		[HttpPost]
		public IActionResult ChangePasswd(PasswdPostModel model)
		{
			if(!ModelState.IsValid || !model.Password.Equals(model.PasswordRepeat))
			{
				return View();
			}

			// set new password
			UserService.SetPassword(UserId, model.Password);

			// force logout
			HttpContext.Authentication.SignOutAsync("notes");

			// redirect to home
			return RedirectToAction("index", "home");
		}
    }
}