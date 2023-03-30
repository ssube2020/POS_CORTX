using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TimeoutMessage
    {
        public string EcrId { get; set; }
        public string? Reason { get; set; }
    }
}
