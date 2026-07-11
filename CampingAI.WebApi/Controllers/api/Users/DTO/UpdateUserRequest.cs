namespace CampingAI.WebApi.Controllers.api.Users.DTO;
public record UpdateUserRequest(string? Name,
                                string? Email,
                                string? Password);
