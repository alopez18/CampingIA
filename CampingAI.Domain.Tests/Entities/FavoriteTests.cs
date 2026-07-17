using FluentAssertions;
using CampingAI.Domain.Entities;

namespace CampingAI.Domain.Tests.Entities;
public class FavoriteTests {

    [Fact]
    public void CreateNew_Should_InitializeFavoriteWithCorrectValues() {
        // Arrange
        var userId = Guid.NewGuid();
        var campingId = Guid.NewGuid();

        // Act
        var favorite = Favorite.CreateNew(userId, campingId);

        // Assert
        favorite.Should().NotBeNull();
        favorite.Id.Should().NotBe(Guid.Empty);
        favorite.UserId.Should().Be(userId);
        favorite.CampingId.Should().Be(campingId);
        favorite.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentException_WhenIdIsEmpty() {
        // Act
        Action act = () => new Favorite(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("idFavorite");
    }

    [Fact]
    public void CreateNew_Should_GenerateUniqueIds_ForDifferentFavorites() {
        // Arrange
        var userId = Guid.NewGuid();
        var campingId = Guid.NewGuid();

        // Act
        var favorite1 = Favorite.CreateNew(userId, campingId);
        var favorite2 = Favorite.CreateNew(userId, campingId);

        // Assert
        favorite1.Id.Should().NotBe(favorite2.Id);
    }
}
