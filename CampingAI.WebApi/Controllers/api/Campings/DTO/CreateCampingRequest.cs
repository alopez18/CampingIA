namespace CampingAI.WebApi.Controllers.api.Campings.DTO;
public record CreateCampingRequest(string Name,
                                   string Description,
                                   decimal Latitude,
                                   decimal Longitude,
                                   decimal PricePerNight,
                                   Guid OwnerId,
                                   int CategoryId,
                                   IEnumerable<Guid>? FacilityIds) {
}
