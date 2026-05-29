using System;

namespace Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BillId { get; set; }

        public decimal Amount { get; set; }

        public string Mode { get; set; }

        public string ReferenceNo { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}