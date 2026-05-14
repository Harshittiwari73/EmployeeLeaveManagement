using EmployeeLeaveManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeLeaveManagement.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByPhoneAsync(string phone);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<bool> UserExistsAsync(int id);
}
