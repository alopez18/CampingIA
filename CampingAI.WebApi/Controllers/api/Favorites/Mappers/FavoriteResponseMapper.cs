namespace CampingAI.WebApi.Controllers.api.Favorites.Mappers;
public class FavoriteResponseMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Favorite, DTO.FavoriteResponse> {
    public override DTO.FavoriteResponse Map(Domain.Entities.Favorite src) {
        return new DTO.FavoriteResponse(src.Id, src.CampingId, src.CreatedAt);
    }
}
