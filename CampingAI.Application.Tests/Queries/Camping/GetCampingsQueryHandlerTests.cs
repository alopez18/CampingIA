using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Camping.GetCampings;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Queries.Camping;
public class GetCampingsQueryHandlerTests {

    private readonly Mock<ICampingsReadRepository> _repositoryMock;
    private readonly GetCampingsQueryHandler _handler;

    public GetCampingsQueryHandlerTests() {
        _repositoryMock = new Mock<ICampingsReadRepository>();
        _handler = new GetCampingsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnPagedResult_WhenCampingsExist() {
        // Arrange
        var campings = new List<Domain.Entities.Camping> {
            Domain.Entities.Camping.CreateNew("C1", "D1", 40m, -3m, 20m, Guid.NewGuid(), Guid.NewGuid()),
            Domain.Entities.Camping.CreateNew("C2", "D2", 41m, -2m, 30m, Guid.NewGuid(), Guid.NewGuid())
        };

        _repositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, null))
            .ReturnsAsync((campings, 2));

        var query = new GetCampingsQuery(1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmptyResult_WhenNoCampingsExist() {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, null))
            .ReturnsAsync((Enumerable.Empty<Domain.Entities.Camping>(), 0));

        var query = new GetCampingsQuery(1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
