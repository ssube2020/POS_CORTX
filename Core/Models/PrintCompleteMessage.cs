using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PrintCompleteMessage
    {
        public string EcrId { get; set; }
        public string ResultCode { get; set; }
        public string NextAction { get; set; }
    }
}
