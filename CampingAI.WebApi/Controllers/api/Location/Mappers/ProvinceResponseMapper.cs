namespace CampingAI.WebApi.Controllers.api.Location.Mappers;
public class ProvinceResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Province, DTO.ProvinceResponse> {
    public override DTO.ProvinceResponse Map(Domain.Entities.Province src) {
        return new DTO.ProvinceResponse(src.Id, src.Code, src.Name.ToString(), src.CountryId);
    }
}
