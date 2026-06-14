using Models;

namespace ViewModels
{
    public class DoctorShiftVM
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string DayName
                    { get
                        {
                           return ((DayOfWeek)DayOfWeek).ToString();  
                        }                       
                    }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}