using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Categories;

public class CategoriesWriteRepository : Domain.Repositories.ICategoriesWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CATEGORIES> _modelExtractor;
    readonly Abstractions.IUnitOfWork _unitOfWork;
    readonly Mappers.CategoriesMapper _categoriesMapper;
    readonly ILogger<CategoriesWriteRepository> _logger;
    #endregion

    public CategoriesWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_CATEGORIES> modelExtractor,
                                     Abstractions.IUnitOfWork unitOfWork,
                                     Mappers.CategoriesMapper categoriesMapper,
                                     ILogger<CategoriesWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _unitOfWork = unitOfWork;
        _categoriesMapper = categoriesMapper;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.Category category)
    {
        var dbConnection = _unitOfWork.Connection;

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
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding category {CategoryId}. Query: {Query}", category.Id, query);
            throw;
        }
    }

    public async Task UpdateAsync(Domain.Entities.Category category)
    {
        var dbConnection = _unitOfWork.Connection;

        var model = _categoriesMapper.ReverseMap(category);
        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_Name)} = @{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_Name)}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)} = @{nameof(Models.CampingAI_DB.T_CATEGORIES.CAT_IdCategory)}");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}. Query: {Query}", category.Id, query);
            throw;
        }
    }
}
