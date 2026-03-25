using Moq;
using Xunit;
using FluentAssertions;
using Auth.Application.Commands.Login;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Tests.Commands;

public class LoginHandlerTests
{
    private readonly Mock<IAuthUserRepository> _repositoryMock;
    private readonly Mock<IJwtGenerator> _jwtGeneratorMock;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _repositoryMock = new Mock<IAuthUserRepository>();
        _jwtGeneratorMock = new Mock<IJwtGenerator>();
        _handler = new LoginHandler(_repositoryMock.Object, _jwtGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var command = new LoginRequest("admin@empresa.com", "Admin123!");
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var user = new AuthUser(Guid.NewGuid(), command.Email, hash, Guid.NewGuid());

        _repositoryMock.Setup(repo => repo.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _jwtGeneratorMock.Setup(jwt => jwt.GenerateToken(user)).Returns("fake_jwt_token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be("fake_jwt_token");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenPasswordIsInvalid()
    {
        var command = new LoginRequest("admin@empresa.com", "WrongPassword!");
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var user = new AuthUser(Guid.NewGuid(), command.Email, hash, Guid.NewGuid());

        _repositoryMock.Setup(repo => repo.GetByEmailAsync(command.Email)).ReturnsAsync(user);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Credenciales inválidas.");
    }
}