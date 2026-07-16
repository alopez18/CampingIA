namespace CampingAI.Application.Commands.User.RejectManager;
public class RejectManagerCommandHandler : Abstractions.Command.ICommandHandler<RejectManagerCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    #endregion

    public RejectManagerCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                       Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                       Infra.Abstractions.IUnitOfWork unitOfWork) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Domain.Entities.User> HandleAsync(RejectManagerCommand command) {
        var user = await _usersReadRepository.GetByIdAsync(command.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{command.UserId}'.");

        user.RejectManagerRole();
        user.Updated();

        await _usersWriteRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }
}
