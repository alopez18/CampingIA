namespace CampingAI.Application.Queries.Camping.SearchCampings;
public record SearchCampingsResult(IEnumerable<Domain.Entities.Camping> Items, int TotalCount) {
}
