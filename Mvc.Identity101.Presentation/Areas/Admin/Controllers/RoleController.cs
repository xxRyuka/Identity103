using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Mvc.Identity101.Areas.Admin.Data.Dto;
using Mvc.Identity101.Data.Entites;

namespace Mvc.Identity101.Areas.Admin.Controllers;

[Authorize(Roles = "god")]
[Area("Admin")]
public class RoleController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET
    public IActionResult Index()
    {
        var dtoList = _roleManager.Roles.Select(x => new ViewRoleDto
            {
            Name = x.Name,
            Id = x.Id
        }).ToList();
        return View(dtoList);
    }

    [Authorize(Roles = "role manipulation")]
    public IActionResult AddRole()
    {
        return View();
    }

    [Authorize(Roles = "role manipulation")]
    [HttpPost]
    public async Task<IActionResult> AddRole(AddRoleDto request)
    {
        if (ModelState.IsValid)
        {
            var newrole = await _roleManager.CreateAsync(new AppRole
            {
                Name = request.Name
            });

            if (newrole.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", newrole.Errors.First().Description);
        }

        return View();
    }

    [Authorize(Roles = "role manipulation")]
    public IActionResult EditRole(string id)
    {
        var role = _roleManager.Roles.SingleOrDefault(x => x.Id == id);
        return View(new EditRoleDto()
        {
            Name = role.Name,
        });
    }

    [Authorize(Roles = "role manipulation")]
    [HttpPost]
    public async Task<IActionResult> EditRole(EditRoleDto request)
    {
        if (ModelState.IsValid)
        {
            var oldRole = _roleManager.Roles.SingleOrDefault(x => x.Id == request.Id);
            if (oldRole == null)
            {
                return NotFound();
            }

            oldRole.Name = request.Name;

            await _roleManager.UpdateAsync(oldRole);
            return RedirectToAction(nameof(Index));
        }

        return View(request);
    }

    [Authorize(Roles = "role manipulation")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = _roleManager.Roles.SingleOrDefault(x => x.Id == id);
        if (role == null)
        {
            return NotFound();
        }

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [Authorize(Roles = "role manipulation")]
    public async Task<IActionResult> AssignRole(string id)
    {
        var user = _userManager.Users.SingleOrDefault(x => x.Id == id);
        ViewBag.UserName = user.UserName;
        ViewBag.UserId = user.Id;
        // var dto = new UserListDto()
        // {
        //     Id = user.Id,
        //     UserName = user.UserName,
        //     Email = user.Email,
        //
        // };

        var roleList = await _roleManager.Roles.ToListAsync();
        var userRoleList = await _userManager.GetRolesAsync(user);
        var roleDtoList = new List<AssignRoleDto>();

        foreach (var role in roleList)
        {
            var AssignRoleDto = new AssignRoleDto() { Name = role.Name, Id = role.Id };

            if (userRoleList.Contains(role.Name))
            {
                AssignRoleDto.Exist = true;
            }

            roleDtoList.Add(AssignRoleDto);
        }

        return View(roleDtoList);
    }

    [Authorize(Roles = "role manipulation")]
    [HttpPost]
    public async Task<IActionResult> AssignRole(string UserId, List<AssignRoleDto> request)
    {
        var user = _userManager.Users.SingleOrDefault(x => x.Id == UserId);


        foreach (var role in request)
        {
            if (role.Exist)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, role.Name);
            }
        }


        return RedirectToAction("UserList", "Home");
    }
}