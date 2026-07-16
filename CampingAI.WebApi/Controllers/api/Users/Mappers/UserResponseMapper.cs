namespace CampingAI.WebApi.Controllers.api.Users.Mappers;
public class UserResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.User, DTO.UserResponse> {
    public override DTO.UserResponse Map(Domain.Entities.User src) =>
        new() {
            Id = src.Id,
            Email = src.Email.ToString(),
            Name = src.Name,
            RoleId = src.RoleId,
            RoleName = src.Role.Name,
            CreatedOn = src.CreatedOn.Value
        };
}
