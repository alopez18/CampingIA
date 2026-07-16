namespace CampingAI.WebApi.Controllers.api.Users.DTO;
public class UserResponse {
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Name { get; init; }
    public int RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public DateTime CreatedOn { get; init; }
}
