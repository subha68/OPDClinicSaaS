using System;

namespace ViewModels
{
    public class DoctorScheduleVM
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public String? DoctorFirstName { get; set; }
        public String? DoctorLastName { get; set; }
        public String? ClinicName { get; set; }

        /// <summary>
        /// 0 = Sunday
        /// 1 = Monday
        /// 2 = Tuesday
        /// 3 = Wednesday
        /// 4 = Thursday
        /// 5 = Friday
        /// 6 = Saturday
        /// </summary>
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        /// <summary>
        /// Duration of each appointment slot in minutes.
        /// Example: 10, 15, 20, 30
        /// </summary>
        public int SlotDurationMinutes { get; set; }
        /// <summary>
        /// Maximum number of patients allowed per slot.
        /// </summary>
        public int MaxPatientsPerSlot { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
