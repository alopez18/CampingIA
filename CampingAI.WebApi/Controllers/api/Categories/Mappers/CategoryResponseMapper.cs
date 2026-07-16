namespace CampingAI.WebApi.Controllers.api.Categories.Mappers;
public class CategoryResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Category, DTO.CategoryResponse> {
    public override DTO.CategoryResponse Map(Domain.Entities.Category src) =>
        new(src.Id, src.Name.ToString());
}
