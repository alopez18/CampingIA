using FluentValidation;

namespace CampingAI.Application.Commands.User.UpdateUser;
public class UpdateUserCommandHandler : Abstractions.Command.ICommandHandler<UpdateUserCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    readonly IValidator<UpdateUserCommand> _validator;
    #endregion

    public UpdateUserCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                    Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                    Infra.Abstractions.IUnitOfWork unitOfWork,
                                    Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService,
                                    IValidator<UpdateUserCommand> validator) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _unitOfWork = unitOfWork;
        _passwordHashingService = passwordHashingService;
        _validator = validator;
    }

    public async Task<Domain.Entities.User> HandleAsync(UpdateUserCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var user = await _usersReadRepository.GetByIdAsync(command.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{command.UserId}'.");

        if (command.Name is not null)
            user.UpdateProfile(command.Name);

        if (!string.IsNullOrWhiteSpace(command.Email))
            user.UpdateEmail(command.Email);

        if (!string.IsNullOrWhiteSpace(command.Password)) {
            string hashed = _passwordHashingService.HashPassword(command.Password)!;
            user.UpdatePassword(hashed);
        }

        user.Updated();

        await _usersWriteRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }
}
