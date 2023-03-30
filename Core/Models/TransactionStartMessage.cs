using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TransactionStartMessage
    {
        public string? EcrId { get; set; }
        public string Type { get; set; }
        public Amount Amount { get; set; }
        public string OrderRef { get; set; }
    }
}
