using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using NLog.Web;
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
			// only needed if the file is somewhere else or has a different name.
			//env.ConfigureNLog("NLog.config");

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			var _keyStore = Configuration.GetSection("Settings")["KeyStore"];
			if (!String.IsNullOrEmpty(_keyStore))
			{
				services.AddDataProtection(options =>
				{
					options.ApplicationDiscriminator = "notes";
				}).PersistKeysToFileSystem(new DirectoryInfo(_keyStore));
			}

			// IIS integration
			services.Configure<IISOptions>(options =>
			{

			});

			// add options to DI
			services.AddOptions();
			services.Configure<Settings>(Configuration.GetSection("Settings"));

			// add custom model binders
			services.AddMvc(options =>
			{
				options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
			});

			// add sessions
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.CookieName = "notes_session";
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.CookieHttpOnly = true;
			});

			// authorization policies
			services.AddAuthorization(options =>
			{
				options.AddPolicy("AdministratorOnly", policy =>
				{
					policy.RequireRole("Administrator");
				});
			});

			// DI
			services.AddAutoMapper();
			services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<Settings>>().Value);
			services.AddScoped<MongoContext>();
			services.AddTransient<NoteService>();
			services.AddTransient<UserService>();
			services.AddTransient<MailService>();
			services.AddTransient<MaintenanceService>();
			services.AddScoped<IViewRenderService, ViewRenderService>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
		{
			logger.AddNLog();

			app.AddNLogWeb();

			app.UseStatusCodePagesWithReExecute("/error/{0}");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/error/500");
			}

			app.UseStaticFiles();

			app.UseCookieAuthentication(
				new CookieAuthenticationOptions
				{
					AuthenticationScheme = "notes",
					CookieName = "notes",
					LoginPath = new PathString("/login"),
					AccessDeniedPath = new PathString("/"),
					AutomaticAuthenticate = true,
					AutomaticChallenge = true,
					Events = new CookieAuthenticationEvents
					{
						OnValidatePrincipal = CookieValidator.ValidateAsync
					}
				});

			app.UseSession();

			app.AddRoutes();
		}
	}
}