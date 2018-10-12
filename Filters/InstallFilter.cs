using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using notes.Core.Services;

namespace notes.Filters
{
	public class InstallAttribute : ResultFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext context)
		{
			// test for users
			var _userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
			var _result = _userService.HasUsers().Result;

			// test for file
			/* if (File.Exists("installed.txt"))
			{
				context.Result = new NotFoundResult();
			} */

			base.OnResultExecuting(context);
		}
	}
}