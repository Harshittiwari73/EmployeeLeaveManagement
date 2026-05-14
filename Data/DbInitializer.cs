using Microsoft.EntityFrameworkCore;
using EmployeeLeaveManagement.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeLeaveManagement.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<EmployeeLeaveManagementContext>();

                // Create database automatically
                context.Database.EnsureCreated();

                // Roles
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role { RoleName = "Admin" },
                        new Role { RoleName = "Manager" },
                        new Role { RoleName = "Employee" }
                    );

                    context.SaveChanges();
                }

                // Admin User
                if (!context.Users.Any(u => u.Email == "admin@example.com"))
                {
                    var adminRole = context.Roles
                        .FirstOrDefault(r => r.RoleName == "Admin");

                    if (adminRole != null)
                    {
                        context.Users.Add(new User
                        {
                            Username = "Admin",
                            Email = "admin@example.com",
                            PhoneNumber = "1234567890",
                            Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                            RoleId = adminRole.RoleId,
                            IsActive = true,
                            CreatedDate = DateTime.Now
                        });

                        context.SaveChanges();
                    }
                }

                // Permissions
                try
                {
                    var dbModules = new Dictionary<string, List<string>>
                    {
                        { "Dashboard", new List<string> { "View" } },

                        { "Employee", new List<string>
                            { "View", "Create", "Edit", "Delete" }
                        },

                        { "Role", new List<string>
                            { "View", "Create", "Edit", "Delete" }
                        },

                        { "User", new List<string>
                            { "View", "Create", "Edit", "Delete" }
                        },

                        { "LeaveRequest", new List<string>
                            { "View", "Create", "Approve", "Reject" }
                        }
                    };

                    foreach (var module in dbModules)
                    {
                        foreach (var action in module.Value)
                        {
                            var permKey = $"{module.Key}.{action}";

                            if (!context.Permissions
                                .Any(p => p.PermissionKey == permKey))
                            {
                                context.Permissions.Add(new Permission
                                {
                                    Module = module.Key,
                                    ModuleName = "",
                                    PermissionName = action,
                                    PermissionKey = permKey
                                });
                            }
                        }
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB Insert Error: " + ex.Message);
                }

                // Admin Permissions
                var adminRoleObj = context.Roles
                    .FirstOrDefault(r => r.RoleName == "Admin");

                if (adminRoleObj != null)
                {
                    var allPerms = context.Permissions.ToList();

                    var existingAdminPerms = context.RolePermissions
                        .Where(rp => rp.RoleId == adminRoleObj.RoleId)
                        .Select(rp => rp.PermissionId)
                        .ToList();

                    foreach (var p in allPerms)
                    {
                        if (!existingAdminPerms.Contains(p.PermissionId))
                        {
                            context.RolePermissions.Add(
                                new RolePermission
                                {
                                    RoleId = adminRoleObj.RoleId,
                                    PermissionId = p.PermissionId,
                                    IsAllowed = true
                                });
                        }
                    }

                    context.SaveChanges();
                }

                // Manager Permissions
                var managerRoleObj = context.Roles
                    .FirstOrDefault(r => r.RoleName == "Manager");

                if (managerRoleObj != null)
                {
                    var managerPermKeys = new[]
                    {
                        "Employee.View",
                        "Employee.Create",
                        "Employee.Edit",
                        "LeaveRequest.View",
                        "LeaveRequest.Approve",
                        "LeaveRequest.Reject",
                        "Dashboard.View"
                    };

                    var managerPerms = context.Permissions
                        .Where(p => managerPermKeys.Contains(p.PermissionKey))
                        .ToList();

                    var existingManagerPerms = context.RolePermissions
                        .Where(rp => rp.RoleId == managerRoleObj.RoleId)
                        .Select(rp => rp.PermissionId)
                        .ToList();

                    foreach (var p in managerPerms)
                    {
                        if (!existingManagerPerms.Contains(p.PermissionId))
                        {
                            context.RolePermissions.Add(
                                new RolePermission
                                {
                                    RoleId = managerRoleObj.RoleId,
                                    PermissionId = p.PermissionId,
                                    IsAllowed = true
                                });
                        }
                    }

                    context.SaveChanges();
                }

                // Employee Permissions
                var employeeRoleObj = context.Roles
                    .FirstOrDefault(r => r.RoleName == "Employee");

                if (employeeRoleObj != null)
                {
                    var employeePermKeys = new[]
                    {
                        "Employee.View",
                        "LeaveRequest.View",
                        "LeaveRequest.Create",
                        "Dashboard.View"
                    };

                    var employeePerms = context.Permissions
                        .Where(p => employeePermKeys.Contains(p.PermissionKey))
                        .ToList();

                    var existingEmployeePerms = context.RolePermissions
                        .Where(rp => rp.RoleId == employeeRoleObj.RoleId)
                        .Select(rp => rp.PermissionId)
                        .ToList();

                    foreach (var p in employeePerms)
                    {
                        if (!existingEmployeePerms.Contains(p.PermissionId))
                        {
                            context.RolePermissions.Add(
                                new RolePermission
                                {
                                    RoleId = employeeRoleObj.RoleId,
                                    PermissionId = p.PermissionId,
                                    IsAllowed = true
                                });
                        }
                    }

                    context.SaveChanges();
                }

                // Employees
                if (!context.Employees.Any())
                {
                    context.Employees.AddRange(
                        new Employee
                        {
                            EmployeeName = "John Doe",
                            Email = "john@example.com",
                            Department = "IT",
                            JoiningDate = DateTime.Now.AddYears(-2)
                        },

                        new Employee
                        {
                            EmployeeName = "Jane Smith",
                            Email = "jane@example.com",
                            Department = "HR",
                            JoiningDate = DateTime.Now.AddYears(-1)
                        }
                    );

                    context.SaveChanges();
                }

                // Leave Types
                if (!context.LeaveTypes.Any())
                {
                    context.LeaveTypes.AddRange(
                        new LeaveType { LeaveTypeName = "Sick Leave" },
                        new LeaveType { LeaveTypeName = "Casual Leave" },
                        new LeaveType { LeaveTypeName = "Annual Leave" }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}