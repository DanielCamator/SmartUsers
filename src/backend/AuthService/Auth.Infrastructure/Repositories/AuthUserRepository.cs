using Microsoft.EntityFrameworkCore;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence;

namespace Auth.Infrastructure.Repositories;

public class AuthUserRepository : IAuthUserRepository
{
    private readonly AuthDbContext _context;

    public AuthUserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<AuthUser?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(AuthUser user)
        => await _context.Users.AddAsync(user);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}