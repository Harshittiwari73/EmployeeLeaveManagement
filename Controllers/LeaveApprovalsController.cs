using EmployeeLeaveManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EmployeeLeaveManagement.Filters;

namespace EmployeeLeaveManagement.Controllers
{
    [Authorize]
    public class LeaveApprovalsController : Controller
    {
        private readonly EmployeeLeaveManagementContext _context;
        public LeaveApprovalsController(EmployeeLeaveManagementContext context)
        {
            _context = context;
        }

        [PermissionAuthorize("LeaveRequest", "View")]
        public async Task<IActionResult> Index(string? employee, string? leaveType, string? status)
        {
            IQueryable<LeaveRequest> query = _context.LeaveRequests
                .Include(x => x.Employee)
                .Include(x => x.LeaveType);

            if (!string.IsNullOrEmpty(employee))
            {
                query = query.Where(l => l.Employee.EmployeeName.Contains(employee));
            }
            if (!string.IsNullOrEmpty(leaveType))
            {
                query = query.Where(l => l.LeaveType.LeaveTypeName.Contains(leaveType));
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.Status == status);
            }

            ViewBag.EmployeeSearch = employee;
            ViewBag.LeaveTypeSearch = leaveType;
            ViewBag.StatusSearch = status;

            return View(await query.ToListAsync());
        }

        [HttpPost]
        [PermissionAuthorize("LeaveRequest", "Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave != null)
            {
                leave.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [PermissionAuthorize("LeaveRequest", "Reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave != null)
            {
                leave.Status = "Rejected";

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}