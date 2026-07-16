namespace CampingAI.Application.Queries.User.GetPendingManagers;
public record GetPendingManagersQuery() : Abstractions.Query.IQuery<IEnumerable<Domain.Entities.User>> {
}
