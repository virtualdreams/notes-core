using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Core;
using notes.Extensions;
using notes.Models;

namespace notes.Controllers
{
	[Authorize]
	public class UserController : BaseController
	{
		private readonly IMapper Mapper;
		private readonly Settings Options;
		private readonly UserService UserService;

		public UserController(IMapper mapper, IOptionsSnapshot<Settings> settings, UserService user)
			: base(user)
		{
			Mapper = mapper;
			Options = settings.Value;
			UserService = user;
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
		public async Task<IActionResult> Login(LoginModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				try
				{
					// if no accounts exists, create the first user as administrator.
					// HACK!
					if (!(await UserService.HasUsersAsync()))
					{
						await UserService.CreateAsync(model.Username, model.Password, "Administrator", "Administrator", true, Options.PageSize);
					}

					var _user = await UserService.LoginAsync(model.Username, model.Password);
					if (_user == null || !_user.Enabled)
						throw new NotesLoginFailedException();

					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, _user.Username, ClaimValueTypes.String),
						new Claim(ClaimTypes.Role, _user.Role, ClaimValueTypes.String)
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

			return View("Login", model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View("Forgot_Password");
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword(PasswdForgotPostModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await UserService.ForgotPasswordAsync(model.Username, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}");

					return View("Forgot_Confirmation");
				}
				catch (NotesException ex)
				{
					ModelState.AddModelError("error", ex.Message);
				}
			}

			return View("Forgot_Confirmation");
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

				return View("Reset_Password", view);
			}
			catch (NotesException ex)
			{
				ModelState.AddModelError("error", ex.Message);
			}

			return View("Forgot_Password");
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword(string id, PasswdResetPostModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var _user = await UserService.GetByTokenAsync(id);
					if (_user == null)
						throw new NotesInvalidTokenException();

					await UserService.UpdatePasswordAsync(_user.Id, model.NewPassword);
					await UserService.RemoveTokenAsync(id);

					return RedirectToAction("Login");
				}
				catch (NotesInvalidTokenException ex)
				{
					ModelState.AddModelError("error", ex.Message);

					return View("Forgot_Password");
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

			return View("Reset_Password", view);
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
			if (ModelState.IsValid)
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
			if (!ModelState.IsValid)
			{
				var view = new UserSettingsEditContainer
				{
					Settings = new UserSettingsModel
					{
						DisplayName = model.DisplayName,
						Items = model.Items
					}
				};

				return View(view);
				// return RedirectToAction("settings");
			}

			await UserService.UpdateSettingsAsync(CurrentUser.Id, model.DisplayName, model.Items);

			return RedirectToAction("settings", "user");
		}
		#endregion
	}
}