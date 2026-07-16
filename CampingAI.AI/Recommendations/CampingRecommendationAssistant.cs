using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using CampingAI.AI.DTOs;
using CampingAI.AI.Prompts;
using CampingAI.AI.Providers;

namespace CampingAI.AI.Recommendations;
/// <summary>
/// Asistente de recomendaciones personalizadas basadas en el perfil del usuario.
/// </summary>
public class CampingRecommendationAssistant {

    #region Dependencias
    readonly IAIProvider _aiProvider;
    readonly RecommendationService _recommendationService;
    readonly ILogger<CampingRecommendationAssistant> _logger;
    #endregion

    static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public CampingRecommendationAssistant(IAIProvider aiProvider,
                                          RecommendationService recommendationService,
                                          ILogger<CampingRecommendationAssistant> logger) {
        _aiProvider = aiProvider;
        _recommendationService = recommendationService;
        _logger = logger;
    }

    public async Task<(RecommendationResponse Response, IReadOnlyList<Domain.Entities.Camping> Campings)> RecommendAsync(Guid userId, int maxResults = 5, CancellationToken cancellationToken = default) {
        var (preferences, candidates) = await _recommendationService.BuildProfileAsync(userId);

        if (candidates.Count == 0)
            return (new RecommendationResponse { Reasoning = "No hay campings candidatos disponibles." }, []);

        var candidatesCatalog = BuildCandidatesCatalog(candidates);
        var userPrompt = PromptTemplates.BuildRecommendationUser(preferences, candidatesCatalog, maxResults);
        var json = await _aiProvider.GenerateJsonAsync(PromptTemplates.RecommendationSystem, userPrompt, cancellationToken);

        var response = Deserialize(json);

        var validIds = candidates.Select(c => c.Id).ToHashSet();
        response.RecommendedCampingIds = response.RecommendedCampingIds.Where(validIds.Contains).Take(maxResults).ToList();

        var byId = candidates.ToDictionary(c => c.Id);
        var recommended = response.RecommendedCampingIds.Select(id => byId[id]).ToList();

        return (response, recommended);
    }

    RecommendationResponse Deserialize(string json) {
        try {
            return JsonSerializer.Deserialize<RecommendationResponse>(json, JsonOptions) ?? new RecommendationResponse();
        }
        catch (JsonException ex) {
            _logger.LogError(ex, "La IA devolvió un JSON de recomendaciones inválido: {Json}", json);
            throw new InvalidOperationException("El asistente de IA devolvió una respuesta no válida.", ex);
        }
    }

    static string BuildCandidatesCatalog(IEnumerable<Domain.Entities.Camping> campings) {
        var sb = new StringBuilder();
        foreach (var c in campings)
            sb.AppendLine($"{c.Id} = {c.Name} | {c.PricePerNight.Value}€/noche | {c.Description}");
        return sb.ToString();
    }
}
