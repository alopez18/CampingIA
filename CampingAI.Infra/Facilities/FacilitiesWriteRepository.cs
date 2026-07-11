using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Facilities;

public class FacilitiesWriteRepository : Domain.Repositories.IFacilitiesWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_FACILITIES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.FacilitiesMapper _facilitiesMapper;
    readonly ILogger<FacilitiesWriteRepository> _logger;
    #endregion

    public FacilitiesWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_FACILITIES> modelExtractor,
                                     Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                     Mappers.FacilitiesMapper facilitiesMapper,
                                     ILogger<FacilitiesWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _facilitiesMapper = facilitiesMapper;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.Facility facility)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var model = _facilitiesMapper.ReverseMap(facility);
        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_FACILITIES.FAC_Name)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_FACILITIES.FAC_Name)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding facility {FacilityId}. Query: {Query}", facility.Id, query);
            throw;
        }
    }

    public async Task UpdateAsync(Domain.Entities.Facility facility)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var model = _facilitiesMapper.ReverseMap(facility);
        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_FACILITIES.FAC_Name)} = @{nameof(Models.CampingAI_DB.T_FACILITIES.FAC_Name)}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)} = @{nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)}");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating facility {FacilityId}. Query: {Query}", facility.Id, query);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"DELETE FROM {_modelExtractor.GetTableNameForSql()}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)} = @Id");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting facility {FacilityId}. Query: {Query}", id, query);
            throw;
        }
    }
}
