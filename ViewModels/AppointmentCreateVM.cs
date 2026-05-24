public class AppointmentCreateVM
{
    public int SlotId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string SlotTime { get; set; }

    public string Mobile { get; set; }
    public string PatientName { get; set; }
    public string PatientSearch { get; set; }

    public string VisitType { get; set; } = "New";
    public bool IsVisitLocked { get; set; } = false;
    public int VisitId { get; set; }

}