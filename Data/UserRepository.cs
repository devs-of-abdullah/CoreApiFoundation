using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class UserRepository : IUserRepository
{
    readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _context.users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await _context.users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.users
            .AnyAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<int> CreateAsync(UserEntity user)
    {
        await _context.users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task UpdateAsync(UserEntity user)
    {
        var existingUser = await _context.users
            .FirstOrDefaultAsync(u => u.Id == user.Id && !u.IsDeleted);

        if (existingUser is null)
            throw new KeyNotFoundException("User not found");

        existingUser.Email = user.Email;
        existingUser.Role = user.Role;
        existingUser.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var user = await _context.users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user is null)
            throw new KeyNotFoundException("User not found");

        user.IsDeleted = true;

        await _context.SaveChangesAsync();
    }

    
}
