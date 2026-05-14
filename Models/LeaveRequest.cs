using System;
using System.Collections.Generic;

namespace EmployeeLeaveManagement.Models;

public partial class LeaveRequest
{
    public int LeaveRequestId { get; set; }
    public int? EmployeeId { get; set; }
    public int? LeaveTypeId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public string? Reason { get; set; }
    public string? Status { get; set; }
    public DateTime? AppliedDate { get; set; }
    public virtual Employee? Employee { get; set; }
    public virtual LeaveType? LeaveType { get; set; }
}
