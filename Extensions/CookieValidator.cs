using Microsoft.AspNetCore.Authentication.Cookies;
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
			var _user = _userService.GetByName(_username);
			
			if(_user == null)
			{
				context.RejectPrincipal();
        		await context.HttpContext.Authentication.SignOutAsync("notes");
			}
		}
	}
}