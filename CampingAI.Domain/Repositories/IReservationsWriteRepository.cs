namespace CampingAI.Domain.Repositories;
public interface IReservationsWriteRepository {
    Task AddAsync(Entities.Reservation reservation);
    Task UpdateAsync(Entities.Reservation reservation);
    Task DeleteAsync(Guid id);
}
