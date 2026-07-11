namespace CampingAI.Application.Commands.User.LoginUser;
public record LoginUserCommand(string Email,
                               string Password) : Abstractions.Command.ICommand {
}
