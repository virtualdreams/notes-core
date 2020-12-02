using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using notes.Core.Services;

namespace notes.Events
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
	}
}