namespace Mvc.Identity101.Data.Dto;

public class DropCommentDto
{
    public string Content { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Default value for CreatedDate


    // Navigation properties
    public string? UserId { get; set; } // Foreign key to AppUser
    public int PhotoId { get; set; }
}