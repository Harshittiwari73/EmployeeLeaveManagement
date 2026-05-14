using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagement.Filters;

public class PermissionAuthorizeAttribute : TypeFilterAttribute
{
    public PermissionAuthorizeAttribute(string moduleName, string permissionName) 
        : base(typeof(PermissionAuthorizationFilter))
    {
        Arguments = new object[] { moduleName, permissionName };
    }
}
