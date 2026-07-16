namespace CampingAI.Domain.Repositories;
public interface ICategoriesReadRepository {
    Task<Entities.Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.Category>> GetAllAsync();
    Task<IEnumerable<Entities.Category>> GetByCampingIdAsync(Guid campingId);
}
