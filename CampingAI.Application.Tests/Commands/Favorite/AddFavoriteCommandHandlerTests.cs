using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Favorite.AddFavorite;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Commands.Favorite;
public class AddFavoriteCommandHandlerTests {

    private readonly Mock<IFavoritesReadRepository> _favoritesReadRepositoryMock;
    private readonly Mock<IFavoritesWriteRepository> _favoritesWriteRepositoryMock;
    private readonly Mock<IValidator<AddFavoriteCommand>> _validatorMock;
    private readonly AddFavoriteCommandHandler _handler;

    public AddFavoriteCommandHandlerTests() {
        _favoritesReadRepositoryMock = new Mock<IFavoritesReadRepository>();
        _favoritesWriteRepositoryMock = new Mock<IFavoritesWriteRepository>();
        _validatorMock = new Mock<IValidator<AddFavoriteCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<AddFavoriteCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new AddFavoriteCommandHandler(
            _favoritesReadRepositoryMock.Object,
            _favoritesWriteRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_AddFavorite_WhenNotDuplicated() {
        // Arrange
        var userId = Guid.NewGuid();
        var campingId = Guid.NewGuid();
        var command = new AddFavoriteCommand(userId, campingId);

        _favoritesReadRepositoryMock
            .Setup(r => r.ExistsAsync(userId, campingId))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.CampingId.Should().Be(campingId);
        _favoritesWriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Favorite>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowDomainException_WhenAlreadyExists() {
        // Arrange
        var userId = Guid.NewGuid();
        var campingId = Guid.NewGuid();
        var command = new AddFavoriteCommand(userId, campingId);

        _favoritesReadRepositoryMock
            .Setup(r => r.ExistsAsync(userId, campingId))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<Domain.Exceptions.DomainException>();
        _favoritesWriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Favorite>()), Times.Never);
    }
}
