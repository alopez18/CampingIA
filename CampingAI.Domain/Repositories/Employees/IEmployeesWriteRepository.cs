namespace CampingAI.Domain.Repositories.Employees;
public interface IEmployeesWriteRepository {
    Task<Entities.Employee?> GetById(Guid id, bool onlyNotDeleted);
    Task SaveAsync(Entities.Employee employee);

}