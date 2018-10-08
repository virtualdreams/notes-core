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
using notes.Helper;
using notes.ModelBinders;

namespace notes
{
	public class Startup
	{
		private readonly ILogger<Startup> Log;

		public IConfiguration Configuration { get; set; }

		public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> log)
		{
			Log = log;
			Configuration = configuration;

			Log.LogInformation($"Application notes {ApplicationVersion.InfoVersion()} started.");
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// add options to DI
			services.AddOptions();
			services.Configure<Settings>(Configuration.GetSection("Settings"));

			// DI
			services.AddAutoMapper();
			services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<Settings>>().Value);
			services.AddScoped<MongoContext>();
			services.AddTransient<NoteService>();
			services.AddTransient<UserService>();
			services.AddTransient<MailService>();
			services.AddTransient<MaintenanceService>();
			services.AddScoped<RazorViewToStringRenderer>();

			// get settings
			var settings = services.BuildServiceProvider().GetRequiredService<Settings>();

			// key ring
			if (!String.IsNullOrEmpty(settings.KeyStore))
			{
				services.AddDataProtection(options =>
				{
					options.ApplicationDiscriminator = "notes";
				}).PersistKeysToFileSystem(new DirectoryInfo(settings.KeyStore));
			}

			// IIS integration
			services.Configure<IISOptions>(options =>
			{

			});

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
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
		{
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