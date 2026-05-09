using System;

namespace Models
{
    public class AppointmentSlot
    {
        public int SlotId { get; set; }

        public int DoctorId { get; set; }

        public int ClinicId { get; set; }

        /// <summary>
        /// Logical appointment date.
        /// </summary>
        public DateTime SlotDate { get; set; }

        /// <summary>
        /// Slot start datetime.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Slot end datetime.
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// Maximum patients allowed in slot.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Current booked patient count.
        /// </summary>
        public int BookedCount { get; set; }

        /// <summary>
        /// True = slot blocked/unavailable.
        /// </summary>
        public bool IsBlocked { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}