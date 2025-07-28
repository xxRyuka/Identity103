using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Identity101.Data.Dto;


//Mvc = ViewModel
//Api = Dto
public class SignUpDto
{
    [Required(ErrorMessage ="Username is required")]
    [DisplayName("Username")]
    public string UserName { get; set; }
    
    [EmailAddress(ErrorMessage ="Email format is wrong")]
    [Required(ErrorMessage ="Email is required")]
    public string Email { get; set; }
    
    [Phone(ErrorMessage ="Phone format is wrong")]
    [Required(ErrorMessage ="Phone is required")]
    public string Phone { get; set; }
    
    [Required(ErrorMessage ="Password is required")]
    public string Password { get; set; }
    
    [Compare(nameof(Password), ErrorMessage ="Passwords do not match")]
    [Required(ErrorMessage ="Confirm password is required")]
    public string ConfirmPassword { get; set; }
    
}