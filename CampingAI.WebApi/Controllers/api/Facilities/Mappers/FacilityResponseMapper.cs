namespace CampingAI.WebApi.Controllers.api.Facilities.Mappers;
public class FacilityResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Facility, DTO.FacilityResponse> {
    public override DTO.FacilityResponse Map(Domain.Entities.Facility src) =>
        new(src.Id, src.Name.ToString());
}
