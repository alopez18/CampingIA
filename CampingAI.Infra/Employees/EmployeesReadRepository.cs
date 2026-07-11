using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Employees;
public class EmployeesReadRepository : Domain.Repositories.Employees.IEmpoyeesReadRepository {

    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.REDARBOR_DB.T_EMPLOYEES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.EmployeesMapper _employeesMapper;
    readonly ILogger<EmployeesReadRepository> _logger;
    #endregion


    public EmployeesReadRepository(Abstractions.ModelExtractor<Models.REDARBOR_DB.T_EMPLOYEES> modelExtractor,
                                  Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                  Mappers.EmployeesMapper employeesMapper,
                                  ILogger<EmployeesReadRepository> logger) {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _employeesMapper = employeesMapper;
        _logger = logger;
    }


    public async Task<Domain.Entities.Employee?> Get(Guid id, bool onlyNotDeleted) {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        StringBuilder sql = new();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.REDARBOR_DB.T_EMPLOYEES.EMP_IdEmployee)} = @IdEmployee ");
        if (onlyNotDeleted) {
            sql.AppendLine($"AND {nameof(Models.REDARBOR_DB.T_EMPLOYEES.EMP_DeletedOn)} IS NULL ");
        }
        sql.AppendLine($"order by {nameof(Models.REDARBOR_DB.T_EMPLOYEES.EMP_CreatedOn)} DESC");
        string query = sql.ToString();
        Models.REDARBOR_DB.T_EMPLOYEES? modelDB;
        try {
            modelDB = await dbConnection.QueryFirstOrDefaultAsync<Models.REDARBOR_DB.T_EMPLOYEES>(query, new { IdEmployee = id.ToString() });
        } catch (Exception ex) {
            _logger.LogError(ex, $"Error on get employee from database with id {id}. Query: {query}");
            throw;
        }
        if (modelDB == null) {
            return null;
        }
        var result = _employeesMapper.Map(modelDB);


        return result;
    }

    public async Task<IEnumerable<Domain.Entities.Employee>> GetAllAsync(bool onlyNotDeleted) {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        StringBuilder sql = new();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        if (onlyNotDeleted) {
            sql.AppendLine($"WHERE {nameof(Models.REDARBOR_DB.T_EMPLOYEES.EMP_DeletedOn)} IS NULL");
        }
        sql.AppendLine($"order by {nameof(Models.REDARBOR_DB.T_EMPLOYEES.EMP_CreatedOn)} DESC");
        string query = sql.ToString();
        IEnumerable<Models.REDARBOR_DB.T_EMPLOYEES> modelsDB;
        try {
            modelsDB = await dbConnection.QueryAsync<Models.REDARBOR_DB.T_EMPLOYEES>(query);
        } catch (Exception ex) {
            _logger.LogError(ex, $"Error on get all employees from database. Query: {query}");
            throw;
        }

        var result = _employeesMapper.Map(modelsDB);
        return result;
    }
}