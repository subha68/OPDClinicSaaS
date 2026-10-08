using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum PaymentType
    {
        [Display(Name = "Receipt")]
        Receipt = 1,

        [Display(Name = "Refund")]
        Refund = 2
    }

}