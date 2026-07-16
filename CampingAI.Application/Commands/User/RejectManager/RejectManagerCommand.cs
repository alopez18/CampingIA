namespace CampingAI.Application.Commands.User.RejectManager;
public record RejectManagerCommand(Guid UserId) : Abstractions.Command.ICommand {
}
