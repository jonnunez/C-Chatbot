using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace BotApp
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseApplicationInsights()
        .ConfigureLogging((hostingContext, logging) =>
            {
                  // Add Azure Logging
                  logging.AddAzureWebAppDiagnostics();
            })
            .UseStartup<Startup>()
            .Build();
  }
}
