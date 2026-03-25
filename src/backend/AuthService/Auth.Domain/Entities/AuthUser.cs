namespace Auth.Domain.Entities;

public class AuthUser
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Guid RoleId { get; private set; }

    public AuthUser(Guid id, string email, string passwordHash, Guid roleId)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
    }

    private AuthUser() { }
}