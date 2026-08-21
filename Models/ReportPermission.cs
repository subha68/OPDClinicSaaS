namespace Models
{
    public class ReportPermission
    {
        public long ReportPermissionId { get; set; }

        public long ReportId { get; set; }

        public string PrincipalType { get; set; } = string.Empty;

        public long PrincipalId { get; set; }

        public string PermissionCode { get; set; } = string.Empty;
    }
}