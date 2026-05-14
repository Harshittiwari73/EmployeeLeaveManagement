using EmployeeLeaveManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeLeaveManagement.Filters;

namespace EmployeeLeaveManagement.Controllers;

[Authorize]
public class RolesController : Controller
{
    private readonly EmployeeLeaveManagementContext _context;
    public RolesController(EmployeeLeaveManagementContext context)
    {
        _context = context;
    }

    [PermissionAuthorize("Role", "View")]
    public async Task<IActionResult> Index(string? roleName, string? status)
    {
        IQueryable<Role> query = _context.Roles;

        if (!string.IsNullOrEmpty(roleName))
        {
            query = query.Where(r => r.RoleName.Contains(roleName));
        }
        if (!string.IsNullOrEmpty(status))
        {
            bool isActive = status.Equals("active", StringComparison.OrdinalIgnoreCase);
            query = query.Where(r => r.IsActive == isActive);
        }

        ViewBag.RoleName = roleName;
        ViewBag.Status = status;

        return View(await query.ToListAsync());
    }

    [HttpGet]
    [PermissionAuthorize("Role", "View")]
    public async Task<IActionResult> GetRoleWithPermissions(int? id)
    {
        var allPermissions = await _context.Permissions.ToListAsync();
        var model = new RoleViewModel();
        if (id.HasValue && id > 0)
        {
            var role = await _context.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.RoleId == id);
            if (role == null) return NotFound();
            model.RoleId = role.RoleId;
            model.RoleName = role.RoleName;
            model.IsActive = role.IsActive;
            model.Modules = allPermissions.GroupBy(p => p.Module).Select(g => new ModulePermissionViewModel
            {
                ModuleName = g.Key,
                Permissions = g.Select(p => new PermissionItemViewModel
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    IsAllowed = role.RolePermissions.Any(rp => rp.PermissionId == p.PermissionId && rp.IsAllowed)
                }).ToList()
            }).ToList();
        }
        else
        {
            model.Modules = allPermissions.GroupBy(p => p.Module).Select(g => new ModulePermissionViewModel
            {
                ModuleName = g.Key,
                Permissions = g.Select(p => new PermissionItemViewModel
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    IsAllowed = false
                }).ToList()
            }).ToList();
        }
        return Json(model);
    }

    [HttpPost]
    [PermissionAuthorize("Role", "Edit")]
    public async Task<IActionResult> SaveRole([FromBody] RoleViewModel model)
    {
        Role role;
        if (model.RoleId > 0)
        {
            role = await _context.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.RoleId == model.RoleId);
            if (role == null) return NotFound();
            role.RoleName = model.RoleName;
            role.IsActive = model.IsActive;
            _context.RolePermissions.RemoveRange(role.RolePermissions);
        }
        else
        {
            role = new Role { RoleName = model.RoleName, IsActive = model.IsActive };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(); 
        }

        foreach (var module in model.Modules)
        {
            foreach (var perm in module.Permissions)
            {
                if (perm.IsAllowed)
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.RoleId,
                        PermissionId = perm.PermissionId,
                        IsAllowed = true
                    });
                }
            }
        }
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    [PermissionAuthorize("Role", "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();
        var permissions = _context.RolePermissions.Where(rp => rp.RoleId == id);
        _context.RolePermissions.RemoveRange(permissions);
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }
}
