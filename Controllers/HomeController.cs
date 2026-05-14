using EmployeeLeaveManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeLeaveManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly EmployeeLeaveManagementContext _context;
        public HomeController(EmployeeLeaveManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEmployees =
                await _context.Employees.CountAsync();
            ViewBag.TotalLeaveRequests =
                await _context.LeaveRequests.CountAsync();
            ViewBag.ApprovedLeaves =
                await _context.LeaveRequests
                    .CountAsync(x => x.Status == "Approved");
            ViewBag.PendingLeaves =
                await _context.LeaveRequests
                    .CountAsync(x => x.Status == "Pending");
            ViewBag.RejectedLeaves =
                await _context.LeaveRequests
                    .CountAsync(x => x.Status == "Rejected");
            var recentRequests = await _context.LeaveRequests
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .OrderByDescending(x => x.AppliedDate)
                .Take(5)
                .ToListAsync();
            return View(recentRequests);
        }
    }
}