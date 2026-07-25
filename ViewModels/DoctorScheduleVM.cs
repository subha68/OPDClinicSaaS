using Models;

namespace ViewModels
{
    public class DoctorScheduleVM
    {
        public int ScheduleId { get; set; }
        public int DepartmentId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public String? DoctorFirstName { get; set; }
        public String? DoctorLastName { get; set; }
        public String? ClinicName { get; set; }
        public int SlotDurationMinutes { get; set; }
        public int MaxPatientsPerSlot { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<DoctorShiftVM> Shifts { get; set; }

        public DoctorScheduleVM()
        {
            SlotDurationMinutes = 15;
            MaxPatientsPerSlot = 1;
            Shifts = new List<DoctorShiftVM>();
        }
        public DoctorScheduleVM(int clinicId)
        {
            ClinicId = clinicId;
            SlotDurationMinutes = 15;
            MaxPatientsPerSlot = 1;
            Shifts = new List<DoctorShiftVM>();
        }        
    }
}
