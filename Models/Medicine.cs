namespace Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }

        public string? MedicineCode { get; set; }

        public string MedicineName { get; set; } = string.Empty;

        public string GenericName { get; set; } = string.Empty;

        public string? HSNCode { get; set; }

        public string? Strength { get; set; }

        public string DosageForm { get; set; } = string.Empty;

        public string UnitOfMeasure { get; set; } = string.Empty;

        public int? ReorderLevel { get; set; }

        public decimal? GSTPercent { get; set; }
        public string? Manufacturer { get; set; }
        public string ScheduleType { get; set; } = string.Empty;
        public bool? CreatedFromPrescription { get; set; }

        public bool?     IsActive { get; set; } = true;
    }
}