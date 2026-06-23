namespace Models
{
    public class JournalLine
    {
        public int JournalLineId { get; set; }
        public int ClinicId { get; set; }
        public int JournalHeaderId { get; set; }
        public int LedgerAccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        public JournalLine()
        {
        }

        public JournalLine(int clinicId)
        {
            ClinicId = clinicId;
        }
    }

}