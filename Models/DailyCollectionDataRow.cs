namespace Models
{
    public class DailyCollectionDataRow
    {
        public DateTime CollectionDate { get; set; }
        public string PaymentMode { get; set; } = "";
        public int TransactionCount { get; set; }
        public decimal CollectionAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal TotalAmount => 
                    CollectionAmount - RefundAmount;
    }
}