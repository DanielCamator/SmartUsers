using MediatR;
using User.Application.Interfaces;

namespace User.Application.Commands;

public record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address) : IRequest;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IUserRepository _repository;

    public UpdateUserProfileHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber, request.Address);

        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();
    }
}