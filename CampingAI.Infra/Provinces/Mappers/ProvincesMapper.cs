namespace CampingAI.Infra.Provinces.Mappers;

public class ProvincesMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CampingAI_DB.T_PROVINCES, Domain.Entities.Province>
{
    public override Domain.Entities.Province Map(Models.CampingAI_DB.T_PROVINCES src)
    {
        return new Domain.Entities.Province(
            src.PRV_IdProvince,
            src.PRV_Code,
            src.PRV_Name,
            src.PRV_CountryId);
    }

    public override Models.CampingAI_DB.T_PROVINCES ReverseMap(Domain.Entities.Province src)
    {
        return new Models.CampingAI_DB.T_PROVINCES
        {
            PRV_IdProvince = src.Id,
            PRV_Code       = src.Code,
            PRV_Name       = src.Name,
            PRV_CountryId  = src.CountryId
        };
    }
}
