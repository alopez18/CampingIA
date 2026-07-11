using FluentValidation;

namespace CampingAI.Application.Commands.User.UpdateUser;
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand> {
    public UpdateUserCommandValidator() {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El identificador de usuario es obligatorio.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es válido.")
            .MaximumLength(200).WithMessage("El email no puede superar 200 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Password)
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}
