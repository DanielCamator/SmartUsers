using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using Auth.Domain.Entities;
using Auth.Application.Interfaces;

namespace Auth.Application.Consumers;

public class SyncUserConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IAuthUserRepository _repository;
    private readonly ILogger<SyncUserConsumer> _logger;

    public SyncUserConsumer(IAuthUserRepository repository, ILogger<SyncUserConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("📥 [Auth] Recibiendo evento UserCreatedEvent para: {Email}", message.Email);

        var authUser = new AuthUser(
            message.UserId,
            message.Email,
            message.PasswordHash,
            message.RoleId
        );

        try
        {
            await _repository.AddAsync(authUser);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("✅ [Auth] Usuario sincronizado exitosamente en AuthDb: {Email}", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Auth] Error al sincronizar usuario: {Email}", message.Email);
            throw;
        }
    }
}