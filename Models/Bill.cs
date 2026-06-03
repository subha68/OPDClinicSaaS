namespace Models
{
    public class Bill
    {
        public int BillId { get; set; }

        public DateTime BillDate { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int VisitId { get; set; }
        public Visit? Visit { get; set; }
        public DateTime? VisitDate { get; set; }

        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        public decimal? GrossAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? NetAmount { 
                            get
                            {
                                if (GrossAmount == null || DiscountAmount == null)
                                    return null;

                                return GrossAmount - DiscountAmount;
                            }
            }
        public decimal? TotalPaidAmount { get; set; } = 0;
        public string Status { 
                            get
                            {
                                if (TotalPaidAmount > 0 && TotalPaidAmount < NetAmount)
                                    return "Partially Paid";
                                else if (TotalPaidAmount >= NetAmount)
                                    return "Paid";
                                else
                                    return "Due";
                            }
                        }
        public decimal? BalanceAmount { 
                        get
                        {
                            if (NetAmount == null)
                                return null;

                            return NetAmount - TotalPaidAmount;
                        }
                    }
        public List<Payment>? Payments { get; set; } = new List<Payment>();      

        public string? AmountInWords { 
                        get
                        {
                            return NetAmount?.ToString(); // replace later with Humanizer if needed
                        }
                    }

        public List<BillLine>? BillLines { get; set; } = new List<BillLine>();
    }

}