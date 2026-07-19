using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Reservations;

public class ReservationsWriteRepository : Domain.Repositories.IReservationsWriteRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_RESERVATIONS> _modelExtractor;
    readonly Abstractions.IUnitOfWork _unitOfWork;
    readonly Mappers.ReservationsMapper _reservationsMapper;
    readonly ILogger<ReservationsWriteRepository> _logger;
    #endregion

    public ReservationsWriteRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_RESERVATIONS> modelExtractor,
                                       Abstractions.IUnitOfWork unitOfWork,
                                       Mappers.ReservationsMapper reservationsMapper,
                                       ILogger<ReservationsWriteRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _unitOfWork = unitOfWork;
        _reservationsMapper = reservationsMapper;
        _logger = logger;
    }

    public async Task AddAsync(Domain.Entities.Reservation reservation)
    {
        var dbConnection = _unitOfWork.Connection;

        var model = _reservationsMapper.ReverseMap(reservation);
        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {_modelExtractor.GetTableNameForSql()} (");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_IdReservation)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_UserId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CampingId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckIn)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckOut)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_TotalPrice)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_StatusId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CreatedOn)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_UpdatedOn)}");
        sql.AppendLine($") VALUES (");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_IdReservation)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_UserId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CampingId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckIn)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckOut)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_TotalPrice)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_StatusId)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CreatedOn)},");
        sql.AppendLine($"    @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_UpdatedOn)}");
        sql.AppendLine($")");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding reservation {ReservationId}. Query: {Query}", reservation.Id, query);
            throw;
        }
    }

    public async Task UpdateAsync(Domain.Entities.Reservation reservation)
    {
        var dbConnection = _unitOfWork.Connection;

        var model = _reservationsMapper.ReverseMap(reservation);
        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckIn)} = @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckIn)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckOut)} = @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_CheckOut)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_TotalPrice)} = @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_TotalPrice)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_StatusId)} = @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_StatusId)},");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_UpdatedOn)} = @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_UpdatedOn)}");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_IdReservation)} = @{nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_IdReservation)}");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, model, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reservation {ReservationId}. Query: {Query}", reservation.Id, query);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var dbConnection = _unitOfWork.Connection;

        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {_modelExtractor.GetTableNameForSql()} SET");
        sql.AppendLine($"    {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_DeletedOn)} = @Now");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_RESERVATIONS.RES_IdReservation)} = @Id");
        string query = sql.ToString();

        try
        {
            await dbConnection.ExecuteAsync(query, new { Id = id, Now = DateTime.UtcNow }, _unitOfWork.CurrentTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting reservation {ReservationId}. Query: {Query}", id, query);
            throw;
        }
    }
}
