namespace CampingAI.Application.Commands.User.DeleteUser;
public class DeleteUserCommandHandler : Abstractions.Command.ICommandHandler<DeleteUserCommand> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    #endregion

    public DeleteUserCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                    Domain.Repositories.IUsersWriteRepository usersWriteRepository) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
    }

    public async Task HandleAsync(DeleteUserCommand command) {
        var exists = await _usersReadRepository.GetByIdAsync(command.UserId);
        if (exists is null)
            throw new KeyNotFoundException($"No existe ningún usuario con el id '{command.UserId}'.");

        await _usersWriteRepository.DeleteAsync(command.UserId);
    }
}
