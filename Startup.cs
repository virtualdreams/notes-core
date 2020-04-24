using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System;
using notes.Core.Services;
using notes.Extensions;
using notes.Filter;

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
			services.AddOptions();
			services.Configure<Settings>(Configuration.GetSection("Settings"));
			// services.Configure<Smtp>(Configuration.GetSection("Settings.Smtp"));

			// get settings for local usage
			var settings = new Settings();
			Configuration.GetSection("Settings").Bind(settings);

			// database context
			services.AddDbContext<MySqlContext>(options =>
			{
				options.UseMySql(settings.ConnectionString, mySqlOptions => { });
				//options.EnableSensitiveDataLogging(true);
			},
			ServiceLifetime.Scoped);

			// dependency injection
			services.AddAutoMapper();
			services.AddTransient<NoteService>();
			services.AddTransient<UserService>();
			services.AddTransient<MailService>();
			services.AddTransient<RevisionService>();
			services.AddScoped<CustomCookieEvents>();

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

			// mvc
			services.AddMvc(options =>
			{
				options.Filters.Add<ModelStateDebugFilter>();
				// options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
			})
			.AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			})
			.AddFluentValidation(options =>
			{
				options.RegisterValidatorsFromAssemblyContaining<Startup>();
				options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
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
					options.EventsType = typeof(CustomCookieEvents);
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

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

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

			app.UseRouting();

			// app.UseCors();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseSession();

			app.AddEndpoints();
		}
	}
}