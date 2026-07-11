using FluentValidation;

namespace CampingAI.Application.Commands.Employee.CreateEmployee;
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand> {
    public CreateEmployeeCommandValidator() {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre de usuario no puede superar 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El formato del email no es válido.")
            .MaximumLength(200).WithMessage("El email no puede superar 200 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("CompanyId debe ser mayor que 0.");

        RuleFor(x => x.PortalId)
            .GreaterThan(0).WithMessage("PortalId debe ser mayor que 0.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("RoleId debe ser mayor que 0.");

        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("StatusId debe ser mayor que 0.");
    }
}
