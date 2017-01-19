using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using notes.Core.Services;
using notes.Extensions;
using notes.ModelBinders;

namespace notes
{
    public class Startup
	{
		public IConfigurationRoot Configuration { get; set; }

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			var _keyStore = Configuration.GetSection("Settings")["KeyStore"];
			if(!String.IsNullOrEmpty(_keyStore))
			{
				services.AddDataProtection(options => {
					options.ApplicationDiscriminator = "notes";
				}).PersistKeysToFileSystem(new DirectoryInfo(_keyStore));
			}

			// IIS integration
			services.Configure<IISOptions>(options => {

			});

			// add logging to DI
			services.AddLogging();

			// add options to DI
			services.AddOptions();
			services.Configure<Settings>(Configuration.GetSection("Settings"));

			// add custom model binders
			services.AddMvc(options => {
				options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
			});

			// authorization policies
			services.AddAuthorization(options => {
				options.AddPolicy("AdministratorOnly", policy => {
					policy.RequireRole("Administrator");
				});
			});

			// DI
			services.AddAutoMapper();
			services.AddScoped<MongoContext>();
			services.AddTransient<NoteService>();
			services.AddTransient<UserService>();
			services.AddScoped<IViewRenderService, ViewRenderService>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
		{
			logger.AddConsole(Configuration.GetSection("Logging"));

			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseCookieAuthentication(
				new CookieAuthenticationOptions {
				AuthenticationScheme = "notes",
				CookieName = "notes",
				LoginPath = new PathString("/login"),
				AccessDeniedPath = new PathString("/login"),
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				Events = new CookieAuthenticationEvents
				{
					OnValidatePrincipal = CookieValidator.ValidateAsync
				}
			});

			app.AddRoutes();
		}
	}
}