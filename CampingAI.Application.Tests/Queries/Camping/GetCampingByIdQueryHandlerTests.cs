using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Camping.GetCampingById;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Queries.Camping;
public class GetCampingByIdQueryHandlerTests {

    private readonly Mock<ICampingsReadRepository> _repositoryMock;
    private readonly GetCampingByIdQueryHandler _handler;

    private static Domain.Entities.Camping CreateSampleCamping() =>
        Domain.Entities.Camping.CreateNew("Camping", "Desc", 40m, -3m, 25m, Guid.NewGuid(), Guid.NewGuid());

    public GetCampingByIdQueryHandlerTests() {
        _repositoryMock = new Mock<ICampingsReadRepository>();
        _handler = new GetCampingByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnCamping_WhenExists() {
        // Arrange
        var camping = CreateSampleCamping();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(camping.Id))
            .ReturnsAsync(camping);

        var query = new GetCampingByIdQuery(camping.Id);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(camping.Id);
        result.Name.ToString().Should().Be(camping.Name.ToString());
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenCampingDoesNotExist() {
        // Arrange
        var campingId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(campingId))
            .ReturnsAsync((Domain.Entities.Camping?)null);

        var query = new GetCampingByIdQuery(campingId);

        // Act
        Func<Task> act = () => _handler.HandleAsync(query);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{campingId}*");
    }
}
