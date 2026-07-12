namespace CampingAI.Application.Queries.Favorite.GetFavorites;
public record GetFavoritesQuery(Guid UserId) : Abstractions.Query.IQuery<IEnumerable<Domain.Entities.Favorite>>;
