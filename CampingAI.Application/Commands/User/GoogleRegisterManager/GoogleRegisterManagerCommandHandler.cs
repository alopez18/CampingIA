namespace CampingAI.Application.Commands.User.GoogleRegisterManager;
public class GoogleRegisterManagerCommandHandler : Abstractions.Command.ICommandHandler<GoogleRegisterManagerCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    #endregion

    public GoogleRegisterManagerCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                               Domain.Repositories.IUsersWriteRepository usersWriteRepository) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
    }

    public async Task<Domain.Entities.User> HandleAsync(GoogleRegisterManagerCommand command) {
        var user = await _usersReadRepository.GetByEmailAsync(command.Email);

        if (user is null) {
            user = Domain.Entities.User.CreateNew(command.Email,
                                                  "GOOGLE_OAUTH",
                                                  command.Name,
                                                  Domain.Enums.UserRole.Comun);
            user.GrantManagerRoleInstantly();
            await _usersWriteRepository.AddAsync(user);
        }
        else {
            user.GrantManagerRoleInstantly();
            user.Updated();
            await _usersWriteRepository.UpdateAsync(user);
        }

        return user;
    }
}
