namespace CampingAI.Application.Queries.User.GetAllUsers;
public record GetAllUsersResult(IEnumerable<Domain.Entities.User> Items, int TotalCount);
