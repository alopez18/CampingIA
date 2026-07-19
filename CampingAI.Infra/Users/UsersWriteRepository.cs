using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Users;

public class UsersWriteRepository : Domain.Repositories.IUsersWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_USERS> _modelExtractor;
    readonly Abstractions.IUnitOfWork _unitOfWork;
    readonly Mappers.UsersMapper _usersMapper;
    readonly ILogger<UsersWriteRepository> _logger;
    #endregion

    public UsersWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_USERS> modelExtractor,
                                Abstractions.IUnitOfWork unitOfWork,
                                Mappers.UsersMapper usersMapper,
                                ILogger<UsersWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _unitOfWork = unitOfWork;
        _usersMapper = usersMapper;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.User user)
    {
        var dbConnection = _unitOfWork.Connection;

        var model = _usersMapper.ReverseMap(user);
        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_IdUser)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_Email)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_PasswordHashed)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_Name)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_RoleId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_ManagerStatus)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_CreatedOn)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_UpdatedOn)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_IdUser)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_Email)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_PasswordHashed)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_Name)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_RoleId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_ManagerStatus)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_CreatedOn)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_USERS.USR_UpdatedOn)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId}. Query: {Query}", user.Id, query);
            throw;
        }
    }

    public async Task UpdateAsync(Domain.Entities.User user)
    {
        var dbConnection = _unitOfWork.Connection;

        var model = _usersMapper.ReverseMap(user);
        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_Email)} = @{nameof(Models.CampingAI_DB.T_USERS.USR_Email)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_PasswordHashed)} = @{nameof(Models.CampingAI_DB.T_USERS.USR_PasswordHashed)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_Name)} = @{nameof(Models.CampingAI_DB.T_USERS.USR_Name)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_RoleId)} = @{nameof(Models.CampingAI_DB.T_USERS.USR_RoleId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_ManagerStatus)} = @{nameof(Models.CampingAI_DB.T_USERS.USR_ManagerStatus)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_UpdatedOn)} = @{nameof(Models.CampingAI_DB.T_USERS.USR_UpdatedOn)}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_USERS.USR_IdUser)} = @{nameof(Models.CampingAI_DB.T_USERS.USR_IdUser)}");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}. Query: {Query}", user.Id, query);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var dbConnection = _unitOfWork.Connection;

        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_USERS.USR_DeletedOn)} = @Now");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_USERS.USR_IdUser)} = @Id");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { Id = id, Now = DateTime.UtcNow }, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}. Query: {Query}", id, query);
            throw;
        }
    }
}
