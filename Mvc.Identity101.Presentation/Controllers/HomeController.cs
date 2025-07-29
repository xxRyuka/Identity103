using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Mvc.Identity101.Data;
using Mvc.Identity101.Data.Dto;
using Mvc.Identity101.Data.Entites;
using Mvc.Identity101.Models;
using Mvc.Identity101.Services.Abstract;

namespace Mvc.Identity101.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager, IEmailService emailService, IMemoryCache cache, AppDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _cache = cache;
        _context = context;
    }


    [HttpPost]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        // returnUrl = returnUrl ?? Url.Content("~/");
        var redirectUrl = Url.Action("ExternalLoginCallback", "Home", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }


    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(SignIn));
        }

        // Kullanıcıyı bulmaya çalış
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (user == null)
        {
            // Kullanıcı bulunamadı, yeni kullanıcı oluştur
            user = new AppUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email)?.Split("@")[0],
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                imgPath = info.Principal.FindFirstValue("picture")
                    .Replace("=s96-c", "=s1080-c") // Google'dan gelen profil fotoğrafı
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Error");
            }

            
            
            var newClaim = new Claim("ExchangeExpire", DateTime.Now.AddMinutes(1).ToString());
            var newUser = await _userManager.FindByEmailAsync(user.Email!);

            await _userManager.AddClaimAsync(newUser!, newClaim);
            
            
            
            // Kullanıcıyı login et
            
            await _userManager.AddLoginAsync(user, info);
        }

        // Kullanıcıyı oturum açtır
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction(nameof(Index));
    }


    [Authorize(Policy = "TestPolicy")]
    public IActionResult Index()
    {
        return View();
    }

    // [Authorize(Policy = "Nodeirn")]
    public async Task<IActionResult> PublicList()
    {
        var list = await _userManager.Users.ToListAsync();
        var dtoList = list.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName ?? "",
            Email = u.Email ?? "",
            imgPath = string.IsNullOrEmpty(u.imgPath) ? "/img/default.jpg" : u.imgPath
        }).ToList();
        return View(dtoList);
    }


    [Authorize(Policy = "ExchangePolicy")]
    public async Task<IActionResult> Exchange()
    {
        // var list = await _userManager.Users.ToListAsync();
        return View();
    }

    [Authorize]
    public async Task<IActionResult> PostDetail(PostDetailDto dto)
    {

        var post = await _context.UserPhotos
            .Include(p => p.Comments)
            .ThenInclude(c => c.User) // Kullanıcı bilgilerini de dahil et
            .AsNoTracking() // Performans için izleme devre dışı bırakıldı
            .FirstOrDefaultAsync(p => p.Id == dto.PhotoId);

        if (post == null)
        {
            return NotFound();
        }

        dto = new PostDetailDto()
        {
            ImgPath = post!.imgPath,
            Comments = post.Comments,
            Description = post.Description,
            PhotoId = post.Id

        };

        return View(dto);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(DropCommentDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);
        
        var photo = await _context.UserPhotos
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == request.PhotoId);
        
        photo.Comments.Add(new Comment()
        {
            Content = request.Content,
            UserId = userId,
            CreatedDate = request.CreatedDate,
            
        });
        // photo.UserId = userId;
        await _context.SaveChangesAsync();
        
        return RedirectToAction("PostDetail", new { photoId = request.PhotoId });
    }
    
    
    
    

    [HttpGet]
    public async Task<IActionResult> UserDetail(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest();

        // Kullanıcıyı ve galeriyi Include ile çek
        var user = await _userManager.Users
            .Include(u => u.Gallery)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        var model = new UserDetailViewModel
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            Email = user.Email ?? "",
            ImgPath = string.IsNullOrEmpty(user.imgPath) ? "/img/default.jpg" : user.imgPath
        };

        // Galeri foto’larını map et
        model.Photos = user.Gallery
            .OrderByDescending(p => p.UploadDate)
            .Select(p => new UserPhotoViewModel
            {
                Id = p.Id,
                ImgPath = string.IsNullOrEmpty(p.imgPath) ? "/img/default.jpg" : p.imgPath,
                Description = p.Description
            })
            .ToList();

        return View(model);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignUp()
    {
        if (User.Identity.IsAuthenticated && User.Identity != null)
        {
            return RedirectToAction("Index");
        }

        return View();
    }

    [ValidateAntiForgeryToken] // Bu gerekli işlemlerin tek koldan ypaıldıgını dogrulayarak
