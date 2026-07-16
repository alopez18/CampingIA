namespace CampingAI.WebApi.Controllers.api.Campings.DTO;
public record CreateCampingRequest(string Name,
                                   string Description,
                                   decimal Latitude,
                                   decimal Longitude,
                                   decimal PricePerNight,
                                   Guid OwnerId,
                                   Guid CategoryId,
                                   Guid? ProvinciaId,
                                   IEnumerable<Guid>? FacilityIds,
                                   IEnumerable<Guid>? AdditionalCategoryIds) {
}
