namespace CampingAI.Application.Queries.Reservation.GetReservationById;
public class GetReservationByIdQueryHandler : Abstractions.Query.IQueryHandler<GetReservationByIdQuery, Domain.Entities.Reservation> {

    #region Dependencias
    readonly Domain.Repositories.IReservationsReadRepository _reservationsReadRepository;
    #endregion

    public GetReservationByIdQueryHandler(Domain.Repositories.IReservationsReadRepository reservationsReadRepository) {
        _reservationsReadRepository = reservationsReadRepository;
    }

    public async Task<Domain.Entities.Reservation> HandleAsync(GetReservationByIdQuery query) {
        var reservation = await _reservationsReadRepository.GetByIdAsync(query.ReservationId)
            ?? throw new Domain.Exceptions.DomainException("Reserva no encontrada.");

        if (reservation.UserId != query.UserId)
            throw new Domain.Exceptions.DomainException("No tienes permiso para ver esta reserva.");

        return reservation;
    }
}
