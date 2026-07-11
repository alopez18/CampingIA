namespace CampingAI.Domain.Repositories;
public interface IUsersWriteRepository {
    Task AddAsync(Entities.User user);
    Task UpdateAsync(Entities.User user);
    Task DeleteAsync(Guid id);
}
