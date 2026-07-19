using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Categories;

public class CampingCategoriesWriteRepository : Domain.Repositories.ICampingCategoriesWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_CATEGORIES> _modelExtractor;
    readonly Abstractions.IUnitOfWork _unitOfWork;
    readonly ILogger<CampingCategoriesWriteRepository> _logger;
    #endregion

    public CampingCategoriesWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_CATEGORIES> modelExtractor,
                                            Abstractions.IUnitOfWork unitOfWork,
                                            ILogger<CampingCategoriesWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.CampingCategory campingCategory)
    {
        var dbConnection = _unitOfWork.Connection;

        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_IdCampingCategory)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CategoryId)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_IdCampingCategory)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CategoryId)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        var model = new Models.CampingAI_DB.T_CAMPING_CATEGORIES
        {
            CCA_IdCampingCategory = campingCategory.Id,
            CCA_CampingId         = campingCategory.CampingId,
            CCA_CategoryId        = campingCategory.CategoryId
        };

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding camping-category link {Id}. Query: {Query}", campingCategory.Id, query);
            throw;
        }
    }

    public async Task DeleteAsync(Guid campingId, Guid categoryId)
    {
        var dbConnection = _unitOfWork.Connection;

        var sql = new StringBuilder();
        sql.AppendLine($"DELETE FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)} = @CampingId ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CategoryId)} = @CategoryId");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { CampingId = campingId, CategoryId = categoryId }, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting camping-category link ({CampingId}, {CategoryId}). Query: {Query}", campingId, categoryId, query);
            throw;
        }
    }

    public async Task DeleteByCampingIdAsync(Guid campingId)
    {
        var dbConnection = _unitOfWork.Connection;

        var sql = new StringBuilder();
        sql.AppendLine($"DELETE FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)} = @CampingId");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { CampingId = campingId }, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting camping-category links for camping {CampingId}. Query: {Query}", campingId, query);
            throw;
        }
    }
}
