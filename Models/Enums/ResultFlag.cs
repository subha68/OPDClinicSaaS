using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum ResultFlag
    {
        [Display(Name = "Normal")]
        Normal = 1,

        [Display(Name = "High")]
        High = 2,

        [Display(Name = "Low")]
        Low = 3,

        [Display(Name = "Critical")]
        Critical = 4
    }

}