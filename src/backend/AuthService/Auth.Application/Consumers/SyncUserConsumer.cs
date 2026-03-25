using MassTransit;
using Shared.Contracts.Events;
using Auth.Domain.Entities;
using Auth.Application.Interfaces;

namespace Auth.Application.Consumers;

public class SyncUserConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IAuthUserRepository _repository;

    public SyncUserConsumer(IAuthUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var message = context.Message;

        var authUser = new AuthUser(
            message.UserId,
            message.Email,
            message.PasswordHash,
            message.RoleId
        );

        await _repository.AddAsync(authUser);
        await _repository.SaveChangesAsync();
    }
}