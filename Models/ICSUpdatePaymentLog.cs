using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtaxService.Models
{
    [Table("ICSUpdatePaymentLog", Schema = "ta")]
    public class ICSUpdatePaymentLog
    {
        [Key]
        public string Ref1 { get; set; }
        public string NotificationNo { get; set; }
        public string InvoiceNo { get; set; }
        public string DebtId { get; set; }
        public string ReceiptId { get; set; }
        public string UpdateStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
