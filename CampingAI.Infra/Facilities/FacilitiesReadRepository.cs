using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Facilities;

public class FacilitiesReadRepository : Domain.Repositories.IFacilitiesReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_FACILITIES> _modelExtractor;
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_FACILITIES> _campingFacilitiesExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.FacilitiesMapper _facilitiesMapper;
    readonly ILogger<FacilitiesReadRepository> _logger;
    #endregion

    public FacilitiesReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_FACILITIES> modelExtractor,
                                    Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_FACILITIES> campingFacilitiesExtractor,
                                    Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                    Mappers.FacilitiesMapper facilitiesMapper,
                                    ILogger<FacilitiesReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _campingFacilitiesExtractor = campingFacilitiesExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _facilitiesMapper = facilitiesMapper;
        _logger = logger;
    }

    public async Task<Domain.Entities.Facility?> GetByIdAsync(Guid id)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)} = @Id");
        string query = sql.ToString();

        Models.CampingAI_DB.T_FACILITIES? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_FACILITIES>(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting facility by id {Id}. Query: {Query}", id, query);
            throw;
        }

        return row is null ? null : _facilitiesMapper.Map(row);
    }

    public async Task<IEnumerable<Domain.Entities.Facility>> GetAllAsync()
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()}");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_FACILITIES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_FACILITIES>(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all facilities. Query: {Query}", query);
            throw;
        }

        return _facilitiesMapper.Map(rows);
    }

    public async Task<IEnumerable<Domain.Entities.Facility>> GetByCampingIdAsync(Guid campingId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT f.{nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)},");
        sql.AppendLine($"       f.{nameof(Models.CampingAI_DB.T_FACILITIES.FAC_Name)} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} f ");
        sql.AppendLine($"INNER JOIN {_campingFacilitiesExtractor.GetTableNameForSql()} cf ");
        sql.AppendLine($"    ON f.{nameof(Models.CampingAI_DB.T_FACILITIES.FAC_IdFacility)} = cf.{nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_FacilityId)} ");
        sql.AppendLine($"WHERE cf.{nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)} = @CampingId");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_FACILITIES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_FACILITIES>(query, new { CampingId = campingId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting facilities by camping {CampingId}. Query: {Query}", campingId, query);
            throw;
        }

        return _facilitiesMapper.Map(rows);
    }
}
