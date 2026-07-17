namespace CampingAI.WebApi.Controllers.api.AdminUsers.DTO;
public record AdminUpdateUserRequest(string? Name,
                                     string? Email,
                                     int? RoleId);
