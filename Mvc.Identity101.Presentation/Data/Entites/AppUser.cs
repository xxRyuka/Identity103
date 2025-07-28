using Microsoft.AspNetCore.Identity;

namespace Mvc.Identity101.Data.Entites;

public class AppUser : IdentityUser
{
    public string? City { get; set; }
    public string? imgPath { get; set; }
    
    
    // Navigation properties
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<UserPhoto> Gallery { get; set; } = new List<UserPhoto>();
    
}