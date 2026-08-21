using System.ComponentModel.DataAnnotations;
using Models;

namespace ViewModels
{
    public class ChangePasswordVM
    {
        [Required(
            ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(
            8,
            ErrorMessage =
                "New password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Please confirm the new password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare(
            "NewPassword",
            ErrorMessage =
                "The new passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}