namespace Models
{
    public class TestPanel
    {
        public int TestPanelId { get; set; }
        public int? ClinicId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
