using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.TagHelpers;

[HtmlTargetElement("navbar-profile-img")]
public class ProfileImgTagHelper : TagHelper
{
    public string DefaultStyle { get; set; } =
        "width: 75px; border-radius: 40%; object-fit: cover;aspect-ratio: 1/1"; // Varsayılan stil

    public string DefaultClass { get; set; }
    public string imgPath { get; set; } = "/img/default.jpg";
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProfileImgTagHelper(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

        if (userName != null)
        {
            var d = _userManager.FindByNameAsync(userName).Result;
            if (d != null)
            {
                imgPath = d.imgPath!;
            }
        }

        if (imgPath == null)
        {
            imgPath = "/img/default.jpg";
        }

        output.TagName = "img"; // Tag adını img olarak ayarlıyoruz
        output.Attributes.SetAttribute("src", imgPath); // src özniteliğini ayarlıyoruz
        output.Attributes.SetAttribute("alt", "Profile Image"); // alt özniteliğini ayarlıyoruz
        output.Attributes.SetAttribute("class", DefaultClass); // class özniteliğini ayarlıyoruz
        output.Attributes.SetAttribute("style", DefaultStyle); // Stil özniteliğini ayarlıyoruz
    }
}