using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog;
using System.IO;
using System;

namespace Notes
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var _logger = LogManager.GetCurrentClassLogger();
			try
			{
				_logger.Info($"Application notes {ApplicationVersion.InfoVersion()} started.");
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Stopped program because of exception");
				throw;
			}
			finally
			{
				LogManager.Flush();
				LogManager.Shutdown();
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
							config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
							config.AddJsonFile($"logsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
							config.AddEnvironmentVariables(prefix: "NOTES__");
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
