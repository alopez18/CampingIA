namespace CampingAI.WebApi.Controllers.api.Campings.DTO;
public record UpdateCampingRequest(string Name,
                                   string Description,
                                   decimal Latitude,
                                   decimal Longitude,
                                   decimal PricePerNight,
                                   int CategoryId,
                                   Guid? ProvinciaId,
                                   IEnumerable<Guid>? FacilityIds) {
}
