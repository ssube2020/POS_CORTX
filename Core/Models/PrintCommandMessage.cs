

namespace Core.Models
{
    public class PrintCommandMessage
    {
        public string FormattedReceipt { get; set; }
        public List<string> Lines { get; set; }
        public string? AidCard { get; set; }
        public Amount Amount1 { get; set; }  
        public Amount? Amount2 { get; set; }
        public string? AppPreferredName { get; set; }
        public string? AuthorisationCode { get; set; }
        public string? AuthorisationResponseCode { get; set; }
        public string? CardSeqNo { get; set; }
        public string? Cryptogram { get; set; }
        public DateTime? ExpDate { get; set; }
        public string? Iin { get; set; }
        public string LanguagePref { get; set; }
        public string MerchantId { get; set; }
        public string MerchantNameAndLocation { get; set; }
        public string OrderRef { get; set; }
        public string? PanMasked { get; set; }
        public int? PosEntryMode { get; set; }
        public string? ProcessorName { get; set; }
        public string ReceiptDuplicate { get; set; }
        public string ReceiptFooter { get; set; }
        public int SettlementDate { get; set; }
        public string TerminalId { get; set; }
        public string TransactionId { get; set; }
        public string TransactionSeq { get; set; }
        public string TransactionStatus { get; set; }
        public bool TransactionSucceded { get; set; }
        public DateTime TransactionTime { get; set; }
        public int TransactionType { get; set; }
        public string? TVR { get; set; }
    }
}
