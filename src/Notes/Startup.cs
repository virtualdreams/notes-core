using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Logging;
using Notes.Core;
using Notes.Events;
using Notes.Extensions;
using Notes.Features;
using Notes.Filter;
using Notes.Migrations;
using Notes.Options;
using Notes.Provider;
using System.IO;
using System;

namespace Notes
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
			services.AddFeatureManagement(Configuration.GetSection("FeatureFlags"));

			// add options to DI
			services.AddOptions<AppSettings>()
				.Bind(Configuration.GetSection(AppSettings.SectionName));
			//.ValidateDataAnnotations();
			services.AddOptions<MailSettings>()
				.Bind(Configuration.GetSection(MailSettings.SectionName));
			//.ValidateDataAnnotations();

			// get settings for local usage
			var _keyStore = Configuration.GetSection(AppSettings.SectionName).GetValue<string>("KeyStore", null);
			var _provider = Configuration.GetSection("Database").GetValue<DatabaseProvider>("Provider", DatabaseProvider.PgSql);
			var _connectionString = Configuration.GetConnectionString(_provider.ToString());

			// migrations
			services.AddFluentMigrator(_connectionString, _provider);

			// database context
			services.AddDatabaseContext(_connectionString, _provider);

			// dependency injection
			services.AddAutoMapper();

			services.AddScoped<CustomCookieAuthenticationEvents>();

			services.AddNoteServices(_provider);

			// set keystore
			if (String.IsNullOrEmpty(_keyStore))
			{
				_keyStore = "keystore";
			}

			// key ring
			services.AddDataProtection(options =>
			{
				options.ApplicationDiscriminator = "notes";
			})
			.PersistKeysToFileSystem(
				new DirectoryInfo(_keyStore)
			);

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
			});

			// fluent validation
			services.AddFluentValidationAutoValidation(options =>
			{
				options.DisableDataAnnotationsValidation = true;
			})
			.AddFluentValidationClientsideAdapters();

			services.AddValidatorsFromAssemblyContaining<Startup>();

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
			app.MigrateDatabase();

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