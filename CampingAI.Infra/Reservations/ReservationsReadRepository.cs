using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Reservations;

public class ReservationsReadRepository : Domain.Repositories.IReservationsReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_RESERVATIONS> _modelExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.ReservationsMapper _reservationsMapper;
    readonly ILogger<ReservationsReadRepository> _logger;
    #endregion

    public ReservationsReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_RESERVATIONS> modelExtractor,
                                      Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                      Mappers.ReservationsMapper reservationsMapper,
                                      ILogger<ReservationsReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _reservationsMapper = reservationsMapper;
        _logger = logger;
    }

    public async Task<Domain.Entities.Reservation?> GetByIdAsync(Guid id)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_IdReservation)} = @Id ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_DeletedOn)} IS NULL");
        string query = sql.ToString();

        Models.CampingAI_DB.T_RESERVATIONS? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_RESERVATIONS>(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservation by id {Id}. Query: {Query}", id, query);
            throw;
        }

        return row is null ? null : _reservationsMapper.Map(row);
    }

    public async Task<IEnumerable<Domain.Entities.Reservation>> GetByUserIdAsync(Guid userId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_UserId)} = @UserId ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_DeletedOn)} IS NULL");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_RESERVATIONS> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_RESERVATIONS>(query, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservations by user {UserId}. Query: {Query}", userId, query);
            throw;
        }

        return _reservationsMapper.Map(rows);
    }

    public async Task<IEnumerable<Domain.Entities.Reservation>> GetByCampingIdAsync(Guid campingId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CampingId)} = @CampingId ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_DeletedOn)} IS NULL");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_RESERVATIONS> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_RESERVATIONS>(query, new { CampingId = campingId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservations by camping {CampingId}. Query: {Query}", campingId, query);
            throw;
        }

        return _reservationsMapper.Map(rows);
    }
}
