using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Favorites;

public class FavoritesReadRepository : Domain.Repositories.IFavoritesReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_FAVORITES> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.FavoritesMapper _favoritesMapper;
    readonly ILogger<FavoritesReadRepository> _logger;
    #endregion

    public FavoritesReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_FAVORITES> modelExtractor,
                                   Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                   Mappers.FavoritesMapper favoritesMapper,
                                   ILogger<FavoritesReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _favoritesMapper = favoritesMapper;
        _logger = logger;
    }

    public async Task<IEnumerable<Domain.Entities.Favorite>> GetByUserIdAsync(Guid userId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_UserId)} = @UserId");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_FAVORITES> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_FAVORITES>(query, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorites by user {UserId}. Query: {Query}", userId, query);
            throw;
        }

        return _favoritesMapper.Map(rows);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid campingId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT COUNT(1) FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_UserId)} = @UserId ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_FAVORITES.FAV_CampingId)} = @CampingId");
        string query = sql.ToString();

        try
        {
            int count = await dbConnection.ExecuteScalarAsync<int>(query, new { UserId = userId, CampingId = campingId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite existence user={UserId} camping={CampingId}. Query: {Query}", userId, campingId, query);
            throw;
        }
    }
}
