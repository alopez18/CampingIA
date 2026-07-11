using FluentValidation;

namespace CampingAI.Application.Commands.Camping.UpdateCamping;
public class UpdateCampingCommandValidator : AbstractValidator<UpdateCampingCommand> {
    public UpdateCampingCommandValidator() {
        RuleFor(x => x.CampingId)
            .NotEqual(Guid.Empty).WithMessage("El CampingId no puede estar vacío.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del camping es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción del camping es obligatoria.")
            .MaximumLength(2000).WithMessage("La descripción no puede superar los 2000 caracteres.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m).WithMessage("La latitud debe estar entre -90 y 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m).WithMessage("La longitud debe estar entre -180 y 180.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0m).WithMessage("El precio por noche debe ser mayor que 0.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("El CategoryId debe ser mayor que 0.");
    }
}
