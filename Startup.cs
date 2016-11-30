using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using notes.Core.Models;
using notes.Core.Services;
using notes.Helper;
using notes.ModelBinders;
using notes.Models;

namespace notes
{
    public class Startup
	{
		public IConfigurationRoot Configuration { get; set; }

		public Startup(IHostingEnvironment env, ILoggerFactory logger)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging();
			services.AddMvc(options => {
				options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
			});

			services.AddOptions();
			services.Configure<Settings>(Configuration.GetSection("Settings"));

			services.AddAuthorization(options => {
				options.AddPolicy("AdministratorOnly", policy => {
					policy.RequireRole("Administrator");
				});
			});

			services.AddScoped<MongoContext>();
			services.AddTransient<NoteService>();
			services.AddTransient<UserService>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
		{
			logger.AddConsole();

			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseCookieAuthentication(new CookieAuthenticationOptions(){
				AuthenticationScheme = "notes",
				CookieName = "notes",
				LoginPath = new PathString("/login"),
				AccessDeniedPath = new PathString("/login"),
				AutomaticAuthenticate = true,
				AutomaticChallenge = true
			});

			app.UseMvc(routes => {

				routes.MapRoute(
					name: "search",
					template: "search/{q?}",
					defaults: new { controller = "Home", action = "Search" }
				);

				routes.MapRoute(
					name: "tag",
					template: "tag/{id}",
					defaults: new { controller = "Tag", action = "View" }
				);

				routes.MapRoute(
					name: "notebook",
					template: "notebook/{id}",
					defaults: new { controller = "Notebook", action = "View" }
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

			InitializeAutoMapper();
		}

		private void InitializeAutoMapper()
		{
			Mapper.Initialize(config => {
				config.CreateMap<Note, NoteModel>()
					.ForMember(d => d.Tags, map => map.MapFrom(s => String.Join(", ", s.Tags ?? new string[] {})))
					.ForMember(d => d.Age, map => map.MapFrom(s => s.Id.CreationTime.ToLocalTime().AgeInMinutes().AgeInWords()));

				config.CreateMap<User, UserModel>();
			});

			Mapper.AssertConfigurationIsValid();
		}
	}
}