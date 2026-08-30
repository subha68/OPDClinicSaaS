using System;

namespace Models
{
    public class AppointmentBookingResult
    {
        public bool success { get; set; }
        public int AppointmentId { get; set; }
        public int TokenNumber { get; set; }
        public int QueueNumber { get; set; }
        public Patient? Patient { get; set; }
    }
}