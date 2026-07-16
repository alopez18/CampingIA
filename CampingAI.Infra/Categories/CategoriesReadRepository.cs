using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Categories;

public class CategoriesReadRepository : Domain.Repositories.ICategoriesReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CATEGORIES> _modelExtractor;
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_CATEGORIES> _campingCategoriesExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.CategoriesMapper _categoriesMapper;
    readonly ILogger<CategoriesReadRepository> _logger;
    #endregion

    public CategoriesReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_CATEGORIES> modelExtractor,
                                    Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_CATEGORIES> campingCategoriesExtractor,
                                    Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                    Mappers.CategoriesMapper categoriesMapper,
                                    ILogger<CategoriesReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _campingCategoriesExtractor = campingCategoriesExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _categoriesMapper = categoriesMapper;
        _logger = logger;
    }

    public async Task<Domain.Entities.Category?> GetByIdAsync(Guid id)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)} = @Id");
        string query = sql.ToString();

        Models.CampingAI_DB.T_CATEGORIES? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_CATEGORIES>(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by id {Id}. Query: {Query}", id, query);
            throw;
        }

        return row is null ? null : _categoriesMapper.Map(row);
    }

    public async Task<IEnumerable<Domain.Entities.Category>> GetAllAsync()
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()}");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_CATEGORIES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CATEGORIES>(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories. Query: {Query}", query);
            throw;
        }

        return _categoriesMapper.Map(rows);
    }

    public async Task<IEnumerable<Domain.Entities.Category>> GetByCampingIdAsync(Guid campingId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT c.{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)},");
        sql.AppendLine($"       c.{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_Name)} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} c ");
        sql.AppendLine($"INNER JOIN {_campingCategoriesExtractor.GetTableNameForSql()} cc ");
        sql.AppendLine($"    ON c.{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)} = cc.{nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CategoryId)} ");
        sql.AppendLine($"WHERE cc.{nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)} = @CampingId");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_CATEGORIES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CATEGORIES>(query, new { CampingId = campingId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories by camping {CampingId}. Query: {Query}", campingId, query);
            throw;
        }

        return _categoriesMapper.Map(rows);
    }
}
