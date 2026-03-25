using FluentAssertions;
using Moq;
using User.Application.Commands;
using User.Application.Interfaces;
using Xunit;

namespace User.Application.Tests.Users.Commands;

public class DeleteUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new DeleteUserHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUserAndSaveChanges_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User("test@test.com", "hash", Guid.NewGuid(), "A", "B", "123", "C");

        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var command = new DeleteUserCommand(userId);

        await _handler.Handle(command, CancellationToken.None);

        _userRepositoryMock.Verify(repo => repo.DeleteAsync(existingUser), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var command = new DeleteUserCommand(Guid.NewGuid());

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}