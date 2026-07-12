namespace CampingAI.WebApi.Controllers.api.Location.Mappers;
public class CountryResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Country, DTO.CountryResponse> {
    public override DTO.CountryResponse Map(Domain.Entities.Country src) {
        return new DTO.CountryResponse(src.Id, src.Code, src.Name.ToString());
    }
}
