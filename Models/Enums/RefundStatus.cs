using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum RefundStatus
    {
        [Display(Name = "Draft")]
        Draft = 0,

        [Display(Name = "Pending Approval")]
        PendingApproval = 1,

        [Display(Name = "Approved")]
        Approved = 2,

        [Display(Name = "Rejected")]
        Rejected = 3,

        [Display(Name = "Completed")]
        Completed = 4,

        [Display(Name = "Cancelled")]
        Cancelled = 5,

        [Display(Name = "Partially Refunded")]
        PartiallyRefunded = 6

    }

}