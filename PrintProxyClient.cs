﻿using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrintProxy.Models;
using PrintProxy.Services;
using PrintProxy.Services.IServices;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace PrintProxy
{
    public class PrintProxyClient
    {
        private HubConnection _connection;
        private readonly IPrintService _printService;
        private readonly ILogger<PrintProxyClient> _logger;
        private readonly string _productHubServerUrl;
        private readonly IAuthService _authService;
        private string _currentToken;

        public PrintProxyClient(IPrintService printService, ILogger<PrintProxyClient> logger, IConfiguration configuration, IAuthService authService)
        {
            _printService = printService;
            _logger = logger;
            _authService = authService;
            _productHubServerUrl = configuration["ProductHubServerUrl"];
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _currentToken =  await _authService.LoginAsync();
            _connection = new HubConnectionBuilder()
                .WithUrl(_productHubServerUrl, options => {
                    options.Headers["Authorization"] = $"Bearer {_currentToken}";
                })
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();
            await ConnectWithRetryAsync(cancellationToken);
        }

        private void RegisterHandlers()
        {
            _connection.On<PrintJob>("PrintDocument", async (printJob) =>
            {
                try
                {
                    _logger.LogInformation($"Received print job: {printJob.JobId}");
                    var result = await _printService.PrintAsync(printJob);
                    await _connection.InvokeAsync("PrintJobCompleted",
                        printJob.JobId,
                        result.Success,
                        result.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Print error: {ex.Message}");
                    await _connection.InvokeAsync("PrintJobCompleted",
                        printJob.JobId,
                        false,
                        ex.Message);
                }
            });

            _connection.On<int>("GetProductById", async (productId) =>
            {
                try
                {
                    _logger.LogInformation($"Received product notification: {productId}");
                    var notification = new ProductNotification { ProductId = productId };
                    var result = await _printService.ProcessProductNotificationAsync(notification);

                    await _connection.InvokeAsync("ProductProcessed",
                        productId,
                        result.Success,
                        result.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing product: {ex.Message}");
                    await _connection.InvokeAsync("ProductProcessed",
                        productId,
                        false,
                        ex.Message);
                }
            });

        }

        private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _connection.StartAsync(cancellationToken);
                    _logger.LogInformation("Connected to server");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Connection failed: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        public async Task StopAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
            }
        }
    }
}

