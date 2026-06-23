using Models;

namespace ViewModels
{
    public class JournalLineVM
    {
        public int LedgerAccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}