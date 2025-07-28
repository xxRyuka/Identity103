using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Data.Dto;

public class CommentDto
{
    public int CommentId { get; set; }
    public string Content { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Default value for CreatedDate
    
    
    // Navigation properties
    public string? UserId { get; set; } // Foreign key to AppUser
    public string? UserName { get; set; } // Navigation property to AppUser
    
    public int UserPhotoId { get; set; } // Foreign key to Post
    public UserPhoto UserPhoto { get; set; } // Navigation property to Post
}