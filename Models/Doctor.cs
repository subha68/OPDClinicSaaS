using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }       // doctors must be associated with a clinic
        public string? ClinicName { get; set; }
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string RegistrationNo { get; set; }
        public string Qualification { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

    }
}
