namespace CampingAI.WebApi.Controllers.api.Auth.DTO;
public record RegisterRequest(string Email,
                              string Password,
                              string? Name);
