namespace CampingAI.Application.Queries.Camping.GetCampings;
public record GetCampingsQuery(int Page, int PageSize) : Abstractions.Query.IQuery<GetCampingsResult> {
}
