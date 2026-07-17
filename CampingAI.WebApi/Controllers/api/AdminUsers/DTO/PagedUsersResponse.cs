namespace CampingAI.WebApi.Controllers.api.AdminUsers.DTO;
public record PagedUsersResponse(IEnumerable<Controllers.api.Users.DTO.UserResponse> Items,
                                  int TotalCount,
                                  int Page,
                                  int PageSize);
