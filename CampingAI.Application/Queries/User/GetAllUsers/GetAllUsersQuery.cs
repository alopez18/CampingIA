namespace CampingAI.Application.Queries.User.GetAllUsers;
public record GetAllUsersQuery(int Page, int PageSize, string? Search = null) : Abstractions.Query.IQuery<GetAllUsersResult>;
