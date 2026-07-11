namespace CampingAI.Application.Commands.Employee.DeleteEmployee;
public class DeleteEmployeeCommandHandler : Abstractions.Command.ICommandHandler<DeleteEmployeeCommand> {
    #region Dependencies
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.Employees.IEmployeesWriteRepository _employeesWriteRepository;

    #endregion


    public DeleteEmployeeCommandHandler(Domain.Repositories.Employees.IEmployeesWriteRepository employeesWriteRepository,
                                        Infra.Abstractions.IUnitOfWork unitOfWork) {
        _employeesWriteRepository = employeesWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(DeleteEmployeeCommand command) {
        var entity = await _employeesWriteRepository.GetById(command.Id, true);
        if (entity == null) {
            throw new KeyNotFoundException($"Employee with id {command.Id} not found.");
        }

        entity.SetDeleted();
        await _employeesWriteRepository.SaveAsync(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}