namespace CampingAI.Domain.Repositories;
public interface IFavoritesReadRepository {
    Task<IEnumerable<Entities.Favorite>> GetByUserIdAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId, Guid campingId);
}
