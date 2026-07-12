namespace CampingAI.WebApi.Controllers.api.Campings.Mappers;
public class CampingResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Camping, DTO.CampingResponse> {
    public override DTO.CampingResponse Map(Domain.Entities.Camping src) =>
        new(src.Id,
            src.Name.ToString(),
            src.Description.ToString(),
            src.Latitude.Value,
            src.Longitude.Value,
            src.PricePerNight.Value,
            src.OwnerId,
            src.CategoryId,
            src.ProvinciaId,
            src.FacilityIds,
            src.CreatedOn.Value,
            src.UpdatedOn.Value);
}
