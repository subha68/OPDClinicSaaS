namespace Models
{
    public class TestPanelParameter
    {
        public int TestPanelParameterId { get; set; }
        public int? ClinicId { get; set; }
        public int TestPanelId { get; set; }
        public int TestParameterId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
