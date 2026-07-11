namespace CampingAI.Infra.Favorites.Mappers;

public class FavoritesMapper : Domain.Abstractions.Mappers.SimpleMapper<Models.CampingAI_DB.T_FAVORITES, Domain.Entities.Favorite>
{
    public override Domain.Entities.Favorite Map(Models.CampingAI_DB.T_FAVORITES src)
    {
        return new Domain.Entities.Favorite(
            src.FAV_IdFavorite,
            src.FAV_UserId,
            src.FAV_CampingId,
            src.FAV_CreatedAt);
    }
}
