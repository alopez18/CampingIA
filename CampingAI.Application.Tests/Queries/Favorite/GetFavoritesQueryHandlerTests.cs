using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Favorite.GetFavorites;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Queries.Favorite;
public class GetFavoritesQueryHandlerTests {

    private readonly Mock<IFavoritesReadRepository> _favoritesReadRepositoryMock;
    private readonly GetFavoritesQueryHandler _handler;

    public GetFavoritesQueryHandlerTests() {
        _favoritesReadRepositoryMock = new Mock<IFavoritesReadRepository>();
        _handler = new GetFavoritesQueryHandler(_favoritesReadRepositoryMock.Object);
    }

    private static Domain.Entities.Favorite BuildFavorite(Guid userId) =>
        Domain.Entities.Favorite.CreateNew(userId, Guid.NewGuid());

    [Fact]
    public async Task HandleAsync_Should_ReturnFavorites_WhenUserHasFavorites() {
        // Arrange
        var userId = Guid.NewGuid();
        var favorites = new[] { BuildFavorite(userId), BuildFavorite(userId) };
        _favoritesReadRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(favorites);

        var query = new GetFavoritesQuery(userId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(f => f.UserId.Should().Be(userId));
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmpty_WhenUserHasNoFavorites() {
        // Arrange
        var userId = Guid.NewGuid();
        _favoritesReadRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync([]);

        var query = new GetFavoritesQuery(userId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().BeEmpty();
    }
}
