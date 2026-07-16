namespace CampingAI.Application.Commands.User.RequestManagerRole;
public class RequestManagerRoleCommandHandler : Abstractions.Command.ICommandHandler<RequestManagerRoleCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    #endregion

    public RequestManagerRoleCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                            Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                            Infra.Abstractions.IUnitOfWork unitOfWork) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Domain.Entities.User> HandleAsync(RequestManagerRoleCommand command) {
        var user = await _usersReadRepository.GetByIdAsync(command.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{command.UserId}'.");

        user.RequestManagerRole();
        user.Updated();

        await _usersWriteRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }
}
