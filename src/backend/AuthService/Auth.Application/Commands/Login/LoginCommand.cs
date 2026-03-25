using Auth.Application.Interfaces;
using MassTransit.Mediator;
using MediatR;

namespace Auth.Application.Commands.Login;

public record LoginRequest(string Email, string Password) : IRequest<string>;

public class LoginHandler : IRequestHandler<LoginRequest, string>
{
    private readonly IAuthUserRepository _repository;
    private readonly IJwtGenerator _jwtGenerator;

    public LoginHandler(IAuthUserRepository repository, IJwtGenerator jwtGenerator)
    {
        _repository = repository;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<string> Handle(LoginRequest request, CancellationToken ct)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        return _jwtGenerator.GenerateToken(user);
    }
}