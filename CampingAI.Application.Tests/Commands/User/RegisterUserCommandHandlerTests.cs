using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.User.RegisterUser;
using CampingAI.Application.Services.PasswordHashingService.Interfaces;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Commands.User;
public class RegisterUserCommandHandlerTests {

    private readonly Mock<IUsersWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUsersReadRepository> _readRepositoryMock;
    private readonly Mock<IPasswordHashingService> _passwordHashingServiceMock;
    private readonly Mock<IValidator<RegisterUserCommand>> _validatorMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests() {
        _writeRepositoryMock = new Mock<IUsersWriteRepository>();
        _readRepositoryMock = new Mock<IUsersReadRepository>();
        _passwordHashingServiceMock = new Mock<IPasswordHashingService>();
        _validatorMock = new Mock<IValidator<RegisterUserCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RegisterUserCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _passwordHashingServiceMock
            .Setup(s => s.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");

        _handler = new RegisterUserCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _passwordHashingServiceMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_RegisterUser_WhenEmailIsNotDuplicated() {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.ExistsAsync("new@example.com"))
            .ReturnsAsync(false);

        var command = new RegisterUserCommand("new@example.com", "password123", "John Doe");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Email.ToString().Should().Be(command.Email);
        result.Name.Should().Be(command.Name);
        result.RoleId.Should().Be((int)Domain.Enums.UserRole.Comun);
        _writeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowDomainException_WhenEmailAlreadyExists() {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.ExistsAsync("existing@example.com"))
            .ReturnsAsync(true);

        var command = new RegisterUserCommand("existing@example.com", "password123", null);

        // Act
        Func<Task> act = () => _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*existing@example.com*");
    }

    [Fact]
    public async Task HandleAsync_Should_HashPassword_BeforeCreatingUser() {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var command = new RegisterUserCommand("user@example.com", "plaintext", "Test");

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _passwordHashingServiceMock.Verify(s => s.HashPassword("plaintext"), Times.Once);
    }
}
