using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

namespace Mvc.Identity101.Data.Entites;

public class UserPhoto
{
    public int Id { get; set; }

    [Required]
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
    
    public string imgPath { get; set; }
    public string Description { get; set; }
    public DateTime UploadDate { get; set; }
    
    
    // Navigation properties
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}