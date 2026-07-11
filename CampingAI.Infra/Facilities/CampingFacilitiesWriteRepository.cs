using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Facilities;

public class CampingFacilitiesWriteRepository : Domain.Repositories.ICampingFacilitiesWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_FACILITIES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly ILogger<CampingFacilitiesWriteRepository> _logger;
    #endregion

    public CampingFacilitiesWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_FACILITIES> modelExtractor,
                                            Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                            ILogger<CampingFacilitiesWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.CampingFacility campingFacility)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_IdCampingFacility)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_FacilityId)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_IdCampingFacility)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_FacilityId)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        var model = new Models.CampingAI_DB.T_CAMPING_FACILITIES
        {
            CFE_IdCampingFacility = campingFacility.Id,
            CFE_CampingId         = campingFacility.CampingId,
            CFE_FacilityId        = campingFacility.FacilityId
        };

        try
        {
            await dbConnection.ExecuteAsync(query, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding camping-facility link {Id}. Query: {Query}", campingFacility.Id, query);
            throw;
        }
    }

    public async Task DeleteAsync(Guid campingId, Guid facilityId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"DELETE FROM {_modelExtractor.GetTableNameForSql()}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)} = @CampingId");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_FacilityId)} = @FacilityId");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { CampingId = campingId, FacilityId = facilityId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting camping-facility link camping={CampingId} facility={FacilityId}. Query: {Query}", campingId, facilityId, query);
            throw;
        }
    }

    public async Task DeleteByCampingIdAsync(Guid campingId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"DELETE FROM {_modelExtractor.GetTableNameForSql()}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)} = @CampingId");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { CampingId = campingId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all facilities for camping {CampingId}. Query: {Query}", campingId, query);
            throw;
        }
    }
}
