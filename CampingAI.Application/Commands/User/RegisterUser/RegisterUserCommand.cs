namespace CampingAI.Application.Commands.User.RegisterUser;
public record RegisterUserCommand(string Email,
                                  string Password,
                                  string? Name) : Abstractions.Command.ICommand {
}
