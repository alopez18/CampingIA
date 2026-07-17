namespace CampingAI.Domain.Repositories;
public interface ICampingsReadRepository {
    Task<Entities.Camping?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.Camping>> GetAllAsync();
    Task<IEnumerable<Entities.Camping>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Entities.Camping>> GetByOwnerAsync(Guid ownerId);
    Task<(IEnumerable<Entities.Camping> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null);
    Task<(IEnumerable<Entities.Camping> Items, int TotalCount)> SearchAsync(CampingSearchFilters filters);
}
