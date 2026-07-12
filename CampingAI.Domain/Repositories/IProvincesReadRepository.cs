namespace CampingAI.Domain.Repositories;
public interface IProvincesReadRepository {
    Task<IEnumerable<Entities.Province>> GetAllAsync();
    Task<Entities.Province?> GetByIdAsync(Guid id);
    Task<Entities.Province?> GetByCodeAsync(string code);
    Task<IEnumerable<Entities.Province>> GetByCountryIdAsync(Guid countryId);
}
