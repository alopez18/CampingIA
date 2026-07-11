namespace CampingAI.Application.Commands.Employee.CreateEmployee;
public record CreateEmployeeCommand(string Username,
                              string? Name,
                              string Email,
                              string? Fax,
                              string? Password,
                              int CompanyId,
                              int PortalId,
                              int RoleId,
                              int StatusId,
                              string? Telephone) : Abstractions.Command.ICommand {
}