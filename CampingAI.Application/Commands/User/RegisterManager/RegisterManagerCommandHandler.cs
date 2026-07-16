using FluentValidation;

namespace CampingAI.Application.Commands.User.RegisterManager;
public class RegisterManagerCommandHandler : Abstractions.Command.ICommandHandler<RegisterManagerCommand, Domain.Entities.User> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    readonly IValidator<RegisterManagerCommand> _validator;
    #endregion

    public RegisterManagerCommandHandler(Domain.Repositories.IUsersReadRepository usersReadRepository,
                                         Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                                         Infra.Abstractions.IUnitOfWork unitOfWork,
                                         Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService,
                                         IValidator<RegisterManagerCommand> validator) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _unitOfWork = unitOfWork;
        _passwordHashingService = passwordHashingService;
        _validator = validator;
    }

    public async Task<Domain.Entities.User> HandleAsync(RegisterManagerCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var existing = await _usersReadRepository.GetByEmailAsync(command.Email);
        if (existing is not null)
            throw new Domain.Exceptions.DomainException($"Ya existe un usuario con el email '{command.Email}'. Inicia sesión para solicitar el rol de gestor.");

        string? passwordHashed = _passwordHashingService.HashPassword(command.Password);

        var user = Domain.Entities.User.CreateNew(command.Email,
                                                  passwordHashed!,
                                                  command.Name,
                                                  Domain.Enums.UserRole.Comun);
        user.RequestManagerRole();

        await _usersWriteRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }
}
