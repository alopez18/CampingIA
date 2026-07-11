using System.Security.Claims;

namespace CampingAI.Application.Services.JwtTokenService.Interfaces;
public interface IJwtTokenService {
    string GenerateToken(IEnumerable<Claim> claims);
}
