using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Users;

public class UsersReadRepository : Domain.Repositories.IUsersReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_USERS> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.UsersMapper _usersMapper;
    readonly ILogger<UsersReadRepository> _logger;
    #endregion

    public UsersReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_USERS> modelExtractor,
                               Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                               Mappers.UsersMapper usersMapper,
                               ILogger<UsersReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _usersMapper = usersMapper;
        _logger = logger;
    }

    public async Task<Domain.Entities.User?> GetByIdAsync(Guid id)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_USERS.USR_IdUser)} = @Id ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_USERS.USR_DeletedOn)} IS NULL");
        string query = sql.ToString();

        Models.CampingAI_DB.T_USERS? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_USERS>(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id {Id}. Query: {Query}", id, query);
            throw;
        }

        return row is null ? null : _usersMapper.Map(row);
    }

    public async Task<Domain.Entities.User?> GetByEmailAsync(string email)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_USERS.USR_Email)} = @Email ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_USERS.USR_DeletedOn)} IS NULL");
        string query = sql.ToString();

        Models.CampingAI_DB.T_USERS? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_USERS>(query, new { Email = email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}. Query: {Query}", email, query);
            throw;
        }

        return row is null ? null : _usersMapper.Map(row);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT COUNT(1) FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_USERS.USR_Email)} = @Email ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_USERS.USR_DeletedOn)} IS NULL");
        string query = sql.ToString();

        try
        {
            int count = await dbConnection.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user existence for email {Email}. Query: {Query}", email, query);
            throw;
        }
    }

    public async Task<IEnumerable<Domain.Entities.User>> GetPendingManagersAsync()
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_USERS.USR_ManagerStatus)} = @Status ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_USERS.USR_DeletedOn)} IS NULL");
        string query = sql.ToString();

        try
        {
            var rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_USERS>(
                query, new { Status = (int)Domain.Enums.ManagerApprovalStatus.Pending });
            return rows.Select(_usersMapper.Map);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending managers. Query: {Query}", query);
            throw;
        }
    }
}
