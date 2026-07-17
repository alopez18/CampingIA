namespace CampingAI.Application.Commands.User.RejectManager;
public class RejectManagerCommandHandler : Abstractions.Command.ICommandHandler<RejectManagerCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    #endregion

    public RejectManagerCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                       Domain.Repositories.IUsersWriteRepository usersWriteRepository) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
    }

    public async Task<Domain.Entities.User> HandleAsync(RejectManagerCommand command) {
        var user = await _usersReadRepository.GetByIdAsync(command.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{command.UserId}'.");

        user.RejectManagerRole();
        user.Updated();

        await _usersWriteRepository.UpdateAsync(user);

        return user;
    }
}
