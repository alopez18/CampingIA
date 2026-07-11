using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampingAI.Infra.Employees;
public class EmployeesWriteRepository : Domain.Repositories.Employees.IEmployeesWriteRepository {
    #region Dependencies
    readonly Mappers.EmployeesMapper _employeesMapper;
    readonly Models.REDARBOR_DB.REDARBOR_TTContext _dbContext;
    readonly ILogger<EmployeesWriteRepository> _logger;
    #endregion


    public EmployeesWriteRepository(Models.REDARBOR_DB.REDARBOR_TTContext dbContext,
                                    ILogger<EmployeesWriteRepository> logger,
                                    Mappers.EmployeesMapper employeesMapper) {
        _dbContext = dbContext;
        _logger = logger;
        _employeesMapper = employeesMapper;
    }

    public async Task<Domain.Entities.Employee?> GetById(Guid id, bool onlyNotDeleted) {
        Models.REDARBOR_DB.T_EMPLOYEES? employeeDbModel = null;
        try {
            if (onlyNotDeleted) {
                employeeDbModel = await _dbContext.T_EMPLOYEES.AsNoTracking().Where(m => m.EMP_IdEmployee == id && m.EMP_DeletedOn == null).FirstOrDefaultAsync();
            } else {
                employeeDbModel = await _dbContext.T_EMPLOYEES.AsNoTracking().FirstOrDefaultAsync(m => m.EMP_IdEmployee == id);
            }
        } catch (Exception ex) {
            _logger.LogError(ex, $"Error on getting an employee with id {id}.");
            throw;
        }

        if (employeeDbModel == null) {
            return null;
        }
        var entity = _employeesMapper.Map(employeeDbModel);
        return entity;
    }

    public async Task SaveAsync(Domain.Entities.Employee employee) {
        if (employee.Id == Guid.Empty) {
            throw new ArgumentException("Employee Id cannot be empty", nameof(employee));
        }

        try {
            var existe = await _dbContext.T_EMPLOYEES.AsNoTracking().AnyAsync(m => m.EMP_IdEmployee == employee.Id);
            var employeeModel = _employeesMapper.ReverseMap(employee);
            if (existe) {
                _dbContext.T_EMPLOYEES.Update(employeeModel);
            } else {
                _dbContext.T_EMPLOYEES.Add(employeeModel);
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error on saving an employee.");
            throw;
        }
    }
}