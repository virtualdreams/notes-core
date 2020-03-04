using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Helper;

namespace notes.Extensions
{
	public class CookieValidator : CookieAuthenticationEvents
	{
		private readonly ILogger<CookieValidator> Log;
		private readonly UserService UserService;

		public CookieValidator(ILogger<CookieValidator> log, UserService user)
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
			var _user = await UserService.GetByName(_username);

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