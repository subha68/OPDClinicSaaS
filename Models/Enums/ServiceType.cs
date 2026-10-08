using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum ServiceType
    {
        [Display(Name = "Consultation")]
        Consultation = 1,
        
        [Display(Name = "Lab")]
        Lab = 2,
        
        [Display(Name = "Radiology")]
        Radiology = 3,
        
        [Display(Name = "Pharmacy")]
        Pharmacy = 4,
        
        [Display(Name = "Registration")]
        Registration = 5
    }

}