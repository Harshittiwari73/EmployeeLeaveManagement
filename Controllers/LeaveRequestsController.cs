using EmployeeLeaveManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EmployeeLeaveManagement.Filters;

namespace EmployeeLeaveManagement.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly EmployeeLeaveManagementContext _context;
        public LeaveRequestsController(EmployeeLeaveManagementContext context)
        {
            _context = context;
        }
        [PermissionAuthorize("LeaveRequest", "View")]
        public async Task<IActionResult> Index(string? employee, string? leaveType, DateOnly? fromDate, string? status)
        {
            ViewBag.Employees = _context.Employees.ToList();
            ViewBag.LeaveTypes = _context.LeaveTypes.ToList();

            IQueryable<LeaveRequest> query = _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType);

            if (!string.IsNullOrEmpty(employee))
            {
                query = query.Where(l => l.Employee.EmployeeName.Contains(employee));
            }
            if (!string.IsNullOrEmpty(leaveType))
            {
                query = query.Where(l => l.LeaveType.LeaveTypeName.Contains(leaveType));
            }
            if (fromDate.HasValue)
            {
                query = query.Where(l => l.FromDate >= fromDate.Value);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.Status == status);
            }

            ViewBag.EmployeeSearch = employee;
            ViewBag.LeaveTypeSearch = leaveType;
            ViewBag.FromDateSearch = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.StatusSearch = status;

            return View(await query.ToListAsync());
        }
        public IActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(
                _context.Employees,
                "EmployeeId",
                "EmployeeName"
            );
            ViewBag.LeaveTypeId = new SelectList(
                _context.LeaveTypes,
                "LeaveTypeId",
                "LeaveTypeName"
            );
            return View();
        }

        [HttpPost]
        [PermissionAuthorize("LeaveRequest", "Create")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveRequest leaveRequest)
        {
            try
            {
                if (leaveRequest == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid data"
                    });
                }
                leaveRequest.Status = "Pending";
                leaveRequest.AppliedDate = DateTime.Now;
                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("LeaveRequest", "Create")]
        public async Task<IActionResult> Create(LeaveRequest leaveRequest)
        {
            if (ModelState.IsValid)
            {
                leaveRequest.Status = "Pending";
                leaveRequest.AppliedDate = DateTime.Now;
                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "EmployeeName", leaveRequest.EmployeeId);
            ViewBag.LeaveTypeId = new SelectList(_context.LeaveTypes, "LeaveTypeId", "LeaveTypeName", leaveRequest.LeaveTypeId);
            return View(leaveRequest);
        }
    }
}