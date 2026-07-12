namespace CampingAI.Application.Queries.Reservation.GetUserReservations;
public class GetUserReservationsQueryHandler : Abstractions.Query.IQueryHandler<GetUserReservationsQuery, IEnumerable<Domain.Entities.Reservation>> {

    #region Dependencias
    readonly Domain.Repositories.IReservationsReadRepository _reservationsReadRepository;
    #endregion

    public GetUserReservationsQueryHandler(Domain.Repositories.IReservationsReadRepository reservationsReadRepository) {
        _reservationsReadRepository = reservationsReadRepository;
    }

    public async Task<IEnumerable<Domain.Entities.Reservation>> HandleAsync(GetUserReservationsQuery query) {
        return await _reservationsReadRepository.GetByUserIdAsync(query.UserId);
    }
}
