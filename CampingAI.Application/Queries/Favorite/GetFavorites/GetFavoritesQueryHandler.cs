namespace CampingAI.Application.Queries.Favorite.GetFavorites;
public class GetFavoritesQueryHandler : Abstractions.Query.IQueryHandler<GetFavoritesQuery, IEnumerable<Domain.Entities.Favorite>> {

    #region Dependencias
    readonly Domain.Repositories.IFavoritesReadRepository _favoritesReadRepository;
    #endregion

    public GetFavoritesQueryHandler(Domain.Repositories.IFavoritesReadRepository favoritesReadRepository) {
        _favoritesReadRepository = favoritesReadRepository;
    }

    public async Task<IEnumerable<Domain.Entities.Favorite>> HandleAsync(GetFavoritesQuery query) {
        return await _favoritesReadRepository.GetByUserIdAsync(query.UserId);
    }
}
