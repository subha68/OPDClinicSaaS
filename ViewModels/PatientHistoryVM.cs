using Models;

namespace ViewModels
{
    public class PatientHistoryVM
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;

        // ---------------------------------------------------------
        // Visit History
        // ---------------------------------------------------------

        public List<VisitHistoryVM> Visits { get; set; } = new();

        // ---------------------------------------------------------
        // Pagination
        // ---------------------------------------------------------

        public int TotalVisits { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public bool HasMoreVisits =>
            (CurrentPage * PageSize) < TotalVisits;

        public int TotalAppointments { get; set; }
        public int TotalBills { get; set; }
        public int TotalPayments { get; set; }

        public PatientHistoryVM()
        {
        }

        public PatientHistoryVM(Patient patient)
        {
            PatientId = patient.PatientId;
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            Gender = patient.Gender;
            Phone = patient.Phone;
            Age = patient.Age;
        }
    }
}
