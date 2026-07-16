using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using CampingAI.AI.Providers;
using CampingAI.AI.Search;
using CampingAI.Application.Abstractions;
using CampingAI.Application.Queries.Camping.SearchCampings;
using CampingAI.Application.Queries.Category.GetCategories;

namespace CampingAI.AI.Tests.Search;
public class CampingSearchAssistantTests {

    static readonly Guid CategoryFamiliar = new("B1000001-0000-0000-0000-000000000001");
    static readonly Guid CategoryPlaya = new("B1000002-0000-0000-0000-000000000002");
    static readonly Guid FacilityWifi = new("C1000001-0000-0000-0000-000000000001");

    readonly Mock<IAIProvider> _aiProvider = new();
    readonly Mock<IMediator> _mediator = new();
    readonly Mock<Domain.Repositories.IFacilitiesReadRepository> _facilities = new();

    CampingSearchAssistant CreateSut() {
        _mediator.Setup(m => m.SendQueryAsync<GetCategoriesQuery, GetCategoriesResult>(It.IsAny<GetCategoriesQuery>()))
            .ReturnsAsync(new GetCategoriesResult([
                new Domain.Entities.Category(CategoryFamiliar, "Familiar"),
                new Domain.Entities.Category(CategoryPlaya, "Playa")
            ]));

        _facilities.Setup(f => f.GetAllAsync())
            .ReturnsAsync([new Domain.Entities.Facility(FacilityWifi, "WiFi")]);

        return new CampingSearchAssistant(_aiProvider.Object, _mediator.Object, _facilities.Object, NullLogger<CampingSearchAssistant>.Instance);
    }

    [Fact]
    public async Task ExtractFiltersAsync_Should_ParseValidGuids() {
        // Arrange
        var json = $$"""
        { "name": "playa", "categoryIds": ["{{CategoryPlaya}}"], "facilityIds": ["{{FacilityWifi}}"], "minPrice": null, "maxPrice": 40 }
        """;
        _aiProvider.Setup(p => p.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);
        var sut = CreateSut();

        // Act
        var filters = await sut.ExtractFiltersAsync("campings de playa con wifi baratos");

        // Assert
        filters.Name.Should().Be("playa");
        filters.CategoryIds.Should().ContainSingle().Which.Should().Be(CategoryPlaya);
        filters.FacilityIds.Should().ContainSingle().Which.Should().Be(FacilityWifi);
        filters.MaxPrice.Should().Be(40);
    }

    [Fact]
    public async Task ExtractFiltersAsync_Should_DiscardGuidsNotInCatalog() {
        // Arrange
        var unknown = Guid.NewGuid();
        var json = $$"""
        { "name": null, "categoryIds": ["{{unknown}}"], "facilityIds": [], "minPrice": null, "maxPrice": null }
        """;
        _aiProvider.Setup(p => p.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);
        var sut = CreateSut();

        // Act
        var filters = await sut.ExtractFiltersAsync("algo");

        // Assert
        filters.CategoryIds.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractFiltersAsync_Should_Throw_WhenInvalidJson() {
        // Arrange
        _aiProvider.Setup(p => p.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("no soy json");
        var sut = CreateSut();

        // Act
        var act = async () => await sut.ExtractFiltersAsync("algo");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task SearchAsync_Should_ForwardFiltersToMediator() {
        // Arrange
        var json = $$"""
        { "name": "familiar", "categoryIds": ["{{CategoryFamiliar}}"], "facilityIds": [], "minPrice": 10, "maxPrice": null }
        """;
        _aiProvider.Setup(p => p.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        SearchCampingsQuery? captured = null;
        _mediator.Setup(m => m.SendQueryAsync<SearchCampingsQuery, SearchCampingsResult>(It.IsAny<SearchCampingsQuery>()))
            .Callback<SearchCampingsQuery>(q => captured = q)
            .ReturnsAsync(new SearchCampingsResult([], 0));
        var sut = CreateSut();

        // Act
        await sut.SearchAsync("campings familiares", 2, 15);

        // Assert
        captured.Should().NotBeNull();
        captured!.Name.Should().Be("familiar");
        captured.CategoryIds.Should().ContainSingle().Which.Should().Be(CategoryFamiliar);
        captured.MinPrice.Should().Be(10);
        captured.Page.Should().Be(2);
        captured.PageSize.Should().Be(15);
    }
}
