using System.Security.Claims;

namespace CampingAI.Application.Commands.User.GoogleLoginUser;
public class GoogleLoginUserCommandHandler : Abstractions.Command.ICommandHandler<GoogleLoginUserCommand, string> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Services.JwtTokenService.Interfaces.IJwtTokenService _jwtTokenService;
    #endregion

    public GoogleLoginUserCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                         Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                         Infra.Abstractions.IUnitOfWork unitOfWork,
                                         Services.JwtTokenService.Interfaces.IJwtTokenService jwtTokenService) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<string> HandleAsync(GoogleLoginUserCommand command) {
        var user = await _usersReadRepository.GetByEmailAsync(command.Email);

        if (user is null) {
            user = Domain.Entities.User.CreateNew(command.Email,
                                                  "GOOGLE_OAUTH",
                                                  command.Name,
                                                  Domain.Enums.UserRole.Comun);
            await _usersWriteRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email.ToString()),
            new(ClaimTypes.Role, user.RoleId.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Name))
            claims.Add(new Claim(ClaimTypes.Name, user.Name));

        return _jwtTokenService.GenerateToken(claims);
    }
}
