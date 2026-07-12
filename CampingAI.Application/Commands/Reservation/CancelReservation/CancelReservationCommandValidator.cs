using FluentValidation;

namespace CampingAI.Application.Commands.Reservation.CancelReservation;
public class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand> {
    public CancelReservationCommandValidator() {
        RuleFor(x => x.ReservationId).NotEmpty().WithMessage("El identificador de reserva es obligatorio.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("El identificador de usuario es obligatorio.");
    }
}
