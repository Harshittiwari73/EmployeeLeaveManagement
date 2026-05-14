using EmployeeLeaveManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeLeaveManagement.Repositories;

public class UserRepository : IUserRepository
{
    private readonly EmployeeLeaveManagementContext _context;

    public UserRepository(EmployeeLeaveManagementContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.Include(u => u.Role).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByPhoneAsync(string phone)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.PhoneNumber == phone);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(e => e.UserId == id);
    }
}
