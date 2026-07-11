using FluentValidation;

namespace CampingAI.Application.Commands.User.LoginUser;
public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand> {
    public LoginUserCommandValidator() {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El formato del email no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
