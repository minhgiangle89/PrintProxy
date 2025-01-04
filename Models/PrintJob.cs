using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintProxy.Models
{
    public class PrintJob
    {
        public string JobId { get; set; }
        public string DocumentData { get; set; }
        public string PrinterName { get; set; }
        public int Copies { get; set; }
        public Dictionary<string, string> Settings { get; set; }
    }

    public class PrintJobRequest
    {
        public string DocumentData { get; set; }
        public string PrinterName { get; set; }
        public int Copies { get; set; }
        public Dictionary<string, string> Settings { get; set; }
    }

    public class PrintResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public enum PrintProxyStatus
    {
        Online,
        Busy,
        Error,
        Offline
    }
}
