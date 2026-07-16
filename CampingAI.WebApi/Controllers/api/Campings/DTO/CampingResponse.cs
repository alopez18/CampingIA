namespace CampingAI.WebApi.Controllers.api.Campings.DTO;
public record CampingResponse(Guid Id,
                               string Name,
                               string Description,
                               decimal Latitude,
                               decimal Longitude,
                               decimal PricePerNight,
                               Guid OwnerId,
                               Guid CategoryId,
                               Guid? ProvinciaId,
                               IReadOnlyList<Guid> FacilityIds,
                               IReadOnlyList<Guid> AdditionalCategoryIds,
                               DateTime CreatedOn,
                               DateTime UpdatedOn) {
}
