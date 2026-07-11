namespace CampingAI.Application.Commands.Employee.UpdateEmployee;
public class UpdateEmployeeeCommandHandler : Abstractions.Command.ICommandHandler<UpdateEmployeeCommand, Domain.Entities.Employee> {
    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.Employees.IEmployeesWriteRepository _employeesWriteRepository;
    readonly Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    #endregion

    public UpdateEmployeeeCommandHandler(Domain.Repositories.Employees.IEmployeesWriteRepository employeesWriteRepository,
                                         Infra.Abstractions.IUnitOfWork unitOfWork,
                                         Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService) {
        _employeesWriteRepository = employeesWriteRepository;
        _unitOfWork = unitOfWork;
        _passwordHashingService = passwordHashingService;
    }



    public async Task<Domain.Entities.Employee> HandleAsync(UpdateEmployeeCommand command) {
        Domain.Entities.Employee? employeeEntity = await _employeesWriteRepository.GetById(command.Id, true);
        if (employeeEntity == null) {
            throw new KeyNotFoundException($"Employee with id {command.Id} not found.");
        }

        employeeEntity.UpdateIdentity(command.Username, command.Name, command.Email, command.Fax, command.Telephone);
        employeeEntity.UpdateIdentifiers(command.CompanyId, command.PortalId, command.RoleId, command.StatusId);

        if (!string.IsNullOrWhiteSpace(command.Password)) {
            string? passwordHashed = _passwordHashingService.HashPassword(command.Password);
            employeeEntity.UpdatePassword(passwordHashed);//Enviamos el password nulo para que el dominio gestione el error.
        }
        await _employeesWriteRepository.SaveAsync(employeeEntity);
        await _unitOfWork.SaveChangesAsync();

        return employeeEntity;
    }
}