namespace CampingAI.Domain.Repositories;
public interface ICampingFacilitiesWriteRepository {
    Task AddAsync(Entities.CampingFacility campingFacility);
    Task DeleteAsync(Guid campingId, Guid facilityId);
    Task DeleteByCampingIdAsync(Guid campingId);
}
