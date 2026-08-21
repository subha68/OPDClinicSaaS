namespace Models
{
    public class ReportLookupDefinition
    {
        public string LookupCode { get; set; } = "";
        public Type DALType { get; set; } = null!;
        public string MethodName { get; set; } = "";

        public ReportLookupDefinition(string lookupCode, Type dalType, string methodName)
        {
            LookupCode = lookupCode;
            DALType = dalType;
            MethodName = methodName;
        }
    }
}