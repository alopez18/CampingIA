namespace CampingAI.Application.Commands.Reservation.CancelReservation;
public record CancelReservationCommand(Guid ReservationId, Guid UserId) : Abstractions.Command.ICommand;
