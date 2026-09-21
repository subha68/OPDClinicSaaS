namespace Models
{
    public class StockLedger
    {
        public int StockLedgerId { get; set; }
        public int MedicineId { get; set; }
        public int BatchId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? ReferenceType { get; set; }
        public string? ReferenceId { get; set; }
        public int QuantityIn { get; set; }
        public int QuantityOut { get; set; }
        public int BalanceQty { get; set; }        
    }

}