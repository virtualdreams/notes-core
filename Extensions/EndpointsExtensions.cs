using Microsoft.AspNetCore.Builder;

namespace notes.Extensions
{
	public static class EndpointsExtensions
	{
		/// <summary>
		/// Adds endpoints.
		/// </summary>
		/// <param name="app">IApplicationBuilder</param>
		/// <returns></returns>
		public static IApplicationBuilder AddEndpoints(this IApplicationBuilder app)
		{
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "search",
					pattern: "search",
					defaults: new { controller = "Home", action = "Search" }
				);

				endpoints.MapControllerRoute(
					name: "tags",
					pattern: "tags",
					defaults: new { controller = "Note", action = "Tags" }
				);

				endpoints.MapControllerRoute(
					name: "notebooks",
					pattern: "notebooks",
					defaults: new { controller = "Note", action = "Notebooks" }
				);

				endpoints.MapControllerRoute(
					name: "tag",
					pattern: "tag/{id?}",
					defaults: new { controller = "Note", action = "Tag" }
				);

				endpoints.MapControllerRoute(
					name: "notebook",
					pattern: "notebook/{id?}",
					defaults: new { controller = "Note", action = "Notebook" }
				);

				endpoints.MapControllerRoute(
					name: "trash",
					pattern: "trash",
					defaults: new { controller = "Note", action = "Trash" }
				);

				endpoints.MapControllerRoute(
					name: "create",
					pattern: "create",
					defaults: new { controller = "Note", action = "Create" }
				);

				endpoints.MapControllerRoute(
					name: "edit",
					pattern: "/note/edit/{id?}/{slug?}",
					defaults: new { controller = "Note", action = "Edit" }
				);

				endpoints.MapControllerRoute(
					name: "view",
					pattern: "note/{id?}/{slug?}",
					defaults: new { controller = "Note", action = "View" }
				);

				endpoints.MapControllerRoute(
					name: "print",
					pattern: "print/{id?}/{slug?}",
					defaults: new { controller = "Note", action = "Print" }
				);

				endpoints.MapControllerRoute(
					name: "revisions",
					pattern: "revision/{id?}",
					defaults: new { controller = "Revision", action = "Index" }
				);

				endpoints.MapControllerRoute(
					name: "login",
					pattern: "login",
					defaults: new { controller = "user", action = "login" }
				);

				endpoints.MapControllerRoute(
					name: "logout",
					pattern: "logout",
					defaults: new { controller = "user", action = "logout" }
				);

				endpoints.MapControllerRoute(
					name: "forgot_password",
					pattern: "reset_password",
					defaults: new { controller = "user", action = "forgotpassword" }
				);

				endpoints.MapControllerRoute(
					name: "reset_password",
					pattern: "reset_password/{id?}",
					defaults: new { controller = "user", action = "resetpassword" }
				);

				endpoints.MapControllerRoute(
					name: "error",
					pattern: "error/{code?}",
					defaults: new { controller = "home", action = "error" }
				);

				endpoints.MapControllerRoute(
					name: "areaRoute",
					pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				);

				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}"
				);
			});

			return app;
		}
	}
}