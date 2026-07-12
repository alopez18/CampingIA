namespace CampingAI.Infra.Campings.Mappers;

public class CampingsMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CampingAI_DB.T_CAMPINGS, Domain.Entities.Camping>
{
    public override Domain.Entities.Camping Map(Models.CampingAI_DB.T_CAMPINGS src)
    {
        return new Domain.Entities.Camping(
            src.CMP_IdCamping,
            src.CMP_Name,
            src.CMP_Description,
            src.CMP_Latitude,
            src.CMP_Longitude,
            src.CMP_PricePerNight,
            src.CMP_OwnerId,
            src.CMP_CategoryId,
            src.CMP_ProvinciaId,
            src.CMP_CreatedOn,
            src.CMP_UpdatedOn,
            src.CMP_DeletedOn);
    }

    public override Models.CampingAI_DB.T_CAMPINGS ReverseMap(Domain.Entities.Camping src)
    {
        return new Models.CampingAI_DB.T_CAMPINGS
        {
            CMP_IdCamping    = src.Id,
            CMP_Name         = src.Name,
            CMP_Description  = src.Description,
            CMP_Latitude     = src.Latitude.Value,
            CMP_Longitude    = src.Longitude.Value,
            CMP_PricePerNight = src.PricePerNight.Value,
            CMP_OwnerId      = src.OwnerId,
            CMP_CategoryId   = src.CategoryId,
            CMP_ProvinciaId  = src.ProvinciaId,
            CMP_CreatedOn    = src.CreatedOn.Value,
            CMP_UpdatedOn    = src.UpdatedOn.Value,
            CMP_DeletedOn    = src.DeletedOn
        };
    }
}
