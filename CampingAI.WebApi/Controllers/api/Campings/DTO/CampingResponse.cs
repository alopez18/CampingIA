namespace CampingAI.WebApi.Controllers.api.Campings.DTO;
public record CampingResponse(Guid Id,
                               string Name,
                               string Description,
                               decimal Latitude,
                               decimal Longitude,
                               decimal PricePerNight,
                               Guid OwnerId,
                               int CategoryId,
                               Guid? ProvinciaId,
                               IReadOnlyList<Guid> FacilityIds,
                               DateTime CreatedOn,
                               DateTime UpdatedOn) {
}
