using MediatR;
using User.Application.Interfaces;

namespace User.Application.Commands;

public record DeleteUserCommand(Guid UserId) : IRequest;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _repository;

    public DeleteUserHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        await _repository.DeleteAsync(user);
        await _repository.SaveChangesAsync();
    }
}