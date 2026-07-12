namespace CampingAI.WebApi.Controllers.api.Reservations.DTO;
public record CreateReservationRequest(Guid CampingId, DateTime CheckIn, DateTime CheckOut, decimal TotalPrice);
