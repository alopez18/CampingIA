namespace CampingAI.Application.Queries.Reservation.GetUserReservations;
public record GetUserReservationsQuery(Guid UserId) : Abstractions.Query.IQuery<IEnumerable<Domain.Entities.Reservation>>;
