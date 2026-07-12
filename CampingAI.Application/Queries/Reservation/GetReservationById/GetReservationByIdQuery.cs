namespace CampingAI.Application.Queries.Reservation.GetReservationById;
public record GetReservationByIdQuery(Guid ReservationId, Guid UserId) : Abstractions.Query.IQuery<Domain.Entities.Reservation>;
