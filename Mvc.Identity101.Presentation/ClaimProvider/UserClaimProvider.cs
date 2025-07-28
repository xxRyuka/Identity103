using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.ClaimProvider;

public class UserClaimProvider : IClaimsTransformation
{
    private readonly UserManager<AppUser> _userManager;

    public UserClaimProvider(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {

        var identityUser = (ClaimsIdentity)principal.Identity;
        
        // IIdentity interfacesi seklindeydi elimizde 
        
        var user = await _userManager.FindByNameAsync(identityUser.Name);

        if (string.IsNullOrWhiteSpace(user?.City))
        {
            return principal;
        }

        if (principal.HasClaim(x => x.Type != "city")) // city yok ise claimleri arasinda 
        {
            Claim newClaim = new Claim("city", user.City);
            identityUser.AddClaim(newClaim);
        }
        
        
        return principal;
    }
}