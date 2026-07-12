namespace CampingAI.Infra.Countries.Mappers;

public class CountriesMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CampingAI_DB.T_COUNTRIES, Domain.Entities.Country>
{
    public override Domain.Entities.Country Map(Models.CampingAI_DB.T_COUNTRIES src)
    {
        return new Domain.Entities.Country(
            src.CNT_IdCountry,
            src.CNT_Code,
            src.CNT_Name);
    }

    public override Models.CampingAI_DB.T_COUNTRIES ReverseMap(Domain.Entities.Country src)
    {
        return new Models.CampingAI_DB.T_COUNTRIES
        {
            CNT_IdCountry = src.Id,
            CNT_Code      = src.Code,
            CNT_Name      = src.Name
        };
    }
}
