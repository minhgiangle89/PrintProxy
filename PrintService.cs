using Microsoft.Extensions.Logging;
using PrintProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintProxy
{
    public class PrintService : IPrintService
    {
        private readonly ILogger<PrintService> _logger;
        private readonly Queue<ProductNotification> _notificationQueue;

        public PrintService(ILogger<PrintService> logger)
        {
            _logger = logger;
            _notificationQueue = new Queue<ProductNotification>();
        }

        public async Task<PrintResult> PrintAsync(PrintJob job)
        {
            try
            {
                // Thực hiện in thực tế tại đây
                _logger.LogInformation($"Printing job {job.JobId}");
                await Task.Delay(2000); // Giả lập thời gian in

                return new PrintResult { Success = true, Message = "Print completed" };
            }
            catch (Exception ex)
            {
                return new PrintResult { Success = false, Message = ex.Message };
            }
        }

        public async Task<PrintProxyStatus> GetStatusAsync()
        {
            return PrintProxyStatus.Online;
        }

        public async Task<PrintResult> ProcessProductNotificationAsync(ProductNotification notification)
        {
            try
            {
                _logger.LogInformation($"Processing notification for product {notification.ProductId}");

                // Thêm vào queue
                _notificationQueue.Enqueue(notification);

                // Giả lập thời gian xử lý
                await Task.Delay(1000);

                // Xử lý thông báo từ queue
                while (_notificationQueue.Count > 0)
                {
                    var currentNotification = _notificationQueue.Dequeue();
                    _logger.LogInformation($"Sending notification: {currentNotification.Message}");
                }

                return new PrintResult { Success = true, Message = "Notification processed successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing notification: {ex.Message}");
                return new PrintResult { Success = false, Message = ex.Message };
            }
        }
    }

    public interface IPrintService
    {
        Task<PrintResult> PrintAsync(PrintJob job);
        Task<PrintProxyStatus> GetStatusAsync();
        Task<PrintResult> ProcessProductNotificationAsync(ProductNotification notification);
    }
}
