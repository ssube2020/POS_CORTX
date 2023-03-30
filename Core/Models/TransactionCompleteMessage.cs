

namespace Core.Models
{
    public class TransactionCompleteMessage
    {
        public string EcrId { get; set; }
        public string ResultCode { get; set; }
        public string NextAction { get; set; }
        public string TransactionId { get; set; }
    }
}
