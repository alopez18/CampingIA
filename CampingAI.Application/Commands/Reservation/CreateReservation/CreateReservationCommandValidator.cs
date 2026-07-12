using FluentValidation;

namespace CampingAI.Application.Commands.Reservation.CreateReservation;
public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand> {
    public CreateReservationCommandValidator() {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("El identificador de usuario es obligatorio.");
        RuleFor(x => x.CampingId).NotEmpty().WithMessage("El identificador del camping es obligatorio.");
        RuleFor(x => x.CheckIn).NotEmpty().WithMessage("La fecha de entrada es obligatoria.");
        RuleFor(x => x.CheckOut).NotEmpty().WithMessage("La fecha de salida es obligatoria.");
        RuleFor(x => x.TotalPrice).GreaterThan(0).WithMessage("El precio total debe ser mayor que cero.");
    }
}
