using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;


namespace PrintProxy
{
    internal static class Program
    {
        static void Main(string[] args)
        {

                CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                var configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                config.AddConfiguration(new ConfigurationRoot(new List<IConfigurationProvider>
                    {
                new MemoryConfigurationProvider(new MemoryConfigurationSource
                {
                    InitialData = configuration.AppSettings.Settings
                        .Cast<System.Configuration.KeyValueConfigurationElement>()
                        .ToDictionary(x => x.Key, x => x.Value)
                })
                    }));
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<PrintProxyService>();
                services.AddSingleton<PrintProxyClient>();
                services.AddSingleton<IPrintService, PrintService>();
                services.AddSingleton<IHubConnectionBuilder, HubConnectionBuilder>();
            })
            .ConfigureLogging((hostContext, logging) =>
            {
                logging.AddEventLog();
                logging.AddConsole();
            })
            .UseWindowsService();

            return hostBuilder;
        }
    }
}

