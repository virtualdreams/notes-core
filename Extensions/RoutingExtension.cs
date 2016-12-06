using Microsoft.AspNetCore.Builder;

namespace notes.Extensions
{
	static public class RoutingExtensions
	{
		static public IApplicationBuilder AddRoutes(this IApplicationBuilder app)
		{
			app.UseMvc(routes => {
				routes.MapRoute(
					name: "search",
					template: "search/{q?}",
					defaults: new { controller = "Home", action = "Search" }
				);

				routes.MapRoute(
					name: "tag",
					template: "tag/{id}",
					defaults: new { controller = "Note", action = "Tag" }
				);

				routes.MapRoute(
					name: "notebook",
					template: "notebook/{id}",
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
					name: "note",
					template: "note/{id?}/{slug?}",
					defaults: new { controller = "Note", action = "View"},
					constraints: new { id = @"^[a-f0-9]{24}$" } 
				);

				routes.MapRoute(
					name: "login",
					template: "login",
					defaults: new { controller = "account", action = "login"}
				);

				routes.MapRoute(
					name: "logout",
					template: "logout",
					defaults: new { controller = "account", action = "logout"}
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