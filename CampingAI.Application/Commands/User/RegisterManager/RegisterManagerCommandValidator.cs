using FluentValidation;

namespace CampingAI.Application.Commands.User.RegisterManager;
public class RegisterManagerCommandValidator : AbstractValidator<RegisterManagerCommand> {
    public RegisterManagerCommandValidator() {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}
