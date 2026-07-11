namespace CampingAI.Application.Queries.Camping.GetCampings;
public record GetCampingsResult(IEnumerable<Domain.Entities.Camping> Items, int TotalCount) {
}
