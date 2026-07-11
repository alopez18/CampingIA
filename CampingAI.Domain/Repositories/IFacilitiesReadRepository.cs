namespace CampingAI.Domain.Repositories;
public interface IFacilitiesReadRepository {
    Task<Entities.Facility?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.Facility>> GetAllAsync();
    Task<IEnumerable<Entities.Facility>> GetByCampingIdAsync(Guid campingId);
}
