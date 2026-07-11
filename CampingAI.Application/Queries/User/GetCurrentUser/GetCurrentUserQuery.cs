namespace CampingAI.Application.Queries.User.GetCurrentUser;
public record GetCurrentUserQuery(Guid UserId) : Abstractions.Query.IQuery<Domain.Entities.User> {
}
