using System;
using System.Collections.Generic;

namespace EmployeeLeaveManagement.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Department { get; set; }
    public DateTime? JoiningDate { get; set; }
    public bool? IsActive { get; set; }
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