// CSRF saldirilarina korunaklı hale getirir
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpDto dto)
    {
        if (ModelState.IsValid)
        {
            var result = await _userManager.CreateAsync(new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.Phone
            }, password: dto.Password);

            if (result.Succeeded)
            {
                var newClaim = new Claim("ExchangeExpire", DateTime.Now.AddMinutes(1).ToString());
                var newUser = await _userManager.FindByEmailAsync(dto.Email);

                await _userManager.AddClaimAsync(newUser!, newClaim);

                TempData["Message"] = $"User : {dto.UserName} created successfully";
                return RedirectToAction("SignUp");
            }

            foreach (IdentityError error in result.Errors)
            {
                //string empty deme sebebimiz direk all errors divinde göstermek 
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SignIn()
    {
        if (User.Identity.IsAuthenticated && User.Identity != null)
        {
            return RedirectToAction("index");
        }

        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> SignIn(SignInDto dto, string? returnURL) // Simdilik Return url ile ugrasmicaz
    {
        if (ModelState.IsValid)
        {
            var entity = await _userManager.FindByEmailAsync(dto.Email);
            if (entity == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamad<UNK>");
                return View(dto);
            }

            var result = await _signInManager.PasswordSignInAsync(entity, dto.Password, dto.RememberMe, true);
            if (result.Succeeded)
            {
                // return Redirect(returnURL ?? "/"); // Bu yanlıs phising olabileceği için isLocalUrl kullancaz
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnURL) && Url.IsLocalUrl(returnURL))
                    {
                        return Redirect(returnURL);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
            else if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(entity);
                var remaining = lockoutEnd.Value.UtcDateTime - DateTime.UtcNow;
                var minutes = (int)Math.Ceiling(remaining.TotalMinutes);

                ModelState.AddModelError(string.Empty,
                    $"Hesabınız kilitlenmiştir. {minutes} dakika sonra tekrar deneyiniz.");
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz giriş denemesi.");
            }
        }

        return View(dto);
    }


    public async Task<IActionResult> ForgotPassword()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
    {
        if (ModelState.IsValid)
        {
            string cacheKey = $"ForgotPw_{request.Email}"; // her mailin kendi cachesi olacak :)
            if (_cache.TryGetValue(cacheKey, out _))
            {
                ModelState.AddModelError(String.Empty, "Zaten Sifirlama Maili " +
                                                       $" {request.Email} adresinize" +
                                                       "Gonderildi" +
                                                       "Mail Adresinizi Kontrol Ediniz" +
                                                       ",Lütfen Daha Sonra tekrar deneyiniz.");
                return View(request);
            }

            var hasUser = await _userManager.FindByEmailAsync(request.Email);
            if (hasUser == null)
            {
                ModelState.AddModelError("", "Bu e postaya ait bir hesap bulunamamiştir ");
                return View(request);
            }

            string pwToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            string pwResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, token = pwToken },
                protocol: HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmailAsync(request.Email, pwResetLink);

            _cache.Set(cacheKey, true, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                Priority = CacheItemPriority.Normal,
                // SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            TempData["Message"] = "Maile sifre yenileme linki iletildi";
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword(string userId, string token)
    {
        var model = new ResetPasswordDto()
        {
            UserId = userId,
            Token = token
        };
        return View(model);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
    {
        if (request.Token != null)
        {
            var hasUser = await _userManager.FindByIdAsync(request.UserId);
            if (hasUser != null)
            {
                var result = await _userManager.ResetPasswordAsync(hasUser, request.Token, request.Password);

                if (result.Succeeded)
                {
                    TempData["Message"] =
                        "Sifreniz Basariyla Değiştirilmiştir"; // Kanka Bunla ugrasma git bi sifre değiştirme işlemi başarili sayfasina gonder 
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    TempData["Message"] = "Hay Aksi bir sorun oluştu ";
                }

                return View();
            }

            ModelState.AddModelError(key: string.Empty, "Kullanıcı bulunamadi.");
            return View(request);
        }

        var dto = request;

        return View();
    }


    public IActionResult AccessDenied(string reason)
    {
        reason = HttpContext.Items["AuthFailReason"]?.ToString();
        ViewBag.Reason = reason;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}