using MediatR;
using User.Application.Interfaces;

namespace User.Application.Queries;

public record UserDto(Guid Id, string Email, string FirstName, string LastName, string PhoneNumber, string Address);

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repository;

    public GetUserByIdHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId);

        if (user == null) return null;

        return new UserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Address);
    }
}