using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Favorite.RemoveFavorite;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Commands.Favorite;
public class RemoveFavoriteCommandHandlerTests {

    private readonly Mock<IFavoritesWriteRepository> _favoritesWriteRepositoryMock;
    private readonly Mock<IValidator<RemoveFavoriteCommand>> _validatorMock;
    private readonly RemoveFavoriteCommandHandler _handler;

    public RemoveFavoriteCommandHandlerTests() {
        _favoritesWriteRepositoryMock = new Mock<IFavoritesWriteRepository>();
        _validatorMock = new Mock<IValidator<RemoveFavoriteCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RemoveFavoriteCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new RemoveFavoriteCommandHandler(
            _favoritesWriteRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_CallDelete_WhenCalled() {
        // Arrange
        var userId = Guid.NewGuid();
        var campingId = Guid.NewGuid();
        var command = new RemoveFavoriteCommand(userId, campingId);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _favoritesWriteRepositoryMock.Verify(r => r.DeleteAsync(userId, campingId), Times.Once);
    }
}
