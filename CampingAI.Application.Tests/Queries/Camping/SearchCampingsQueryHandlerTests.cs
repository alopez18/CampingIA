using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Camping.SearchCampings;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Queries.Camping;
public class SearchCampingsQueryHandlerTests {

    private readonly Mock<ICampingsReadRepository> _campingsRepositoryMock;
    private readonly Mock<IProvincesReadRepository> _provincesRepositoryMock;
    private readonly SearchCampingsQueryHandler _handler;

    public SearchCampingsQueryHandlerTests() {
        _campingsRepositoryMock = new Mock<ICampingsReadRepository>();
        _provincesRepositoryMock = new Mock<IProvincesReadRepository>();
        _handler = new SearchCampingsQueryHandler(_campingsRepositoryMock.Object, _provincesRepositoryMock.Object);
    }

    private static Domain.Entities.Camping BuildCamping(Guid? provinciaId = null) =>
        Domain.Entities.Camping.CreateNew("Camping Test", "Descripcion", 40m, -3m, 25m, Guid.NewGuid(), 1, provinciaId);

    [Fact]
    public async Task HandleAsync_Should_ReturnItems_WhenNoFiltersApplied() {
        // Arrange
        var campings = new[] { BuildCamping(), BuildCamping() };
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<CampingSearchFilters>()))
            .ReturnsAsync((campings, 2));

        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task HandleAsync_Should_PassProvinciaId_ToRepository() {
        // Arrange
        var provinciaId = Guid.NewGuid();
        var camping = BuildCamping(provinciaId: provinciaId);
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.ProvinciaId == provinciaId)))
            .ReturnsAsync((new[] { camping }, 1));

        var query = new SearchCampingsQuery(null, provinciaId, null, null, null, null, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        _campingsRepositoryMock.Verify(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.ProvinciaId == provinciaId)), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ResolveProvinciaCode_ToId_AndPassToRepository() {
        // Arrange
        var provinciaId = Guid.NewGuid();
        var provinceWithId = new Domain.Entities.Province(provinciaId, "MAD", "Madrid", Guid.NewGuid());

        _provincesRepositoryMock
            .Setup(r => r.GetByCodeAsync("MAD"))
            .ReturnsAsync(provinceWithId);

        var camping = BuildCamping(provinciaId: provinciaId);
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.ProvinciaId == provinciaId)))
            .ReturnsAsync((new[] { camping }, 1));

        var query = new SearchCampingsQuery(null, null, "MAD", null, null, null, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.TotalCount.Should().Be(1);
        _provincesRepositoryMock.Verify(r => r.GetByCodeAsync("MAD"), Times.Once);
        _campingsRepositoryMock.Verify(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.ProvinciaId == provinciaId)), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_PreferProvinciaId_OverCode_WhenBothProvided() {
        // Arrange
        var provinciaId = Guid.NewGuid();
        var camping = BuildCamping(provinciaId: provinciaId);
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.ProvinciaId == provinciaId)))
            .ReturnsAsync((new[] { camping }, 1));

        var query = new SearchCampingsQuery(null, provinciaId, "MAD", null, null, null, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        _provincesRepositoryMock.Verify(r => r.GetByCodeAsync(It.IsAny<string>()), Times.Never);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_Should_PassPriceRangeFilter_ToRepository() {
        // Arrange
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.MinPrice == 10m && f.MaxPrice == 50m)))
            .ReturnsAsync((new[] { BuildCamping() }, 1));

        var query = new SearchCampingsQuery(null, null, null, null, 10m, 50m, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_Should_PassCombinedFilters_ToRepository() {
        // Arrange
        var provinciaId = Guid.NewGuid();
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.Is<CampingSearchFilters>(f =>
                f.ProvinciaId == provinciaId && f.MaxPrice == 80m)))
            .ReturnsAsync((new[] { BuildCamping(provinciaId) }, 1));

        var query = new SearchCampingsQuery(null, provinciaId, null, null, null, 80m, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmpty_WhenNoResults() {
        // Arrange
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<CampingSearchFilters>()))
            .ReturnsAsync((Enumerable.Empty<Domain.Entities.Camping>(), 0));

        var query = new SearchCampingsQuery("NoExiste", null, null, null, null, null, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_Should_PassPagination_ToRepository() {
        // Arrange
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.Page == 2 && f.PageSize == 5)))
            .ReturnsAsync((new[] { BuildCamping() }, 6));

        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, 2, 5);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.TotalCount.Should().Be(6);
        _campingsRepositoryMock.Verify(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.Page == 2 && f.PageSize == 5)), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_PassFacilityIds_ToRepository() {
        // Arrange
        var facilityId = Guid.NewGuid();
        _campingsRepositoryMock
            .Setup(r => r.SearchAsync(It.Is<CampingSearchFilters>(f => f.FacilityIds != null && f.FacilityIds.Contains(facilityId))))
            .ReturnsAsync((new[] { BuildCamping() }, 1));

        var query = new SearchCampingsQuery(null, null, null, null, null, null, new[] { facilityId }, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.TotalCount.Should().Be(1);
    }
}