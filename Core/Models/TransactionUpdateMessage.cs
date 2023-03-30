using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TransactionUpdateMessage
    {
        public string EcrId { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }
}
