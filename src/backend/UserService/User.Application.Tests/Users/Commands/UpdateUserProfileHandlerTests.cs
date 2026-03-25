using FluentAssertions;
using Moq;
using User.Application.Commands;
using User.Application.Interfaces;
using Xunit;

namespace User.Application.Tests.Users.Commands;

public class UpdateUserProfileHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UpdateUserProfileHandler _handler;

    public UpdateUserProfileHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateUserProfileHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserProfileAndSaveChanges_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User(
            "test@empresa.com",
            "hashedPassword",
            Guid.NewGuid(),
            "ViejoNombre",
            "ViejoApellido",
            "+521111111111",
            "Vieja Direccion");

        // Usamos reflection para setear el ID privado para la prueba
        var idProperty = typeof(Domain.Entities.User).GetProperty("Id");
        idProperty?.SetValue(existingUser, userId);

        var command = new UpdateUserProfileCommand(
            userId,
            "Daniel",
            "Desarrollador",
            "+523312345678",
            "Nueva Direccion 123");

        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        await _handler.Handle(command, CancellationToken.None);

        existingUser.FirstName.Should().Be("Daniel");
        existingUser.LastName.Should().Be("Desarrollador");
        existingUser.PhoneNumber.Should().Be("+523312345678");
        existingUser.Address.Should().Be("Nueva Direccion 123");

        _userRepositoryMock.Verify(repo => repo.UpdateAsync(existingUser), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        var command = new UpdateUserProfileCommand(
            Guid.NewGuid(),
            "Daniel",
            "Desarrollador",
            "+523312345678",
            "Nueva Direccion");

        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Usuario no encontrado.");

        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}