namespace CampingAI.Application.Commands.User.RequestManagerRole;
public class RequestManagerRoleCommandHandler : Abstractions.Command.ICommandHandler<RequestManagerRoleCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    #endregion

    public RequestManagerRoleCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                            Domain.Repositories.IUsersWriteRepository usersWriteRepository) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
    }

    public async Task<Domain.Entities.User> HandleAsync(RequestManagerRoleCommand command) {
        var user = await _usersReadRepository.GetByIdAsync(command.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{command.UserId}'.");

        user.RequestManagerRole();
        user.Updated();

        await _usersWriteRepository.UpdateAsync(user);

        return user;
    }
}
