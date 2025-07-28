using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mvc.Identity101.ClaimProvider;
// using Mvc.Identity101.CustomMiddlewares;
using Mvc.Identity101.Data;
using Mvc.Identity101.Data.Entites;
using Mvc.Identity101.Extensions;
using Mvc.Identity101.Requirements;
using Mvc.Identity101.Services.Abstract;
using Mvc.Identity101.Services.Concrete;
using Mvc.Identity101.Services.Data.Dto;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCustomizedIdentity(builder.Configuration); // DbContext ve Identity servis kayitlari burda
// extension method olarak

builder.Services.AddMemoryCache(); // Bununla rate limiting yapacağim bir sürü forgot pw gonderirlerse sıkıntı 
// bunu iyi ogren 
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services
    .AddExtendedCookieConfigurations(builder
        .Configuration); // cookileri extension methodla ayri biyerde yazdik ve ekledik buraya 
// amaç program.cs karişmasin 
builder.Services.AddSingleton<IAuthorizationHandler, TestRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireHandler>();
builder.Services.AddScoped<IProfileImageService, ProfileImageService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = IdentityConstants.ApplicationScheme; // cookie authentication
    opt.DefaultChallengeScheme = IdentityConstants.ApplicationScheme; // challenge scheme
    opt.DefaultSignInScheme = IdentityConstants.ExternalScheme; // external provider
}).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    // options.SaveTokens = true; // tokenlari kaydet
    options.Scope.Add("email"); // email scope ekle
    options.Scope.Add("profile"); // profile scope ekle
    // options.Scope.Add("name"); //   bu yokmuski hata veriyor
    options.ClaimActions.MapJsonKey("city", "city"); // claim ekle
    options.ClaimActions.MapJsonKey("picture", "picture"); // picture claim ekle
});


builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Nodeirn", policy => { policy.RequireClaim("city", "Nodeirn"); });

    opt.AddPolicy("ExchangePolicy", policy => { policy.AddRequirements(new ExchangeExpireRequirement()); });

    opt.AddPolicy("TestPolicy", p => { p.AddRequirements(new TestRequirement() { DynamicThresOld = -5 }); });
});

// builder.Services.AddDbContext<AppDbContext>(optionsAction: opt =>
// {
//     opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
//     // opt.UseSqlServer(co);
// });

// builder.Services.AddIdentity<AppUser, AppRole>(options =>
//     {
//         // options.User.AllowedUserNameCharacters="abcdefghi"; // konfigura edebiliriz
//         options.User.RequireUniqueEmail = true;
//         options.Password.RequiredLength = 6;
//         options.Password.RequireNonAlphanumeric = false;
//         // options.SignIn.RequireConfirmedAccount = true;
//         // options.SignIn.RequireConfirmedEmail = true;
//         // options.SignIn.RequireConfirmedPhoneNumber = true;
//         options.Lockout.MaxFailedAccessAttempts = 5;
//         options.Lockout.AllowedForNewUsers = true;
//         options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//
//     })
//     .AddEntityFrameworkStores<AppDbContext>();
// // .AddDefaultTokenProviders() // henuz eklemiyoruz token provideri // refactored with extension 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();


// app.UseMiddleware<StatusCodeCaptureMiddleware>();

// app.Use(async (context, next) =>
// {
//
//     if (context.Response.StatusCode == 403)
//     {
//         var reason = context.Items["AuthFailReason"]?.ToString() ?? "AccessDenied";
//         var redirectUrl = $"/Home/AccessDenied?reason={Uri.EscapeDataString(reason)}";
//
//         context.Response.Redirect(redirectUrl);
//     }
//     await next();
// });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();