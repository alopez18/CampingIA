namespace CampingAI.Application.Commands.User.RequestManagerRole;
public record RequestManagerRoleCommand(Guid UserId) : Abstractions.Command.ICommand {
}
