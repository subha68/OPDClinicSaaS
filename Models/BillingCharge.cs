namespace Models
{
    public class BillingCharge
    {
        public int ChargeId { get; set; }
        public int ClinicId { get; set; }
        public string ChargeCode { get; set; }
        public string ChargeName { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
    }
}