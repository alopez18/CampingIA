namespace CampingAI.Application.Queries.User.GetUserById;
public record GetUserByIdQuery(Guid UserId) : Abstractions.Query.IQuery<Domain.Entities.User> {
}
