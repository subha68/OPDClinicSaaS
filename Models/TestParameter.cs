namespace Models
{
    public class TestParameter
    {
        public int TestParameterId { get; set; }
        public int? ClinicId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DataType { get; set; } = "Numeric";
        public string? Unit { get; set; }
        public string? NormalRange { get; set; }
        public decimal? NormalMin { get; set; }
        public decimal? NormalMax { get; set; }
        public string? TestMethod { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
    }
}
