namespace CampingAI.Infra.Reservations.Mappers;

public class ReservationsMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CampingAI_DB.T_RESERVATIONS, Domain.Entities.Reservation>
{
    public override Domain.Entities.Reservation Map(Models.CampingAI_DB.T_RESERVATIONS src)
    {
        return new Domain.Entities.Reservation(
            src.RES_IdReservation,
            src.RES_UserId,
            src.RES_CampingId,
            src.RES_CheckIn,
            src.RES_CheckOut,
            src.RES_TotalPrice,
            src.RES_StatusId,
            src.RES_CreatedOn,
            src.RES_UpdatedOn,
            src.RES_DeletedOn);
    }

    public override Models.CampingAI_DB.T_RESERVATIONS ReverseMap(Domain.Entities.Reservation src)
    {
        return new Models.CampingAI_DB.T_RESERVATIONS
        {
            RES_IdReservation = src.Id,
            RES_UserId        = src.UserId,
            RES_CampingId     = src.CampingId,
            RES_CheckIn       = src.Dates.CheckIn,
            RES_CheckOut      = src.Dates.CheckOut,
            RES_TotalPrice    = src.TotalPrice.Value,
            RES_StatusId      = src.StatusId,
            RES_CreatedOn     = src.CreatedOn.Value,
            RES_UpdatedOn     = src.UpdatedOn.Value,
            RES_DeletedOn     = src.DeletedOn
        };
    }
}
