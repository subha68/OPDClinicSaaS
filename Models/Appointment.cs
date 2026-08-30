using System;

namespace Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int DoctorId { get; set; }
        public int DepartmentId { get; set; }

        public int ClinicId { get; set; }

        public int PatientId { get; set; }
        public int VisitId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string AppointmentDateDisplay => AppointmentDate.ToString("dd-MMM-yyyy");
        public TimeSpan SlotTime { get; set; }
        public string SlotTimeDisplay => SlotStartTime.ToString(@"hh\:mm");
        public TimeSpan SlotStartTime {
            get
            {
               return SlotTime; 
            }
        }
        public TimeSpan SlotEndTime { 
            get
            {
                return SlotTime.Add(TimeSpan.FromMinutes(15));
            }
        }

        public string Status { get; set; }  // Booked, In Consultation, Completed, Cancelled, No Show 
        public int TokenNumber { get; set; }    // per clinic per day
        public int QueueNumber { get; set; }    // per doctor per clinic per day
        public string? CancellationReason { get; set; }
        public string? ClinicName { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public decimal? Fee { get; set; }
        public string? DepartmentName { get; set; }

        public string BillingStatus { get; set; } = "Unbilled";  // Billed, Unbilled
        public string PaymentStatus { get; set; } = "None";  // Paid, Partially Paid, Due, None (Bill not generated)               
    }
}