namespace CampingAI.Domain.Repositories;
public interface ICampingsWriteRepository {
    Task AddAsync(Entities.Camping camping);
    Task UpdateAsync(Entities.Camping camping);
    Task DeleteAsync(Guid id);
}
