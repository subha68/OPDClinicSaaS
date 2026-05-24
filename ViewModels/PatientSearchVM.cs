using Models;

namespace ViewModels
{
    public class PatientSearchVM
    {
        public string? PatientId { get; set; }
        public string? Phone { get; set; }
        public string? Name { get; set; }

        public List<Patient> Results { get; set; } = new();
    }
}