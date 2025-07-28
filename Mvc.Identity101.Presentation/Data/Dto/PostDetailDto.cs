using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Data.Dto;

public class PostDetailDto
{
    public int PhotoId { get; set; }
    public string ImgPath { get; set; }
    public string Description { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}