namespace CampingAI.Application.Commands.User.DeleteUser;
public record DeleteUserCommand(Guid UserId) : Abstractions.Command.ICommand;
