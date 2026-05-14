using EmployeeLeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeLeaveManagement.Filters;

public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly string _moduleName;
    private readonly string _permissionName;
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationFilter(string moduleName, string permissionName, IPermissionService permissionService)
    {
        _moduleName = moduleName;
        _permissionName = permissionName;
        _permissionService = permissionService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(roleClaim))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            return;
        }

        bool hasPermission = await _permissionService.HasPermissionAsync(roleClaim, _moduleName, _permissionName);

        if (!hasPermission)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}
