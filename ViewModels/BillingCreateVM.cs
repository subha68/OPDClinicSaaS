using Models;

namespace ViewModels
{
    public class BillingCreateVM
    {
        public Bill Bill { get; set; } = new();

        public int PreviousBillCount { get; set; }

        public List<BillingAppointmentVM> Appointments { get; set; } = new();
        public int? ConsultationChargeId { get; set; }
    }

    public class BillingAppointmentVM
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public decimal? Fee { get; set; }
    }

}
