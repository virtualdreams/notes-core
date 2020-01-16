using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;
using System;
using notes.Helper;

namespace notes
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
			try
			{
				logger.Info($"Application notes {ApplicationVersion.InfoVersion()} started.");
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception e)
			{
				logger.Error(e, "Stopped program because of exception");
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.UseContentRoot(Directory.GetCurrentDirectory())
						.ConfigureAppConfiguration((hostContext, config) =>
						{
							config.Sources.Clear();
							config.SetBasePath(Directory.GetCurrentDirectory());
							config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
							config.AddJsonFile($"logsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
							config.AddEnvironmentVariables();
						})
						.UseStartup<Startup>();
				})
				.ConfigureLogging(logging =>
				{
					logging.ClearProviders();
					logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
				})
				.UseNLog();
	}
}
