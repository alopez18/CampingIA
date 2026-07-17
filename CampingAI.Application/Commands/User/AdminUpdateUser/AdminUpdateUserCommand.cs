namespace CampingAI.Application.Commands.User.AdminUpdateUser;
public record AdminUpdateUserCommand(Guid UserId,
                                     string? Name,
                                     string? Email,
                                     Domain.Enums.UserRole? Role,
                                     string? NewPassword = null) : Abstractions.Command.ICommand;
