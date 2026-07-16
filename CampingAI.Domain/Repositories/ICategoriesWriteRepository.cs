namespace CampingAI.Domain.Repositories;
public interface ICategoriesWriteRepository {
    Task AddAsync(Entities.Category category);
    Task UpdateAsync(Entities.Category category);
}
