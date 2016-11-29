using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace notes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // configure the server
            var host = new WebHostBuilder()
				.UseKestrel()
				//.UseUrls(GlobalConfig.Get().Kestrel)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>()
				.Build();
            
            // start
			host.Run();
        }
    }
}
