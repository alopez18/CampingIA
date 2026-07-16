namespace CampingAI.Application.Commands.User.GoogleLoginUser;
public record GoogleLoginUserCommand(string Email, string? Name) : Abstractions.Command.ICommand {
}
