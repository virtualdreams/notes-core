using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notes.Core.Interfaces;
using Notes.Core;
using Notes.Extensions;
using Notes.FluentValidation;
using Notes.Models;
using Notes.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Notes.Controllers
{
	[Authorize]
	public class UserController : BaseController
	{
		private readonly ILogger<UserController> Log;

		private readonly IMapper Mapper;

		private readonly AppSettings AppSettings;

		private readonly IUserService UserService;

		private readonly IValidator<LoginModel> LoginModelValidator;

		private readonly IValidator<PasswdForgotPostModel> PasswdForgotPostModelValidator;

		private readonly IValidator<PasswdResetPostModel> PasswdResetPostModelValidator;

		private readonly IValidator<PasswdChangePostModel> PasswdChangePostModelValidator;

		private readonly IValidator<UserSettingsPostModel> UserSettingsPostModelValidator;

		public UserController(
			ILogger<UserController> log,
			IMapper mapper,
			IOptionsSnapshot<AppSettings> appSettings,
			IUserService user,
			IValidator<LoginModel> loginModelValidator,
			IValidator<PasswdForgotPostModel> passwdForgotPostModelValidator,
			IValidator<PasswdResetPostModel> passwdResetPostModelValidator,
			IValidator<PasswdChangePostModel> passwdChangePostModelValidator,
			IValidator<UserSettingsPostModel> userSettingsPostModelValidator)
			: base(user)
		{
			Log = log;
			Mapper = mapper;
			AppSettings = appSettings.Value;
			UserService = user;
			LoginModelValidator = loginModelValidator;
			PasswdForgotPostModelValidator = passwdForgotPostModelValidator;
			PasswdResetPostModelValidator = passwdResetPostModelValidator;
			PasswdChangePostModelValidator = passwdChangePostModelValidator;
			UserSettingsPostModelValidator = userSettingsPostModelValidator;
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Login()
		{
			var view = new LoginModel();

			// This shows a warning if no administrative account exists.
			// HACK!
			if (!(await UserService.HasUsersAsync()))
			{
				ModelState.AddModelError("warning", "The first user is automatically created as an administrative account.");
			}

			return View(view);
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl)
		{
			var _result = await LoginModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				try
				{
					// If there are no administrative accounts yet, the first user is created as an administrative account.
					// HACK!
					if (!(await UserService.HasUsersAsync()))
					{
						await UserService.CreateAsync(model.Username, model.Password, "Administrator", "Administrator", true, AppSettings.PageSize);
					}

					var _user = await UserService.LoginAsync(model.Username, model.Password);
					if (_user == null || !_user.Enabled)
						throw new NotesLoginFailedException();

					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, _user.Username, ClaimValueTypes.String),
						new Claim(ClaimTypes.Role, _user.Role, ClaimValueTypes.String),
						new Claim(ClaimTypes.SerialNumber, _user.Version.ToString())
					};

					var _identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var _principal = new ClaimsPrincipal(_identity);

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, _principal,
						new AuthenticationProperties
						{
							IsPersistent = model.Remember,
							AllowRefresh = model.Remember
						}
					);

					// return to target page.
					if (Url.IsLocalUrl(returnUrl))
						return Redirect(returnUrl);
					else
						return RedirectToAction("index", "home");
				}
				catch (NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			_result.AddToModelState(ModelState);
			return View("Login", model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword(PasswdForgotPostModel model)
		{
			var _result = await PasswdForgotPostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				try
				{
					await UserService.ForgotPasswordAsync(model.Username, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}");

					return View("ForgotConfirmation");
				}
				catch (NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			_result.AddToModelState(ModelState);
			return View("ForgotConfirmation");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword(string id)
		{
			try
			{
				var _user = await UserService.GetByTokenAsync(id);

				if (_user == null)
					throw new NotesInvalidTokenException();

				var view = new PasswdResetModel
				{
					Token = id
				};

				return View(view);
			}
			catch (NotesException ex)
			{
				ModelState.AddModelError("error", ex.Message);
			}

			return View("ForgotPassword");
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword(string id, PasswdResetPostModel model)
		{
			var _result = await PasswdResetPostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				try
				{
					var _user = await UserService.GetByTokenAsync(id);

					if (_user == null)
						throw new NotesInvalidTokenException();

					await UserService.UpdatePasswordAsync(_user.Id, model.NewPassword);

					return RedirectToAction("Login");
				}
				catch (NotesInvalidTokenException ex)
				{
					ModelState.AddModelError("error", ex.Message);

					return View("ForgotPassword");
				}
				catch (NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			var view = new PasswdResetModel
			{
				Token = id
			};

			_result.AddToModelState(ModelState);
			return View(view);
		}

		#region User
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("index", "home");
		}

		[Authorize]
		[HttpGet]
		public IActionResult Security(int id)
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Security(PasswdChangePostModel model)
		{
			var _result = await PasswdChangePostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				try
				{
					var _user = await UserService.LoginAsync(User.GetUserName(), model.OldPassword);
					if (_user == null)
						throw new NotesPasswordIncorrectException();

					await UserService.UpdatePasswordAsync(CurrentUser.Id, model.NewPassword);

					// redirect to home
					return RedirectToAction("settings", "user");
				}
				catch (NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			_result.AddToModelState(ModelState);
			return View();
		}

		[Authorize]
		[HttpGet]
		public IActionResult Settings()
		{
			var view = new UserSettingsEditContainer
			{
				Settings = new UserSettingsModel
				{
					DisplayName = CurrentUser.DisplayName,
					Items = PageSize
				}
			};

			return View(view);
		}

		[HttpPost]
		public async Task<IActionResult> Settings(UserSettingsPostModel model)
		{
			var _result = await UserSettingsPostModelValidator.ValidateAsync(model);
			if (_result.IsValid)
			{
				await UserService.UpdateSettingsAsync(CurrentUser.Id, model.DisplayName, model.Items);

				return RedirectToAction("settings", "user");
			}

			var view = new UserSettingsEditContainer
			{
				Settings = new UserSettingsModel
				{
					DisplayName = model.DisplayName,
					Items = model.Items
				}
			};

			_result.AddToModelState(ModelState);
			return View(view);
		}
		#endregion
	}
}