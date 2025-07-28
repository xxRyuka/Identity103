using System.ComponentModel.DataAnnotations;

namespace Mvc.Identity101.Data.Dto;

public class ChangePasswordDto


{
    [Required(ErrorMessage = "Old Password Field is required")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string NewPassword { get; set; }

    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    [Required(ErrorMessage = "Confirm password is required")]
    public string ConfirmNewPassword { get; set; }
}