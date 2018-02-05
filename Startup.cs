using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvc.RenderViewToString;
using NLog.Extensions.Logging;
using System;
using System.IO;
using notes.Core.Services;
using notes.Extensions;
using notes.ModelBinders;

namespace notes
{
	public class Startup
	{
		public IConfiguration Configuration { get; set; }

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
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
				options.Cookie.Name = "notes_session";
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.Cookie.Name = "notes";
					options.LoginPath = new PathString("/login");
					options.AccessDeniedPath = new PathString("/");
					options.Events = new CookieAuthenticationEvents
					{
						OnValidatePrincipal = CookieValidator.ValidateAsync
					};
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
			services.AddScoped<RazorViewToStringRenderer>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
		{
			logger.AddNLog();

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

			app.UseAuthentication();

			app.UseSession();

			app.AddRoutes();
		}
	}
}