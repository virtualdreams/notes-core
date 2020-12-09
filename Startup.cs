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
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Logging;
using System.IO;
using System;
using notes.Core.Data;
using notes.Core.Internal;
using notes.Core.Services;
using notes.Events;
using notes.Extensions;
using notes.Filter;
using notes.Services;

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
#if DEBUG
			IdentityModelEventSource.ShowPII = true;
#endif

			// features
			services.AddFeatureManagement();

			// add options to DI
			services.AddOptions<Settings>()
				.Bind(Configuration.GetSection(Settings.SettingsName));
			//.ValidateDataAnnotations();

			// get settings for local usage
			var settings = Configuration.GetSection(Settings.SettingsName).Get<Settings>();

			// database context
			services.AddDbContext<DataContext>(options =>
			{
				options.UseMySql(Configuration.GetConnectionString("Default"), mySqlOptions => { });
#if DEBUG
				options.EnableSensitiveDataLogging(true);
#endif
			},
			ServiceLifetime.Scoped);

			// dependency injection
			services.AddAutoMapper();
			services.AddTransient<INoteService, NoteService>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IMailService, MailService>();
			services.AddTransient<IRevisionService, RevisionService>();
			services.AddTransient<ITokenService, TokenService>();
			services.AddScoped<CustomCookieAuthenticationEvents>();
			services.AddSingleton<IPasswordPolicy>(new PasswordPolicy { MinimumNonAlphaCharacters = 0, MinimumUpperCaseCharacters = 0 });

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
				options.Filters.AddForFeature<ModelStateDebugFilter>(nameof(FeatureFlags.ModelStateDebug));
				// options.Filters.Add<ModelStateDebugFilter>();
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
					options.EventsType = typeof(CustomCookieAuthenticationEvents);
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