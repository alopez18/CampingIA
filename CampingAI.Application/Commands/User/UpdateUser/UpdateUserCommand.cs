namespace CampingAI.Application.Commands.User.UpdateUser;
public record UpdateUserCommand(Guid UserId,
                                string? Name,
                                string? Email,
                                string? Password) : Abstractions.Command.ICommand {
}
