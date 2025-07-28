using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Data.Dto;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? imgPath { get; set; } = "/img/default.png";

    public List<UserPhoto>? Photos { get; set; } =  new List<UserPhoto>();
}