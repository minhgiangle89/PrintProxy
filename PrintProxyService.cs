using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintProxy
{
    public class PrintProxyService : BackgroundService
    {
        private readonly PrintProxyClient _printProxyClient;
        private readonly ILogger<PrintProxyService> _logger;

        public PrintProxyService(PrintProxyClient printProxyClient, ILogger<PrintProxyService> logger)
        {
            _printProxyClient = printProxyClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _printProxyClient.StartAsync(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PrintProxyService: {ex.Message}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _printProxyClient.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
