using Models;

namespace ViewModels
{
    public class AppointmentHistoryVM
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; }
        public string DoctorName { get; set; }
        public string DepartmentName { get; set; }
        public string Status { get; set; }

        public PrescriptionVM Prescription { get; set; }

        public AppointmentHistoryVM()
        {
        }

        public AppointmentHistoryVM(Appointment appointment, PrescriptionVM prescription)
        {
            AppointmentId = appointment.AppointmentId;
            AppointmentDate = appointment.AppointmentDate;
            SlotTime = appointment.SlotTime.ToString(@"hh\:mm");
            DoctorName = appointment.DoctorName;
            DepartmentName = appointment.DepartmentName;
            Status = appointment.Status;

            Prescription = prescription;
        }        
    }
}