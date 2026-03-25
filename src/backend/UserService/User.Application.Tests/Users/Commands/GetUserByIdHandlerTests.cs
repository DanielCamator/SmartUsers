using FluentAssertions;
using Moq;
using User.Application.Interfaces;
using User.Application.Queries;
using Xunit;

namespace User.Application.Tests.Users.Queries;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserByIdHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User("correo@test.com", "hash", Guid.NewGuid(), "Daniel", "Dev", "123456", "Calle 1");

        // Forzamos el ID para la prueba
        var idProperty = typeof(Domain.Entities.User).GetProperty("Id");
        idProperty?.SetValue(existingUser, userId);

        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var query = new GetUserByIdQuery(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Email.Should().Be("correo@test.com");
        result.FirstName.Should().Be("Daniel");
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var query = new GetUserByIdQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}