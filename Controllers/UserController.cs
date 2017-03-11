using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Security.Claims;
using notes.Core.Services;
using notes.Helper;
using notes.Models;

namespace notes.Controllers
{
    [Authorize]
	public class UserController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly UserService UserService;
		private readonly IOptions<Settings> Options;

		public UserController(IMapper mapper, UserService user, IOptions<Settings> options)
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
			if(ModelState.IsValid)
			{
				try
				{
					// if no accounts exists, create the first user as administrator.
					// HACK!
					if(!UserService.HasUsers())
					{
						UserService.Create(model.Username, model.Password, "Administrator", "Administrator", true);
					}

					var _user = UserService.Login(model.Username, model.Password);
					if(_user == null || !_user.Enabled)
						throw new NotesLoginFailedException();

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

					// return to target page.
					if (Url.IsLocalUrl(returnUrl))
						return Redirect(returnUrl);
					else
						return RedirectToAction("index", "home");
				}
				catch(NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			return View("Login", model);
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
			if(ModelState.IsValid)
			{
				try
				{
					UserService.ForgotPassword(model.Username, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}");

					return View("Forgot_Confirmation");
				}
				catch(NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			return View();
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
			if(ModelState.IsValid)
			{
				try
				{
					if(!model.NewPassword.Equals(model.ConfirmPassword))
						throw new NotesPasswordMismatchException();

					var _user = UserService.GetUserByToken(model.Token);
					if(_user == null)
						return NotFound();

					UserService.UpdatePassword(_user.Id, model.NewPassword);
					UserService.RemoveToken(model.Token);

					return RedirectToAction("Login");
				}
				catch(NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			var view = new PasswdResetModel {
				Token = model.Token
			};

			return View(view);
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
		public IActionResult Security(PasswdChangePostModel model)
		{
			if(ModelState.IsValid)
			{
				try
				{
					var _user = UserService.Login(User.GetUserName(), model.OldPassword);
					if(_user == null)
						throw new NotesPasswordIncorrectException();

					if(!model.NewPassword.Equals(model.ConfirmPassword))
						throw new NotesPasswordMismatchException();

					UserService.UpdatePassword(UserId, model.NewPassword);

					// redirect to home
					return RedirectToAction("settings", "user");
				}
				catch(NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			return View();
		}

		[Authorize]
		[HttpGet]
		public IActionResult Settings()
		{
			var view = new UserSettingsEditContainer
			{
				Profile = new UserProfileModel
				{
					DisplayName = UserService.GetUserById(UserId)?.DisplayName
				},
				Settings = new UserSettingsModel
				{
					Items = UserSettings?.PageSize ?? Options.Value.PageSize
				}
			};

			return View(view);
		}

		[HttpPost]
		public IActionResult Settings(UserSettingsPostModel model)
		{
			if(!ModelState.IsValid)
			{
				return RedirectToAction("settings");
			}

			UserService.UpdateSettings(UserId, model.Items);

			return RedirectToAction("settings");
		}

		[HttpPost]
		public IActionResult Profile(UserProfilePostModel model)
		{
			if(!ModelState.IsValid)
			{
				return RedirectToAction("settings");
			}

			UserService.UpdateProfile(UserId, model.DisplayName);

			return RedirectToAction("settings");
		}
#endregion
    }
}