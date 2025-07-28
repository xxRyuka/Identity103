namespace Mvc.Identity101.Models;

public class UserDetailViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ImgPath { get; set; } = "/img/default.jpg";
    public List<UserPhotoViewModel> Photos { get; set; } = new List<UserPhotoViewModel>();
}