namespace CampingAI.WebApi.Controllers.api.Auth.Mappers;
public class AuthResponseMapper {
    public DTO.AuthResponse Map(Domain.Entities.User user, string token) =>
        new() {
            Token = token,
            Email = user.Email.ToString(),
            Name = user.Name
        };
}
