using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Provinces;

public class ProvincesReadRepository : Domain.Repositories.IProvincesReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_PROVINCES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.ProvincesMapper _provincesMapper;
    readonly ILogger<ProvincesReadRepository> _logger;
    #endregion

    public ProvincesReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_PROVINCES> modelExtractor,
                                   Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                   Mappers.ProvincesMapper provincesMapper,
                                   ILogger<ProvincesReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _provincesMapper = provincesMapper;
        _logger = logger;
    }

    public async Task<IEnumerable<Domain.Entities.Province>> GetAllAsync()
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"ORDER BY {nameof(Models.CampingAI_DB.T_PROVINCES.PRV_Name)}");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_PROVINCES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_PROVINCES>(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all provinces. Query: {Query}", query);
            throw;
        }

        return _provincesMapper.Map(rows);
    }

    public async Task<Domain.Entities.Province?> GetByIdAsync(Guid id)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_PROVINCES.PRV_IdProvince)} = @Id");
        string query = sql.ToString();

        Models.CampingAI_DB.T_PROVINCES? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_PROVINCES>(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting province by id {Id}. Query: {Query}", id, query);
            throw;
        }

        return row is null ? null : _provincesMapper.Map(row);
    }

    public async Task<Domain.Entities.Province?> GetByCodeAsync(string code)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_PROVINCES.PRV_Code)} = @Code");
        string query = sql.ToString();

        Models.CampingAI_DB.T_PROVINCES? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_PROVINCES>(query, new { Code = code.ToUpperInvariant() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting province by code {Code}. Query: {Query}", code, query);
            throw;
        }

        return row is null ? null : _provincesMapper.Map(row);
    }

    public async Task<IEnumerable<Domain.Entities.Province>> GetByCountryIdAsync(Guid countryId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_PROVINCES.PRV_CountryId)} = @CountryId ");
        sql.AppendLine($"ORDER BY {nameof(Models.CampingAI_DB.T_PROVINCES.PRV_Name)}");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_PROVINCES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_PROVINCES>(query, new { CountryId = countryId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provinces by country {CountryId}. Query: {Query}", countryId, query);
            throw;
        }

        return _provincesMapper.Map(rows);
    }
}
