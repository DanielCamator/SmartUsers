using Microsoft.EntityFrameworkCore;
using User.Application.Interfaces;

namespace User.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User.Domain.Entities.User?> GetByIdAsync(Guid id)
        => await _context.Users.FindAsync(id);

    public async Task<User.Domain.Entities.User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User.Domain.Entities.User user)
        => await _context.Users.AddAsync(user);

    public async Task UpdateAsync(User.Domain.Entities.User user)
        => Task.FromResult(_context.Users.Update(user));

    public async Task DeleteAsync(User.Domain.Entities.User user)
        => Task.FromResult(_context.Users.Remove(user));

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}