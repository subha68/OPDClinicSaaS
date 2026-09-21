using Models;

namespace ViewModels
{
    public class VisitHistoryVM
    {
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; } = string.Empty;

        public List<AppointmentHistoryVM> Appointments { get; set; } = new List<AppointmentHistoryVM>();
        public List<Bill> Bills { get; set; } = new List<Bill>();

        public VisitHistoryVM()
        {
        }

        public VisitHistoryVM(Visit visit)
        {
            VisitId = visit.VisitId;
            VisitDate = visit.VisitDate;
            VisitType = visit.VisitType;
        }
    }
}