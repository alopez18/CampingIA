namespace CampingAI.WebApi.Controllers.api.Reservations.Mappers;
public class ReservationResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Reservation, DTO.ReservationResponse> {
    public override DTO.ReservationResponse Map(Domain.Entities.Reservation src) {
        return new DTO.ReservationResponse {
            Id         = src.Id,
            UserId     = src.UserId,
            CampingId  = src.CampingId,
            CheckIn    = src.Dates.CheckIn,
            CheckOut   = src.Dates.CheckOut,
            Nights     = src.Dates.Nights,
            TotalPrice = src.TotalPrice.Value,
            Status     = ((Domain.Enums.ReservationStatus)src.StatusId).ToString(),
            CreatedOn  = src.CreatedOn.Value
        };
    }
}
