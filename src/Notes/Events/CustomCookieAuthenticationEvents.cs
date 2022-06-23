using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Notes.Core.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Notes.Events
{
	public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
	{
		private readonly ILogger<CustomCookieAuthenticationEvents> Log;
		private readonly IUserService UserService;

		public CustomCookieAuthenticationEvents(ILogger<CustomCookieAuthenticationEvents> log, IUserService user)
		{
			Log = log;
			UserService = user;
		}

		public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
		{
			var _principal = context.Principal;
			var _username = _principal.Identity.Name;
			var _serialString = _principal.FindFirst(ClaimTypes.SerialNumber)?.Value;
			int.TryParse(_serialString, out var _serial);

			// get the user object from database.
			var _user = await UserService.GetByNameAsync(_username);

			// if the user not exists or the user is disabled or his role has changed, reject his login
			if (_user == null || _user.Version != _serial)
			{
				Log.LogInformation($"User {_username} rejected.");

				context.RejectPrincipal();
				await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			}
		}

		public override async Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
		{
			// get the original uri
			var uri = context.RedirectUri;
			UriHelper.FromAbsolute(uri, out var scheme, out var host, out var path, out var query, out var fragment);

			// build absolute uri or relative uri
			//uri = UriHelper.BuildAbsolute(scheme, host, path);
			uri = UriHelper.BuildRelative(path);

			// redirect to the new uri
			context.Response.Redirect(uri);

			await Task.CompletedTask;
		}
	}
}