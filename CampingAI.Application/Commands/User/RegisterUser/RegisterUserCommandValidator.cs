using FluentValidation;

namespace CampingAI.Application.Commands.User.RegisterUser;
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand> {
    public RegisterUserCommandValidator() {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El formato del email no es válido.")
            .MaximumLength(200).WithMessage("El email no puede superar 200 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}
