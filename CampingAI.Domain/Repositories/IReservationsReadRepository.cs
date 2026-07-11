namespace CampingAI.Domain.Repositories;
public interface IReservationsReadRepository {
    Task<Entities.Reservation?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.Reservation>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Entities.Reservation>> GetByCampingIdAsync(Guid campingId);
}
