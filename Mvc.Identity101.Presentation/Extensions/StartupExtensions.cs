using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mvc.Identity101.Data;
using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(optionsAction: opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
        });

        services.Configure<DataProtectionTokenProviderOptions>( opt =>
        {
            opt.TokenLifespan = TimeSpan.FromHours(1);
        });
            
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            // options.SignIn.RequireConfirmedAccount = true;
            // options.SignIn.RequireConfirmedEmail = true;
            // options.SignIn.RequireConfirmedPhoneNumber = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            // options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
        })
        .AddPasswordValidator<CustomValidations.CustomPasswordValidation>()
        .AddUserValidator<CustomValidations.CustomUserValidation>()
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<AppDbContext>();

        return services;
    }
}