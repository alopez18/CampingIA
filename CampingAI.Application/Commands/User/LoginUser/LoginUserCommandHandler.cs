using System.Security.Claims;
using FluentValidation;

namespace CampingAI.Application.Commands.User.LoginUser;
public class LoginUserCommandHandler : Abstractions.Command.ICommandHandler<LoginUserCommand, string> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    readonly Services.JwtTokenService.Interfaces.IJwtTokenService _jwtTokenService;
    readonly IValidator<LoginUserCommand> _validator;
    #endregion

    public LoginUserCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                   Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService,
                                   Services.JwtTokenService.Interfaces.IJwtTokenService jwtTokenService,
                                   IValidator<LoginUserCommand> validator) {
        _usersReadRepository = usersReadRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<string> HandleAsync(LoginUserCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var user = await _usersReadRepository.GetByEmailAsync(command.Email)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el email '{command.Email}'.");

        bool valid = _passwordHashingService.VerifyPassword(command.Password, user.PasswordHashed.ToString());
        if (!valid)
            throw new Domain.Exceptions.DomainException("Las credenciales proporcionadas no son válidas.");

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
