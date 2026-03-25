using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Shared.Contracts.Events;

namespace Audit.Application.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IMongoCollection<AuditLog> _logsCollection;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(IMongoClient mongoClient, ILogger<UserCreatedConsumer> logger)
    {
        var database = mongoClient.GetDatabase("SmartUsersAuditDb");
        _logsCollection = database.GetCollection<AuditLog>("Logs");
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;

        var auditEntry = new AuditLog
        {
            UserId = @event.UserId,
            Email = @event.Email,
            Action = "USER_REGISTERED",
            CreatedAt = DateTime.UtcNow,
            Metadata = @event
        };

        try
        {
            await _logsCollection.InsertOneAsync(auditEntry);
            _logger.LogInformation("✅ [Audit] Evento persistido en MongoDB para: {Email}", @event.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Audit] Error al guardar en MongoDB");
            throw;
        }
    }
}