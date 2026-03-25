using Auth.Application.Interfaces;
using MassTransit.Mediator;
using MediatR;

namespace Auth.Application.Commands.Login;

public record LoginRequest(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(bool Success, string? Token = null, string? Error = null);

public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IAuthUserRepository _repository;
    private readonly IJwtGenerator _jwtGenerator;

    public LoginHandler(IAuthUserRepository repository, IJwtGenerator jwtGenerator)
    {
        _repository = repository;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken ct)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user == null)
            return new LoginResponse(Success: false, Error: "Credenciales inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return new LoginResponse(Success: false, Error: "Credenciales inválidas.");

        var token = _jwtGenerator.GenerateToken(user);
        return new LoginResponse(Success: true, Token: token);
    }
}
