using Microsoft.AspNetCore.Identity;
using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.CustomValidations;

public class CustomPasswordValidation : IPasswordValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
    {
        var errList = new List<IdentityError>();
        if (password.ToLower().Contains(user.UserName.ToLower()))
        {
            errList.Add(new IdentityError { Code = "PasswordContainsUserName", Description = "Pw Contains ur username wtf" });
        }

        if (password.ToLower().StartsWith("1234"))
        {
            errList.Add(new IdentityError { Code = "PasswordContainsAricmetical", Description = "Pw Contains aricmetical" });
        }

        if (errList.Any())
        {
            return Task.FromResult(IdentityResult.Failed(errList.ToArray()));
        }
        return Task.FromResult(IdentityResult.Success);
    }
}