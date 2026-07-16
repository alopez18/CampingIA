namespace CampingAI.Domain.Repositories;
public record CampingSearchFilters(
    string? Name,
    Guid? ProvinciaId,
    IEnumerable<Guid>? CategoryIds,
    decimal? MinPrice,
    decimal? MaxPrice,
    IEnumerable<Guid>? FacilityIds,
    decimal? MinLat,
    decimal? MaxLat,
    decimal? MinLng,
    decimal? MaxLng,
    int Page,
    int PageSize);
