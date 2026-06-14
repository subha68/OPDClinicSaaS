namespace Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }

        public string? MedicineCode { get; set; }

        public string MedicineName { get; set; }

        public string GenericName { get; set; }

        public string? HSNCode { get; set; }

        public string? Strength { get; set; }

        public string DosageForm { get; set; }

        public string UnitOfMeasure { get; set; }

        public int? ReorderLevel { get; set; }

        public decimal? GSTPercent { get; set; }
        public string? Manufacturer { get; set; }
        public string ScheduleType { get; set; }
        public bool? CreatedFromPrescription { get; set; }

        public bool?     IsActive { get; set; } = true;
    }
}