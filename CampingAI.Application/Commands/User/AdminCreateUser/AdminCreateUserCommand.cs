namespace CampingAI.Application.Commands.User.AdminCreateUser;
public record AdminCreateUserCommand(string Email,
                                     string Password,
                                     string? Name,
                                     Domain.Enums.UserRole Role) : Abstractions.Command.ICommand;
