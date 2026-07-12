namespace CampingAI.Domain.Repositories;
public interface ICampingsReadRepository {
    Task<Entities.Camping?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.Camping>> GetAllAsync();
    Task<IEnumerable<Entities.Camping>> GetByCategoryAsync(int categoryId);
    Task<(IEnumerable<Entities.Camping> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<(IEnumerable<Entities.Camping> Items, int TotalCount)> SearchAsync(CampingSearchFilters filters);
}
