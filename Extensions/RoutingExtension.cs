using Microsoft.AspNetCore.Builder;

namespace notes.Extensions
{
	static public class RoutingExtensions
	{
		static public IApplicationBuilder AddRoutes(this IApplicationBuilder app)
		{
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "search",
					template: "search",
					defaults: new { controller = "Home", action = "Search" }
				);

				routes.MapRoute(
					name: "tags",
					template: "tags",
					defaults: new { controller = "Note", action = "Tags" }
				);

				routes.MapRoute(
					name: "notebooks",
					template: "notebooks",
					defaults: new { controller = "Note", action = "Notebooks" }
				);

				routes.MapRoute(
					name: "tag",
					template: "tag/{id?}",
					defaults: new { controller = "Note", action = "Tag" }
				);

				routes.MapRoute(
					name: "notebook",
					template: "notebook/{id?}",
					defaults: new { controller = "Note", action = "Notebook" }
				);

				routes.MapRoute(
					name: "trash",
					template: "trash",
					defaults: new { controller = "Note", action = "Trash" }
				);

				routes.MapRoute(
					name: "create",
					template: "create",
					defaults: new { controller = "Note", action = "Create" }
				);

				routes.MapRoute(
					name: "edit",
					template: "/note/edit/{id?}/{slug?}",
					defaults: new { controller = "Note", action = "Edit" }
				);

				routes.MapRoute(
					name: "view",
					template: "note/{id?}/{slug?}",
					defaults: new { controller = "Note", action = "View" }
				);

				routes.MapRoute(
					name: "print",
					template: "print/{id?}/{slug?}",
					defaults: new { controller = "Note", action = "Print" }
				);

				routes.MapRoute(
					name: "login",
					template: "login",
					defaults: new { controller = "user", action = "login" }
				);

				routes.MapRoute(
					name: "logout",
					template: "logout",
					defaults: new { controller = "user", action = "logout" }
				);

				routes.MapRoute(
					name: "forgot_password",
					template: "reset_password",
					defaults: new { controller = "user", action = "forgotpassword" }
				);

				routes.MapRoute(
					name: "reset_password",
					template: "reset_password/{id?}",
					defaults: new { controller = "user", action = "resetpassword" }
				);

				routes.MapRoute(
					name: "error",
					template: "error/{code?}",
					defaults: new { controller = "home", action = "error" }
				);

				routes.MapRoute(
					name: "areaRoute",
					template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				);

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}"
				);
			});

			return app;
		}
	}
}