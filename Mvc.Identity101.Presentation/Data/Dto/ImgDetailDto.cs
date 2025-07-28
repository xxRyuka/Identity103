using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Data.Dto;

public class ImgDetailDto
{
    public int? id { get; set; }
    public string Description { get; set; }

    public string ImgPath { get; set; }

    public DateTime UploadDate { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}