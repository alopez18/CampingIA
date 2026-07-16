namespace CampingAI.Domain.Repositories;
public record CampingSearchFilters(
    string? Name,
    Guid? ProvinciaId,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    IEnumerable<Guid>? FacilityIds,
    decimal? MinLat,
    decimal? MaxLat,
    decimal? MinLng,
    decimal? MaxLng,
    int Page,
    int PageSize);
