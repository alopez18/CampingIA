namespace CampingAI.Application.Queries.Camping.SearchCampings;
public record SearchCampingsQuery(
    string? Name,
    Guid? ProvinciaId,
    string? ProvinciaCode,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    IEnumerable<Guid>? FacilityIds,
    int Page,
    int PageSize) : Abstractions.Query.IQuery<SearchCampingsResult> {
}
