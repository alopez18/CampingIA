namespace CampingAI.Domain.Repositories;
public interface ICampingCategoriesWriteRepository {
    Task AddAsync(Entities.CampingCategory campingCategory);
    Task DeleteAsync(Guid campingId, Guid categoryId);
    Task DeleteByCampingIdAsync(Guid campingId);
}
