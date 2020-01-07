using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Mvc.RenderViewToString;
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

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// add options to DI
			services.AddOptions<Settings>()
				.Bind(Configuration.GetSection("Settings"));
			// .ValidateDataAnnotations()
			// .Validate(v => { return true; }, "Error Message"); // https://github.com/stevejgordon/OptionsValidationSample

#pragma warning disable ASP0000
			// get settings
			services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<Settings>>().Value);
			var settings = services.BuildServiceProvider().GetRequiredService<Settings>();
#pragma warning restore

			// database context
			services.AddDbContext<MySqlContext>(options =>
			{
				options.UseMySql(settings.ConnectionString, mySqlOptions => { });
				//options.EnableSensitiveDataLogging(true);
			},
			ServiceLifetime.Scoped
			);

			// dependency injection
			services.AddAutoMapper();
			services.AddTransient<NoteService>();
			services.AddTransient<UserService>();
			services.AddTransient<MailService>();
			services.AddTransient<RevisionService>();
			services.AddScoped<RazorViewToStringRenderer>();

			// key ring
			if (!String.IsNullOrEmpty(settings.KeyStore))
			{
				services.AddDataProtection(options =>
				{
					options.ApplicationDiscriminator = "notes";
				}).PersistKeysToFileSystem(new DirectoryInfo(settings.KeyStore));
			}

			// IIS integration
			services.Configure<IISOptions>(options => { });

			// add custom model binders
			services.AddMvc(options =>
			{
				options.EnableEndpointRouting = false;
				// options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
			}).AddJsonOptions(options =>
			{
				// options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
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

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logger)
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