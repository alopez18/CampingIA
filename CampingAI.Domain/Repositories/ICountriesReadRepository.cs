namespace CampingAI.Domain.Repositories;
public interface ICountriesReadRepository {
    Task<IEnumerable<Entities.Country>> GetAllAsync();
    Task<Entities.Country?> GetByCodeAsync(string code);
}
