using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Helper;

namespace notes.Extensions
{
	static public class CookieValidator
	{
		/// <summary>
		/// Validate username from cookie.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		static public async Task ValidateAsync(CookieValidatePrincipalContext context)
		{
			var _userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
			var _username = context.Principal.GetUserName();
			var _user = await _userService.GetByName(_username);

			// if the user not exists or the user is disabled or his role has changed, reject his login
			if (_user == null || !_user.Enabled || !_user.Role.Equals(context.Principal.GetUserRole()))
			{
				context.RejectPrincipal();
				await AuthenticationHttpContextExtensions.SignOutAsync(context.HttpContext);
			}
		}
	}
}