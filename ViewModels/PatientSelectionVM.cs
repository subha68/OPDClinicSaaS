using Models;

namespace ViewModels
{
    public class PatientSelectionVM
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;
        public int? LastVisitId { get; set; }
        public DateTime? LastVisitDate { get; set; }

        public PatientSelectionVM()
        {
            // default constructor
        }

        public PatientSelectionVM(Patient p)
        {
            PatientId = p.PatientId;
            FullName = p.FullName;
            Gender = p.Gender ?? "";
            Age = p.Age;
            Phone = p.Phone;
            LastVisitId = null;
            LastVisitDate = null;
        }
    }

}
