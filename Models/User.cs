namespace Models
{
    public class User
    {
        public int UserId { get; set; }
        public int? ClinicId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsFirstLogin { get; set; }
    }
}