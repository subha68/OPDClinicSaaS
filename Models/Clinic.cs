namespace Models
{
    public class Clinic
    {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string? ClinicCode { get; set; }
        public string? LogoPath { get; set; }
    }
}