namespace CampingAI.Infra.Categories.Mappers;

public class CategoriesMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CampingAI_DB.T_CATEGORIES, Domain.Entities.Category>
{
    public override Domain.Entities.Category Map(Models.CampingAI_DB.T_CATEGORIES src)
    {
        return new Domain.Entities.Category(
            src.CAT_IdCategory,
            src.CAT_Name);
    }

    public override Models.CampingAI_DB.T_CATEGORIES ReverseMap(Domain.Entities.Category src)
    {
        return new Models.CampingAI_DB.T_CATEGORIES
        {
            CAT_IdCategory = src.Id,
            CAT_Name       = src.Name
        };
    }
}
