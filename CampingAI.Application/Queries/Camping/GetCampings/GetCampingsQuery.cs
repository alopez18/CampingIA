namespace CampingAI.Application.Queries.Camping.GetCampings;
public record GetCampingsQuery(int Page, int PageSize, string? Search = null) : Abstractions.Query.IQuery<GetCampingsResult> {
}
