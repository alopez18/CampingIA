
using FluentValidation;

namespace CampingAI.Application.Commands.Employee.CreateEmployee;
public class CreateEmployeeCommandHandler : Abstractions.Command.ICommandHandler<CreateEmployeeCommand, Domain.Entities.Employee> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.Employees.IEmployeesWriteRepository _employeesWriteRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    readonly IValidator<CreateEmployeeCommand> _validator;
    #endregion

    public CreateEmployeeCommandHandler(Domain.Repositories.Employees.IEmployeesWriteRepository employeesWriteRepository,
                                        Infra.Abstractions.IUnitOfWork unitOfWork,
                                        Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService,
                                        IValidator<CreateEmployeeCommand> validator) {
        _employeesWriteRepository = employeesWriteRepository;
        _unitOfWork = unitOfWork;
        _passwordHashingService = passwordHashingService;
        _validator = validator;
    }


    public async Task<Domain.Entities.Employee> HandleAsync(CreateEmployeeCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        string? passwordHashed = _passwordHashingService.HashPassword(command.Password);
        var employee = Domain.Entities.Employee.CreateNew(command.Username,
                                           command.Name,
                                           command.Email,
                                           command.Fax,
                                           passwordHashed, //Enviamos el password nulo para que el dominio gestione el error.
                                           command.CompanyId,
                                           command.PortalId,
                                           command.RoleId,
                                           command.StatusId,
                                           command.Telephone);


        await _employeesWriteRepository.SaveAsync(employee);
        await _unitOfWork.SaveChangesAsync();
        return employee;
    }
}
