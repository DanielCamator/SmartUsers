namespace Audit.Application.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(AuditLog log);
}