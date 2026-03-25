using Auth.Domain.Entities;

namespace Auth.Application.Interfaces;

public interface IJwtGenerator
{
    string GenerateToken(AuthUser user);
}