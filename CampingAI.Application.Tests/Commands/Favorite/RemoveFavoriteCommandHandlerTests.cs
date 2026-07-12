using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Favorite.RemoveFavorite;
using CampingAI.Domain.Repositories;
using CampingAI.Infra.Abstractions;

namespace CampingAI.Application.Tests.Commands.Favorite;
public class RemoveFavoriteCommandHandlerTests {

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFavoritesWriteRepository> _favoritesWriteRepositoryMock;
    private readonly Mock<IValidator<RemoveFavoriteCommand>> _validatorMock;
    private readonly RemoveFavoriteCommandHandler _handler;

    public RemoveFavoriteCommandHandlerTests() {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _favoritesWriteRepositoryMock = new Mock<IFavoritesWriteRepository>();
        _validatorMock = new Mock<IValidator<RemoveFavoriteCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RemoveFavoriteCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new RemoveFavoriteCommandHandler(
            _unitOfWorkMock.Object,
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
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
