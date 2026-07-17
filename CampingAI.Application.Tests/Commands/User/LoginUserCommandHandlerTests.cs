using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.User.LoginUser;
using CampingAI.Application.Services.PasswordHashingService.Interfaces;
using CampingAI.Application.Services.JwtTokenService.Interfaces;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.Repositories;
using System.Security.Claims;

namespace CampingAI.Application.Tests.Commands.User;
public class LoginUserCommandHandlerTests {

    private readonly Mock<IUsersReadRepository> _readRepositoryMock;
    private readonly Mock<IPasswordHashingService> _passwordHashingServiceMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IValidator<LoginUserCommand>> _validatorMock;
    private readonly LoginUserCommandHandler _handler;

    private static Domain.Entities.User CreateSampleUser(string email = "user@example.com") =>
        Domain.Entities.User.CreateNew(email, "hashed_pwd", "Test User", Domain.Enums.UserRole.Comun);

    public LoginUserCommandHandlerTests() {
        _readRepositoryMock = new Mock<IUsersReadRepository>();
        _passwordHashingServiceMock = new Mock<IPasswordHashingService>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _validatorMock = new Mock<IValidator<LoginUserCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginUserCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new LoginUserCommandHandler(
            _readRepositoryMock.Object,
            _passwordHashingServiceMock.Object,
            _jwtTokenServiceMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnToken_WhenCredentialsAreValid() {
        // Arrange
        var user = CreateSampleUser();
        _readRepositoryMock
            .Setup(r => r.GetByEmailAsync("user@example.com"))
            .ReturnsAsync(user);
        _passwordHashingServiceMock
            .Setup(s => s.VerifyPassword("password123", It.IsAny<string>()))
            .Returns(true);
        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns("jwt_token");

        var command = new LoginUserCommand("user@example.com", "password123");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().Be("jwt_token");
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenUserDoesNotExist() {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var command = new LoginUserCommand("unknown@example.com", "password");

        // Act
        Func<Task> act = () => _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*unknown@example.com*");
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowDomainException_WhenPasswordIsInvalid() {
        // Arrange
        var user = CreateSampleUser();
        _readRepositoryMock
            .Setup(r => r.GetByEmailAsync("user@example.com"))
            .ReturnsAsync(user);
        _passwordHashingServiceMock
            .Setup(s => s.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var command = new LoginUserCommand("user@example.com", "wrong_password");

        // Act
        Func<Task> act = () => _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Las credenciales proporcionadas no son válidas.");
    }
}
