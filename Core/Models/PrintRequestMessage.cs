using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PrintRequestMessage
    {
        public string EcrId { get; set; }
        public string? TransactionId { get; set; }
    }
}
