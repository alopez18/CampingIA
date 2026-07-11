namespace CampingAI.Application.Commands.Employee.UpdateEmployee;
public record UpdateEmployeeCommand(Guid Id,
                                    string Username,
                                    string? Name,
                                    string Email,
                                    string? Fax,
                                    string Password,
                                    int CompanyId,
                                    int PortalId,
                                    int RoleId,
                                    int StatusId,
                                    string? Telephone) : Abstractions.Command.ICommand {
}