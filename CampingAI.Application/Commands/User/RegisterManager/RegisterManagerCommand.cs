namespace CampingAI.Application.Commands.User.RegisterManager;
public record RegisterManagerCommand(string Email, string? Name, string Password) : Abstractions.Command.ICommand;
