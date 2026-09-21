namespace Models
{
    public class LedgerAccount
    {
        public int LedgerAccountId { get; set; }
        public int ClinicId { get; set; }
        public int AccountGroupId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
    }

}