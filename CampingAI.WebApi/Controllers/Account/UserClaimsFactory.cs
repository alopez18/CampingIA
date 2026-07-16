using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.Account;

public static class UserClaimsFactory {
    public static ClaimsPrincipal Build(Domain.Entities.User user, string authenticationScheme) {
        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email.ToString()),
            new(ClaimTypes.Role, user.Role.Name)
        };

        if (!string.IsNullOrWhiteSpace(user.Name))
            claims.Add(new Claim(ClaimTypes.Name, user.Name));

        var identity = new ClaimsIdentity(claims, authenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
