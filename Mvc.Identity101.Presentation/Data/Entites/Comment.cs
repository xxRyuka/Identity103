namespace Mvc.Identity101.Data.Entites;

// one to many olacak yani bir postun birden cok yorumu olabilir fakat bir yorum sadece bir posta ait olabilir !! 
public class Comment
{
    public int CommentId { get; set; }
    public string Content { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Default value for CreatedDate
    
    
    // Navigation properties
    public string? UserId { get; set; } // Foreign key to AppUser
    public AppUser? User { get; set; } // Navigation property to AppUser
    
    public int UserPhotoId { get; set; } // Foreign key to Post
    public UserPhoto UserPhoto { get; set; } // Navigation property to Post

    
}
