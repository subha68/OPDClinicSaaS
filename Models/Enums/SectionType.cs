using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum SectionType
    {
        [Display(Name = "Standard")]
        Standard = 0,
        
        [Display(Name = "Multiline")]
        Multiline = 1,
        
        [Display(Name = "Rx")]
        Rx = 2,
        
        [Display(Name = "Vitals")]
        Vital = 3,
        
        [Display(Name = "Test Results")]
        TestResult = 4
    }

}