namespace CampingAI.Domain.Repositories;
public interface IFavoritesWriteRepository {
    Task AddAsync(Entities.Favorite favorite);
    Task DeleteAsync(Guid userId, Guid campingId);
}
