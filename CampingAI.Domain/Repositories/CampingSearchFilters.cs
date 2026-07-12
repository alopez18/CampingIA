namespace CampingAI.Domain.Repositories;
public record CampingSearchFilters(
    string? Name,
    Guid? ProvinciaId,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    IEnumerable<Guid>? FacilityIds,
    int Page,
    int PageSize);
