using EmployeeLeaveManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeLeaveManagement.Services;

public interface IUserService
{
    Task<UserListViewModel> GetUsersListAsync(string? searchTerm, string? username, string? email, string? phone, string? role, string? status, string? sortColumn, string? sortOrder, int pageNumber, int pageSize);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> AuthenticateAsync(string email, string password);
    Task<bool> RegisterUserAsync(UserViewModel model);
    Task<bool> UpdateUserAsync(UserViewModel model);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<Role>> GetRolesAsync();
    Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null);
    Task<bool> IsPhoneUniqueAsync(string phone, int? excludeUserId = null);
    Task<bool> ToggleUserStatusAsync(int id);
}
