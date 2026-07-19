using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Campings;

public class CampingsWriteRepository : Domain.Repositories.ICampingsWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPINGS> _modelExtractor;
    readonly Abstractions.IUnitOfWork _unitOfWork;
    readonly Mappers.CampingsMapper _campingsMapper;
    readonly ILogger<CampingsWriteRepository> _logger;
    #endregion

    public CampingsWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPINGS> modelExtractor,
                                   Abstractions.IUnitOfWork unitOfWork,
                                   Mappers.CampingsMapper campingsMapper,
                                   ILogger<CampingsWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _unitOfWork = unitOfWork;
        _campingsMapper = campingsMapper;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.Camping camping)
    {
        var dbConnection = _unitOfWork.Connection;

        var model = _campingsMapper.ReverseMap(camping);
        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Name)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Description)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Latitude)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Longitude)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_PricePerNight)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_OwnerId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CategoryId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CreatedOn)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_UpdatedOn)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Name)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Description)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Latitude)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Longitude)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_PricePerNight)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_OwnerId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CategoryId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CreatedOn)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_UpdatedOn)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding camping {CampingId}. Query: {Query}", camping.Id, query);
            throw;
        }
    }

    public async Task UpdateAsync(Domain.Entities.Camping camping)
    {
        var dbConnection = _unitOfWork.Connection;

        var model = _campingsMapper.ReverseMap(camping);
        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Name)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Name)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Description)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Description)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Latitude)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Latitude)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Longitude)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Longitude)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_PricePerNight)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_PricePerNight)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CategoryId)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CategoryId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_UpdatedOn)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_UpdatedOn)}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)} = @{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)}");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating camping {CampingId}. Query: {Query}", camping.Id, query);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var dbConnection = _unitOfWork.Connection;

        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} = @Now");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)} = @Id");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { Id = id, Now = DateTime.UtcNow }, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting camping {CampingId}. Query: {Query}", id, query);
            throw;
        }
    }
}
