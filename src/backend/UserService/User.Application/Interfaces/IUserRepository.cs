namespace User.Application.Interfaces;

public interface IUserRepository
{
    Task<Domain.Entities.User?> GetByIdAsync(Guid id);
    Task<Domain.Entities.User?> GetByEmailAsync(string email);
    Task AddAsync(Domain.Entities.User user);
    Task UpdateAsync(Domain.Entities.User user);
    Task DeleteAsync(Domain.Entities.User user);
    Task<bool> SaveChangesAsync();
}