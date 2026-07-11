using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Favorites;

public class FavoritesWriteRepository : Domain.Repositories.IFavoritesWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_FAVORITES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly ILogger<FavoritesWriteRepository> _logger;
    #endregion

    public FavoritesWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_FAVORITES> modelExtractor,
                                    Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                    ILogger<FavoritesWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.Favorite favorite)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_IdFavorite)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_UserId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_CampingId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_CreatedAt)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_FAVORITES.FAV_IdFavorite)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_FAVORITES.FAV_UserId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_FAVORITES.FAV_CampingId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_FAVORITES.FAV_CreatedAt)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        var model = new Models.CampingAI_DB.T_FAVORITES
        {
            FAV_IdFavorite = favorite.Id,
            FAV_UserId     = favorite.UserId,
            FAV_CampingId  = favorite.CampingId,
            FAV_CreatedAt  = favorite.CreatedAt
        };

        try
        {
            await dbConnection.ExecuteAsync(query, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite {FavoriteId}. Query: {Query}", favorite.Id, query);
            throw;
        }
    }

    public async Task DeleteAsync(Guid userId, Guid campingId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"DELETE FROM {_modelExtractor.GetTableNameForSql()}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_UserId)} = @UserId");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_CampingId)} = @CampingId");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { UserId = userId, CampingId = campingId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting favorite user={UserId} camping={CampingId}. Query: {Query}", userId, campingId, query);
            throw;
        }
    }
}
