namespace CampingAI.Application.Commands.Employee.DeleteEmployee;
public record DeleteEmployeeCommand(Guid Id) : Abstractions.Command.ICommand;