namespace Mvc.Identity101.Models;

public class UserPhotoViewModel
{
    public int Id { get; set; }
    public string ImgPath { get; set; } = string.Empty;
    public string? Description { get; set; }
}