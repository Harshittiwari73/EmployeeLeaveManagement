using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagement.Models;

public class UserViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    [Display(Name = "Phone Number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "Role is required")]
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
}
