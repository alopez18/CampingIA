namespace CampingAI.Application.Queries.Camping.SearchCampings;
public record SearchCampingsQuery(
    string? Name,
    Guid? ProvinciaId,
    string? ProvinciaCode,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    IEnumerable<Guid>? FacilityIds,
    decimal? MinLat,
    decimal? MaxLat,
    decimal? MinLng,
    decimal? MaxLng,
    int Page,
    int PageSize) : Abstractions.Query.IQuery<SearchCampingsResult> {
}
