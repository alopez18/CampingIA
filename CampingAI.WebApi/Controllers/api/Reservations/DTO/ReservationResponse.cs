namespace CampingAI.WebApi.Controllers.api.Reservations.DTO;
public class ReservationResponse {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CampingId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Nights { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}
