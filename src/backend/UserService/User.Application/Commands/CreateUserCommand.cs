using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using User.Application.Interfaces;

namespace User.Application.Commands;

public record CreateUserCommand(
    string Email,
    string Password,
    Guid RoleId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address) : IRequest<Guid>;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(IUserRepository repository, IPublishEndpoint publishEndpoint, ILogger<CreateUserHandler> logger)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var existingUser = await _repository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("El correo electrónico ya está en uso.");
        }
        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new Domain.Entities.User(
            request.Email,
            hash,
            request.RoleId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Address);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("📤 [User] Publicando evento UserCreatedEvent para usuario: {Email}", user.Email);

        try
        {
            await _publishEndpoint.Publish(new UserCreatedEvent(
                user.Id, user.Email, user.PasswordHash, user.RoleId), ct);
            _logger.LogInformation("✅ [User] Evento UserCreatedEvent publicado exitosamente para: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [User] Error al publicar evento UserCreatedEvent para: {Email}", user.Email);
            throw;
        }

        return user.Id;
    }
}