using EmployeeLeaveManagement.Models;
using EmployeeLeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using EmployeeLeaveManagement.Filters;

namespace EmployeeLeaveManagement.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [PermissionAuthorize("User", "View")]
    public async Task<IActionResult> Index(string? searchTerm, string? username, string? email, string? phone, string? role, string? status, string? sortColumn, string? sortOrder, int pageNumber = 1)
    {
        var model = await _userService.GetUsersListAsync(searchTerm, username, email, phone, role, status, sortColumn, sortOrder, pageNumber, 10);
        ViewBag.Roles = new SelectList(await _userService.GetRolesAsync(), "RoleName", "RoleName"); // Note: changed to RoleName as value for easier filtering if needed, or keep as RoleId.
        return View(model);
    }

    [HttpGet]
    [PermissionAuthorize("User", "Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(await _userService.GetRolesAsync(), "RoleId", "RoleName");
        return View(new UserViewModel());
    }

    [HttpPost]
    [PermissionAuthorize("User", "Create")]
    public async Task<IActionResult> Create(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (!await _userService.IsEmailUniqueAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
            }
            else if (!await _userService.IsPhoneUniqueAsync(model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number is already in use.");
            }
            else
            {
                var result = await _userService.RegisterUserAsync(model);
                if (result) return RedirectToAction(nameof(Index));
            }
        }
        ViewBag.Roles = new SelectList(await _userService.GetRolesAsync(), "RoleId", "RoleName", model.RoleId);
        return View(model);
    }

    [HttpPost]
    [PermissionAuthorize("User", "Create")]
    public async Task<IActionResult> SaveUser(UserViewModel model)
    {
        if (model.UserId == 0 && string.IsNullOrEmpty(model.Password))
        {
             return Json(new { success = false, message = "Password is required for new users." });
        }
        if (!await _userService.IsEmailUniqueAsync(model.Email, model.UserId))
            return Json(new { success = false, message = "Email already in use." });
        bool result;
        if (model.UserId > 0)
        {
            result = await _userService.UpdateUserAsync(model);
        }
        else
        {
            result = await _userService.RegisterUserAsync(model);
        }

        return Json(new { success = result });
    }

    [HttpGet]
    [PermissionAuthorize("User", "View")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        var model = new UserViewModel
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            IsActive = user.IsActive
        };
        return Json(model);
    }

    [HttpGet]
    [PermissionAuthorize("User", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        var model = new UserViewModel
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            IsActive = user.IsActive,
            Password = "********" 
        };
        ViewBag.Roles = new SelectList(await _userService.GetRolesAsync(), "RoleId", "RoleName", model.RoleId);
        return View(model);
    }

    [HttpPost]
    [PermissionAuthorize("User", "Edit")]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (!await _userService.IsEmailUniqueAsync(model.Email, model.UserId))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
            }
            else
            {
                var result = await _userService.UpdateUserAsync(model);
                if (result) return RedirectToAction(nameof(Index));
            }
        }
        ViewBag.Roles = new SelectList(await _userService.GetRolesAsync(), "RoleId", "RoleName", model.RoleId);
        return View(model);
    }

    [HttpPost]
    [PermissionAuthorize("User", "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteUserAsync(id);
        return Json(new { success = true });
    }

    [HttpPost]
    [PermissionAuthorize("User", "Edit")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _userService.ToggleUserStatusAsync(id);
        return Json(new { success = result });
    }
}
