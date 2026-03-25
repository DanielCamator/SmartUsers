namespace Shared.Contracts.Events;

public record UserCreatedEvent
{
    public UserCreatedEvent() { }

    public UserCreatedEvent(Guid userId, string email, string passwordHash, Guid roleId)
    {
        UserId = userId;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public Guid RoleId { get; init; }
    public DateTime CreatedAt { get; init; }
}