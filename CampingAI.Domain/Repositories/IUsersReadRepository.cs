namespace CampingAI.Domain.Repositories;
public interface IUsersReadRepository {
    Task<Entities.User?> GetByIdAsync(Guid id);
    Task<Entities.User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<(IEnumerable<Entities.User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null);
    Task<IEnumerable<Entities.User>> GetPendingManagersAsync();
}
