using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintProxy.Models
{
    public class ProductNotification
    {
        public int NotificationId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public NotificationStatus Status { get; set; }
    }

    public enum NotificationStatus
    {
        Pending,
        Delivered,
        Failed
    }
}
