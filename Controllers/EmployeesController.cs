using EmployeeLeaveManagement.Models;
using EmployeeLeaveManagement.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeLeaveManagement.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly EmployeeLeaveManagementContext _context;

        public EmployeesController(EmployeeLeaveManagementContext context)
        {
            _context = context;
        }
        [PermissionAuthorize("Employee", "View")]
        public async Task<IActionResult> Index(string? name, string? email, string? department, DateTime? joiningDate, string? status)
        {
            IQueryable<Employee> query = _context.Employees;

            if (User.IsInRole("Employee"))
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                query = query.Where(e => e.Email == userEmail);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.EmployeeName.Contains(name));
            }
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(e => e.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department.Contains(department));
            }
            if (joiningDate.HasValue)
            {
                query = query.Where(e => e.JoiningDate == joiningDate.Value);
            }
            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status.Equals("active", StringComparison.OrdinalIgnoreCase);
                query = query.Where(e => e.IsActive == isActive);
            }

            ViewBag.Name = name;
            ViewBag.Email = email;
            ViewBag.Department = department;
            ViewBag.JoiningDate = joiningDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            return View(await query.ToListAsync());
        }

        [HttpPost]
        [PermissionAuthorize("Employee", "Create")]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                bool emailExists = await _context.Employees
                    .AnyAsync(x => x.Email == employee.Email);
                if (emailExists)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }
                employee.IsActive = employee.IsActive;
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "Employee added successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet]
        [PermissionAuthorize("Employee", "View")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Employee not found"
                    });
                }
                return Json(new
                {
                    success = true,
                    data = employee
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [PermissionAuthorize("Employee", "Edit")]
        public async Task<IActionResult> Edit(Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid data"
                    });
                }
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(x => x.EmployeeId == employee.EmployeeId);
                if (existingEmployee == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Employee not found"
                    });
                }
                bool emailExists = await _context.Employees
                    .AnyAsync(x => x.Email == employee.Email &&
                                   x.EmployeeId != employee.EmployeeId);

                if (emailExists)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }
                existingEmployee.EmployeeName = employee.EmployeeName;
                existingEmployee.Email = employee.Email;
                existingEmployee.Department = employee.Department;
                existingEmployee.JoiningDate = employee.JoiningDate;
                existingEmployee.IsActive = employee.IsActive;
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "Employee updated successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [PermissionAuthorize("Employee", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Employee not found"
                    });
                }
                var leaveRequests = await _context.LeaveRequests
                    .Where(x => x.EmployeeId == id)
                    .ToListAsync();

                _context.LeaveRequests.RemoveRange(leaveRequests);
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "Employee deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}