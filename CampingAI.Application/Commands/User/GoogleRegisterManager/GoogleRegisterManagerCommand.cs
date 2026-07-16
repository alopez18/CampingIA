namespace CampingAI.Application.Commands.User.GoogleRegisterManager;
public record GoogleRegisterManagerCommand(string Email, string? Name) : Abstractions.Command.ICommand {
}
