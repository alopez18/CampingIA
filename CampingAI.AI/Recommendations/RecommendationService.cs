using System.Text;
using Microsoft.Extensions.Logging;
using CampingAI.Application.Abstractions;
using CampingAI.Application.Queries.Camping.SearchCampings;
using CampingAI.Application.Queries.Favorite.GetFavorites;
using CampingAI.Application.Queries.Reservation.GetUserReservations;

namespace CampingAI.AI.Recommendations;
/// <summary>
/// Construye el perfil de preferencias del usuario y el conjunto de campings candidatos
/// a partir de favoritos, reservas y categorías preferidas.
/// </summary>
public class RecommendationService {

    #region Dependencias
    readonly IMediator _mediator;
    readonly ILogger<RecommendationService> _logger;
    #endregion

    public RecommendationService(IMediator mediator, ILogger<RecommendationService> logger) {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<(string Preferences, IReadOnlyList<Domain.Entities.Camping> Candidates)> BuildProfileAsync(Guid userId, int maxCandidates = 40) {
        var favorites = (await _mediator.SendQueryAsync<GetFavoritesQuery, IEnumerable<Domain.Entities.Favorite>>(new GetFavoritesQuery(userId))).ToList();
        var reservations = (await _mediator.SendQueryAsync<GetUserReservationsQuery, IEnumerable<Domain.Entities.Reservation>>(new GetUserReservationsQuery(userId))).ToList();

        var favoriteCampingIds = favorites.Select(f => f.CampingId).ToHashSet();
        var reservedCampingIds = reservations.Select(r => r.CampingId).ToHashSet();

        var search = await _mediator.SendQueryAsync<SearchCampingsQuery, SearchCampingsResult>(
            new SearchCampingsQuery(null, null, null, null, null, null, null, null, null, null, null, 1, maxCandidates));

        var allCampings = search.Items.ToList();

        var preferredCategoryIds = allCampings
            .Where(c => favoriteCampingIds.Contains(c.Id) || reservedCampingIds.Contains(c.Id))
            .Select(c => c.CategoryId)
            .Distinct()
            .ToHashSet();

        var candidates = allCampings
            .Where(c => !favoriteCampingIds.Contains(c.Id) && !reservedCampingIds.Contains(c.Id))
            .OrderByDescending(c => preferredCategoryIds.Contains(c.CategoryId))
            .Take(maxCandidates)
            .ToList();

        var preferences = BuildPreferencesText(favorites.Count, reservations.Count, preferredCategoryIds.Count);
        return (preferences, candidates);
    }

    static string BuildPreferencesText(int favoritesCount, int reservationsCount, int preferredCategories) {
        var sb = new StringBuilder();
        sb.AppendLine($"Favoritos guardados: {favoritesCount}");
        sb.AppendLine($"Reservas realizadas: {reservationsCount}");
        sb.AppendLine($"Categorías preferidas detectadas: {preferredCategories}");
        return sb.ToString();
    }
}
