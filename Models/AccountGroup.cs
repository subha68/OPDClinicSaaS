namespace Models
{
    public class AccountGroup
    {
        public int AccountGroupId { get; set; }
        public int ClinicId { get; set; }
        public string GroupName { get; set; }
        public int ParentGroupId { get; set; }
        public string Nature { get; set; }
    }
}   