namespace CampingAI.WebApi.Controllers.api.Auth.DTO;
public class AuthResponse {
    public string Token { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Name { get; init; }
}
