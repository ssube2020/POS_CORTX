using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AmountForReceipt
    {
        public int Units { get; set; }
        public string CurrencyCode { get; set; }
        public string? CurrencyExp { get; set; }
        public string? DisplayString { get; set; }
    }
}
