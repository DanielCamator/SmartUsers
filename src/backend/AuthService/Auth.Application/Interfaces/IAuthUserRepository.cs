using Auth.Domain.Entities;

namespace Auth.Application.Interfaces;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetByEmailAsync(string email);
    Task AddAsync(AuthUser user);
    Task<bool> SaveChangesAsync();
}