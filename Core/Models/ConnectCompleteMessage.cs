using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ConnectCompleteMessage
    {
        public string EcrId { get; set; }
        public string ConnectResultCode { get; set; }
        public string TerminalWakeUpIpAddress { get; set; }
        public string TerminalWakeUpIPPort { get; set; }
    }
}
