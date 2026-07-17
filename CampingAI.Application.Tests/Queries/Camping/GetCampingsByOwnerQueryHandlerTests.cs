using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Camping.GetCampingsByOwner;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Queries.Camping;
public class GetCampingsByOwnerQueryHandlerTests {

    private readonly Mock<ICampingsReadRepository> _repositoryMock;
    private readonly GetCampingsByOwnerQueryHandler _handler;

    public GetCampingsByOwnerQueryHandlerTests() {
        _repositoryMock = new Mock<ICampingsReadRepository>();
        _handler = new GetCampingsByOwnerQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnCampings_WhenOwnerHasCampings() {
        // Arrange
        var ownerId = Guid.NewGuid();
        var campings = new List<Domain.Entities.Camping> {
            Domain.Entities.Camping.CreateNew("C1", "D1", 40m, -3m, 20m, ownerId, Guid.NewGuid()),
            Domain.Entities.Camping.CreateNew("C2", "D2", 41m, -2m, 30m, ownerId, Guid.NewGuid())
        };

        _repositoryMock
            .Setup(r => r.GetByOwnerAsync(ownerId))
            .ReturnsAsync(campings);

        var query = new GetCampingsByOwnerQuery(ownerId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(c => c.OwnerId.Should().Be(ownerId));
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmpty_WhenOwnerHasNoCampings() {
        // Arrange
        var ownerId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByOwnerAsync(ownerId))
            .ReturnsAsync([]);

        var query = new GetCampingsByOwnerQuery(ownerId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().BeEmpty();
    }
}
