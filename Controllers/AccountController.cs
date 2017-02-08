using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using notes.Core.Services;
using notes.Helper;
using notes.Models;

namespace notes.Controllers
{
    [Authorize]
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

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Forgot_Password()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Forgot_Password(PasswdForgotPostModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			UserService.ForgotPassword(model.Username, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}");

			return View("Forgot_Confirmation");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Reset_Password(string id)
		{
			var _user = UserService.GetUserByToken(id);
			if(_user == null)
				return RedirectToAction("Login");

			var view = new PasswdResetModel {
				Token = id
			};

			return View(view);
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Reset_Password(PasswdResetPostModel model)
		{
			if (!ModelState.IsValid || !model.NewPassword.Equals(model.ConfirmPassword))
			{
				var view = new PasswdResetModel {
					Token = model.Token
				};

				return View(view);
			}

			var _user = UserService.GetUserByToken(model.Token);
			if(_user == null)
				return new StatusCodeResult(404);
			
			UserService.SetPassword(_user.Id, model.NewPassword);
			UserService.RemoveToken(model.Token);

			return RedirectToAction("Login");
		}

#region User
		[Authorize]
		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Authentication.SignOutAsync("notes");

			return RedirectToAction("index", "home");
		}

		[Authorize]
		[HttpGet]
		public IActionResult Security(ObjectId id)
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public IActionResult Security(PasswdPostModel model)
		{
			if(!ModelState.IsValid || (UserService.Login(User.GetUserName(), model.OldPassword) == null || !model.NewPassword.Equals(model.ConfirmPassword)))
			{
				return View();
			}

			// set new password
			UserService.SetPassword(UserId, model.NewPassword);

			// force logout
			HttpContext.Authentication.SignOutAsync("notes");

			// redirect to home
			return RedirectToAction("index", "home");
		}

		[Authorize]
		[HttpGet]
		public IActionResult Settings()
		{
			var view = new SettingsEditContainer
			{
				Settings = new SettingsModel
				{
					Items = UserSettings?.PageSize ?? Options.Value.PageSize,
					Language = UserSettings?.SearchLanguage ?? "en"
				}
			};

			return View(view);
		}

		[HttpPost]
		public IActionResult Settings(SettingsPostModel model)
		{
			if(!ModelState.IsValid)
			{
				var view = new SettingsEditContainer
				{
					Settings = new SettingsModel
					{
						Items = model.Items,
						Language = model.Language
					}
				};

				return View(view);
			}

			UserService.SetSettings(UserId, model.Items, model.Language);

			return RedirectToAction("settings");
		}
#endregion

#region Admin

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
#endregion
    }
}