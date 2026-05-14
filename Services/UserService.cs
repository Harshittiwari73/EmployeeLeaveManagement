using EmployeeLeaveManagement.Models;
using EmployeeLeaveManagement.Repositories;
using BCrypt.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeLeaveManagement.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserListViewModel> GetUsersListAsync(string? searchTerm, string? username, string? email, string? phone, string? role, string? status, string? sortColumn, string? sortOrder, int pageNumber, int pageSize)
    {
        var users = await _userRepository.GetAllUsersAsync();
        var query = users.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                                     u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(username))
        {
            query = query.Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => u.Email.Contains(email, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(phone))
        {
            query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phone));
        }

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u => u.Role.RoleName.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(status))
        {
            bool isActive = status.Equals("active", StringComparison.OrdinalIgnoreCase);
            query = query.Where(u => u.IsActive == isActive);
        }

        if (!string.IsNullOrEmpty(sortColumn))
        {
            bool isDesc = sortOrder == "desc";
            query = sortColumn.ToLower() switch
            {
                "username" => isDesc ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
                "email" => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "role" => isDesc ? query.OrderByDescending(u => u.Role.RoleName) : query.OrderBy(u => u.Role.RoleName),
                "status" => isDesc ? query.OrderByDescending(u => u.IsActive) : query.OrderBy(u => u.IsActive),
                _ => query.OrderByDescending(u => u.CreatedDate)
            };
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedDate);
        }

        int totalItems = query.Count();
        var pagedUsers = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(u => new UserViewModel
        {
            UserId = u.UserId,
            Username = u.Username,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            RoleName = u.Role.RoleName,
            IsActive = u.IsActive,
            CreatedDate = u.CreatedDate
        }).ToList();

        return new UserListViewModel
        {
            Users = pagedUsers,
            SearchTerm = searchTerm,
            Username = username,
            Email = email,
            Phone = phone,
            Role = role,
            Status = status,
            SortColumn = sortColumn,
            SortOrder = sortOrder,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }
    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null;
        }
        return user;
    }
    public async Task<bool> RegisterUserAsync(UserViewModel model)
    {
        if (await IsEmailUniqueAsync(model.Email))
        {
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = model.RoleId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            await _userRepository.AddUserAsync(user);
            return true;
        }
        return false;
    }
    public async Task<bool> UpdateUserAsync(UserViewModel model)
    {
        var user = await _userRepository.GetUserByIdAsync(model.UserId);
        if (user == null) return false;
        user.Username = model.Username;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.RoleId = model.RoleId;
        user.IsActive = model.IsActive;
        if (!string.IsNullOrEmpty(model.Password) && model.Password != "********")
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        }
        await _userRepository.UpdateUserAsync(user);
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        await _userRepository.DeleteUserAsync(id);
        return true;
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        return await _userRepository.GetAllRolesAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null) return true;
        return excludeUserId.HasValue && user.UserId == excludeUserId.Value;
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, int? excludeUserId = null)
    {
        if (string.IsNullOrEmpty(phone)) return true;
        var user = await _userRepository.GetUserByPhoneAsync(phone);
        if (user == null) return true;
        return excludeUserId.HasValue && user.UserId == excludeUserId.Value;
    }

    public async Task<bool> ToggleUserStatusAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }
        return false;
    }
}
