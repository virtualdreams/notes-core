using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using notes.Core.Services;

namespace notes.Extensions
{
	public class CustomCookieEvents : CookieAuthenticationEvents
	{
		private readonly ILogger<CustomCookieEvents> Log;
		private readonly UserService UserService;

		public CustomCookieEvents(ILogger<CustomCookieEvents> log, UserService user)
		{
			Log = log;
			UserService = user;
		}

		public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
		{
			var _principal = context.Principal;

			// get username from claims.
			var _username = _principal.GetUserName();

			// get the user object from database.
			var _user = await UserService.GetByNameAsync(_username);

			// if the user not exists or the user is disabled or his role has changed, reject his login
			if (_user == null || !_user.Enabled || !_user.Role.Equals(context.Principal.GetUserRole()))
			{
				Log.LogInformation($"User {_username} rejected.");

				context.RejectPrincipal();
				await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			}
		}
	}
}