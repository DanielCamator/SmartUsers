using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Contracts.Events;
using User.Application.Commands;
using User.Application.Interfaces;
using Xunit;

namespace User.Application.Tests.Users.Commands;

public class CreateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<CreateUserHandler>> _loggerMock;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _loggerMock = new Mock<ILogger<CreateUserHandler>>();
        _handler = new CreateUserHandler(_userRepositoryMock.Object, _publishEndpointMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_SaveToDb_AndPublishEvent_WhenCommandIsValid()
    {
        var command = new CreateUserCommand(
            "test@cyber.com",
            "Password123!",
            Guid.NewGuid(),
            "Daniel",
            "Cybertitan",
            "+523312345678",
            "Calle Tech 101"
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();

        // Verificar que se guardó en el repositorio
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Domain.Entities.User>(u =>
            u.Email == command.Email &&
            u.FirstName == command.FirstName &&
            u.PhoneNumber == command.PhoneNumber
        )), Times.Once);

        // Verificar que se confirmaron los cambios en la DB
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);

        // Verificar que se publicó el evento en RabbitMQ
        _publishEndpointMock.Verify(bus => bus.Publish(It.Is<UserCreatedEvent>(e =>
            e.UserId == result &&
            e.Email == command.Email
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}