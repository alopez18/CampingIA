using FluentValidation;

namespace CampingAI.Application.Commands.User.RegisterUser;
public class RegisterUserCommandHandler : Abstractions.Command.ICommandHandler<RegisterUserCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    readonly IValidator<RegisterUserCommand> _validator;
    #endregion

    public RegisterUserCommandHandler(Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                      Domain.Repositories.IUsersReadRepository usersReadRepository,
                                      Infra.Abstractions.IUnitOfWork unitOfWork,
                                      Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService,
                                      IValidator<RegisterUserCommand> validator) {
        _usersWriteRepository = usersWriteRepository;
        _usersReadRepository = usersReadRepository;
        _unitOfWork = unitOfWork;
        _passwordHashingService = passwordHashingService;
        _validator = validator;
    }

    public async Task<Domain.Entities.User> HandleAsync(RegisterUserCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        bool exists = await _usersReadRepository.ExistsAsync(command.Email);
        if (exists)
            throw new Domain.Exceptions.DomainException($"Ya existe un usuario con el email '{command.Email}'.");

        string? passwordHashed = _passwordHashingService.HashPassword(command.Password);

        var user = Domain.Entities.User.CreateNew(command.Email,
                                                  passwordHashed!,
                                                  command.Name,
                                                  Domain.Enums.UserRole.Comun);

        await _usersWriteRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }
}
