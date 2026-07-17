namespace CampingAI.Application.Commands.User.AdminCreateUser;
public class AdminCreateUserCommandHandler : Abstractions.Command.ICommandHandler<AdminCreateUserCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    #endregion

    public AdminCreateUserCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                         Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                         Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<Domain.Entities.User> HandleAsync(AdminCreateUserCommand command) {
        bool exists = await _usersReadRepository.ExistsAsync(command.Email);
        if (exists)
            throw new Domain.Exceptions.DomainException($"Ya existe un usuario con el email '{command.Email}'.");

        string hashed = _passwordHashingService.HashPassword(command.Password)!;

        var user = Domain.Entities.User.CreateNew(command.Email, hashed, command.Name, command.Role);

        await _usersWriteRepository.AddAsync(user);

        return user;
    }
}
