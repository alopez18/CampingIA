namespace CampingAI.Application.Commands.User.ApproveManager;
public record ApproveManagerCommand(Guid UserId) : Abstractions.Command.ICommand {
}
