namespace Mvc.Identity101.Extensions;

public static class CookieExtensions
{
    public static IServiceCollection AddExtendedCookieConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureApplicationCookie(opt =>
        {
            opt.LoginPath = "/Home/SignIn";
            opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
            opt.ExpireTimeSpan = TimeSpan.FromMinutes(15);
            opt.SlidingExpiration = true;
            opt.LogoutPath = new PathString("/Member/SignOut");
            opt.ReturnUrlParameter = "returnURL"; // customize edebiliyoz
        });
        
        return services;
    }
}