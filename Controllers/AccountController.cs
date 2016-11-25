using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using postit.Core.Services;
using postit.Models;

namespace postit.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserService UserService;

		public AccountController(UserService user)
		{
			UserService = user;
		}

		[HttpGet]
		public IActionResult Login()
		{
			var view = new LoginModel();

			return View(view);
		}

		[HttpPost]
		public IActionResult Login(LoginModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View("Login", model);
			}

			var _user = UserService.Login(model.Username, model.Password);
			if(_user != null && _user.Enabled)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, _user.Username, ClaimValueTypes.String),
					//new Claim(ClaimTypes.Role, _user.Role, ClaimValueTypes.String)
				};

				var _identity = new ClaimsIdentity(claims, "local");
				var _principal = new ClaimsPrincipal(_identity);

				HttpContext.Authentication.SignInAsync("postit", _principal, 
					new AuthenticationProperties {
						IsPersistent = true,
						AllowRefresh = true
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
		public IActionResult Logout()
		{
			HttpContext.Authentication.SignOutAsync("postit");

			return RedirectToAction("index", "home");
		}
	}
}