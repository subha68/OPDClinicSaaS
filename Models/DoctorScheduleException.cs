using System;

namespace Models
{
    public class DoctorScheduleException
    {
        public int ExceptionId { get; set; }

        public int DoctorId { get; set; }

        public int ClinicId { get; set; }

        /// <summary>
        /// Date for leave / holiday / blocked schedule.
        /// </summary>
        public DateTime ExceptionDate { get; set; }

        /// <summary>
        /// Optional partial-day block start time.
        /// Null if full day leave.
        /// </summary>
        public TimeSpan? StartTime { get; set; }

        /// <summary>
        /// Optional partial-day block end time.
        /// Null if full day leave.
        /// </summary>
        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Reason for leave/block.
        /// Example:
        /// Vacation
        /// Emergency
        /// Conference
        /// Surgery
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// True = entire day blocked
        /// False = partial time blocked
        /// </summary>
        public bool IsFullDay { get; set; }
    }
}