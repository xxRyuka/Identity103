using System.ComponentModel.DataAnnotations;

namespace Mvc.Identity101.Data.Dto;

public class AddPostDto
{

    
    [Required]
    public IFormFile? img { get; set; }

    public string? description { get; set; }
}