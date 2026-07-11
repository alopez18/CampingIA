namespace CampingAI.Domain.Repositories.Employees;
public interface IEmpoyeesReadRepository {
    Task<IEnumerable<Entities.Employee>> GetAllAsync(bool onlyNotDeleted);
    Task<Entities.Employee?> Get(Guid id, bool onlyNotDeleted);

}