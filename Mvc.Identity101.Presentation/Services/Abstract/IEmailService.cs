using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Services.Abstract;

public interface IEmailService
{
    Task SendResetPasswordEmailAsync(string toUserMail, string resetPasswordLink);
}