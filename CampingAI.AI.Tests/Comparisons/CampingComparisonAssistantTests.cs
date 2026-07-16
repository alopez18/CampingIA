using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using CampingAI.AI.Comparisons;
using CampingAI.AI.Providers;
using CampingAI.Application.Abstractions;
using CampingAI.Application.Queries.Camping.GetCampingById;

namespace CampingAI.AI.Tests.Comparisons;
public class CampingComparisonAssistantTests {

    readonly Mock<IAIProvider> _aiProvider = new();
    readonly Mock<IMediator> _mediator = new();

    static Domain.Entities.Camping CreateCamping(string name, decimal price) =>
        Domain.Entities.Camping.CreateNew(name, "Descripción de prueba", 40m, -3m, price, Guid.NewGuid(), Guid.NewGuid());

    CampingComparisonAssistant CreateSut() =>
        new(_aiProvider.Object, _mediator.Object, NullLogger<CampingComparisonAssistant>.Instance);

    [Fact]
    public async Task CompareAsync_Should_Throw_WhenLessThanTwoIds() {
        // Arrange
        var sut = CreateSut();

        // Act
        var act = async () => await sut.CompareAsync([Guid.NewGuid()]);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CompareAsync_Should_ReturnParsedResponse() {
        // Arrange
        var c1 = CreateCamping("A", 20m);
        var c2 = CreateCamping("B", 30m);

        _mediator.SetupSequence(m => m.SendQueryAsync<GetCampingByIdQuery, Domain.Entities.Camping>(It.IsAny<GetCampingByIdQuery>()))
            .ReturnsAsync(c1)
            .ReturnsAsync(c2);

        var json = $$"""
        { "summary": "A es más barato", "bestForBudget": "{{c1.Id}}", "bestOverall": "{{c2.Id}}" }
        """;
        _aiProvider.Setup(p => p.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);
        var sut = CreateSut();

        // Act
        var result = await sut.CompareAsync([c1.Id, c2.Id]);

        // Assert
        result.Summary.Should().Be("A es más barato");
        result.BestForBudget.Should().Be(c1.Id);
        result.BestOverall.Should().Be(c2.Id);
    }
}
