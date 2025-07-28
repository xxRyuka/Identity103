using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Identity101.Areas.Admin.Data.Dto;
using Mvc.Identity101.Data;
using Mvc.Identity101.Data.Entites;
using Mvc.Identity101.Services.Abstract;
using Mvc.Identity101.Services.Concrete;
using Mvc.Identity101.Services.Data;

namespace Mvc.Identity101.Areas.Admin.Controllers;
[Authorize(Roles = "god")]
[Area("Admin")]
public class HomeController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IProfileImageService _profileImageService;
    private readonly AppDbContext _context;

    public HomeController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IProfileImageService profileImageService, AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _profileImageService = profileImageService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.UserCount = _userManager.Users.Count();
        ViewBag.RoleCount = _roleManager.Roles.Count();
        return View();
    }

    public async Task<IActionResult> UserList()
    {
        var userList = await _userManager.Users.ToListAsync();
        List<UserListDto> DtoUserList = new List<UserListDto>();
        foreach (var user in userList)
        {
            DtoUserList.Add(new()
            {
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id,
                imgPath = string.IsNullOrWhiteSpace(user.imgPath) ? "/img/default.jpg" : user.imgPath,
                PhoneNumber = user.PhoneNumber,
                Roles =  _userManager.GetRolesAsync(user).Result.ToList()
            });
        }


        return View(DtoUserList);
    }
    
    
    [HttpPost]
    public async Task<IActionResult> RemoveUser(string id)
    {
        // var OLDuser = await _userManager.FindByIdAsync(id);
        var user =  _context.Users
            .Include(us => us.Gallery)
            .ThenInclude(us => us.Comments)
            .FirstOrDefault(x => x.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        foreach (var photo in user.Gallery)
        {
            foreach (var comment in photo.Comments)
            {
                _context.Comments.Remove(comment);
            }
            await _profileImageService.DeleteImageAsync(user.Id, photo.imgPath, ImageType.GalleryPhoto);
            var ptoto = await _context.UserPhotos.FirstOrDefaultAsync(x => x.Id == photo.Id);
            _context.UserPhotos.Remove(ptoto);
        }

        await _context.SaveChangesAsync();
        
        var result = await _userManager.DeleteAsync(user);
        return RedirectToAction("UserList");
    }
}