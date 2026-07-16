using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Categories;

public class CategoriesWriteRepository : Domain.Repositories.ICategoriesWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CATEGORIES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.CategoriesMapper _categoriesMapper;
    readonly ILogger<CategoriesWriteRepository> _logger;
    #endregion

    public CategoriesWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_CATEGORIES> modelExtractor,
                                     Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                     Mappers.CategoriesMapper categoriesMapper,
                                     ILogger<CategoriesWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _categoriesMapper = categoriesMapper;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.Category category)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var model = _categoriesMapper.ReverseMap(category);
        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_Name)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_Name)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding category {CategoryId}. Query: {Query}", category.Id, query);
            throw;
        }
    }

    public async Task UpdateAsync(Domain.Entities.Category category)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var model = _categoriesMapper.ReverseMap(category);
        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_Name)} = @{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_Name)}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)} = @{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)}");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}. Query: {Query}", category.Id, query);
            throw;
        }
    }
}
