using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using postit.Core.Services;
using postit.ModelBinders;
using postit.Models;
using postit.Helper;
using postit.Core.Models;

namespace postit
{
	public class Startup
	{
		public  IConfigurationRoot Configuration { get; }

		public Startup(IHostingEnvironment env, ILoggerFactory logger)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging();
			services.AddMvc(options => {
				options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
			});

			services.AddScoped<MongoContext>(options => new MongoContext(new MongoClient(), "postit"));
			services.AddTransient<PostitService>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
		{
			logger.AddConsole();

			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();
			app.UseMvc(routes => {

				routes.MapRoute(
					name: "create",
					template: "create",
					defaults: new { controller = "Postit", action = "Create" }
				);

				routes.MapRoute(
					name: "postit",
					template: "p/{id?}/{slug?}",
					defaults: new { controller = "Postit", action = "View"},
					constraints: new { id = @"^[a-f0-9]{24}$" } 
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
				config.CreateMap<Postit, PostitModel>()
					.ForMember(d => d.User, map => map.MapFrom(s => "admin"))
					.ForMember(d => d.Age, map => map.MapFrom(s => s.Id.CreationTime.ToLocalTime().AgeInMinutes().AgeInWords()));
			});

			Mapper.AssertConfigurationIsValid();
		}
	}
}