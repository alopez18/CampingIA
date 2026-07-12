using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Countries;

public class CountriesReadRepository : Domain.Repositories.ICountriesReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_COUNTRIES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.CountriesMapper _countriesMapper;
    readonly ILogger<CountriesReadRepository> _logger;
    #endregion

    public CountriesReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_COUNTRIES> modelExtractor,
                                   Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                   Mappers.CountriesMapper countriesMapper,
                                   ILogger<CountriesReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _countriesMapper = countriesMapper;
        _logger = logger;
    }

    public async Task<IEnumerable<Domain.Entities.Country>> GetAllAsync()
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"ORDER BY {nameof(Models.CampingAI_DB.T_COUNTRIES.CNT_Name)}");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_COUNTRIES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_COUNTRIES>(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all countries. Query: {Query}", query);
            throw;
        }

        return _countriesMapper.Map(rows);
    }

    public async Task<Domain.Entities.Country?> GetByCodeAsync(string code)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_COUNTRIES.CNT_Code)} = @Code");
        string query = sql.ToString();

        Models.CampingAI_DB.T_COUNTRIES? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_COUNTRIES>(query, new { Code = code.ToUpperInvariant() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting country by code {Code}. Query: {Query}", code, query);
            throw;
        }

        return row is null ? null : _countriesMapper.Map(row);
    }
}
