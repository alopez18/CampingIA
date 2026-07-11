namespace CampingAI.Infra.Facilities.Mappers;

public class FacilitiesMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CampingAI_DB.T_FACILITIES, Domain.Entities.Facility>
{
    public override Domain.Entities.Facility Map(Models.CampingAI_DB.T_FACILITIES src)
    {
        return new Domain.Entities.Facility(
            src.FAC_IdFacility,
            src.FAC_Name);
    }

    public override Models.CampingAI_DB.T_FACILITIES ReverseMap(Domain.Entities.Facility src)
    {
        return new Models.CampingAI_DB.T_FACILITIES
        {
            FAC_IdFacility = src.Id,
            FAC_Name       = src.Name
        };
    }
}
