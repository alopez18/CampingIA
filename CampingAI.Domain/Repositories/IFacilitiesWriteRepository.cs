namespace CampingAI.Domain.Repositories;
public interface IFacilitiesWriteRepository {
    Task AddAsync(Entities.Facility facility);
    Task UpdateAsync(Entities.Facility facility);
    Task DeleteAsync(Guid id);
}
