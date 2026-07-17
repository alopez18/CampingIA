namespace CampingAI.Application.Commands.User.AdminUpdateUser;
public class AdminUpdateUserCommandHandler : Abstractions.Command.ICommandHandler<AdminUpdateUserCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    #endregion

    public AdminUpdateUserCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                         Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                         Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<Domain.Entities.User> HandleAsync(AdminUpdateUserCommand command) {
        var user = await _usersReadRepository.GetByIdAsync(command.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{command.UserId}'.");

        if (command.Name is not null)
            user.UpdateProfile(command.Name);

        if (!string.IsNullOrWhiteSpace(command.Email))
            user.UpdateEmail(command.Email);

        if (command.Role.HasValue)
            user.UpdateRole(command.Role.Value);

        if (!string.IsNullOrWhiteSpace(command.NewPassword)) {
            string hashed = _passwordHashingService.HashPassword(command.NewPassword)!;
            user.UpdatePassword(hashed);
        }

        user.Updated();

        await _usersWriteRepository.UpdateAsync(user);

        return user;
    }
}
