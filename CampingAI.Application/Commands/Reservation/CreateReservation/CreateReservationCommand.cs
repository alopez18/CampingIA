namespace CampingAI.Application.Commands.Reservation.CreateReservation;
public record CreateReservationCommand(
    Guid UserId,
    Guid CampingId,
    DateTime CheckIn,
    DateTime CheckOut,
    decimal TotalPrice) : Abstractions.Command.ICommand;
