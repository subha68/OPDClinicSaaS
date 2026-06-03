using Models;

namespace ViewModels
{
    public class PatientHistoryVM
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }

        public List<VisitHistoryVM> Visits { get; set; } = new List<VisitHistoryVM>();

        public int TotalVisits { get; set; }
        public int CurrentPage { get; set; }
        public bool HasMoreVisits => (CurrentPage * 3) < TotalVisits;

        public PatientHistoryVM()
        {
        }

        public PatientHistoryVM(Patient patient)
        {
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            PatientId = patient.PatientId;
            Gender = patient.Gender;
            Phone = patient.Phone;
            Age = patient.Age;
        }
    }
}