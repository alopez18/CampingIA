using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using CampingAI.AI.DTOs;
using CampingAI.AI.Prompts;
using CampingAI.AI.Providers;
using CampingAI.Application.Abstractions;
using CampingAI.Application.Queries.Camping.GetCampingById;

namespace CampingAI.AI.Comparisons;
/// <summary>
/// Asistente de comparación de campings a partir de sus datos reales.
/// </summary>
public class CampingComparisonAssistant {

    #region Dependencias
    readonly IAIProvider _aiProvider;
    readonly IMediator _mediator;
    readonly ILogger<CampingComparisonAssistant> _logger;
    #endregion

    static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public CampingComparisonAssistant(IAIProvider aiProvider,
                                      IMediator mediator,
                                      ILogger<CampingComparisonAssistant> logger) {
        _aiProvider = aiProvider;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<CompareResponse> CompareAsync(IEnumerable<Guid> campingIds, CancellationToken cancellationToken = default) {
        var ids = campingIds.Distinct().ToList();
        if (ids.Count < 2)
            throw new ArgumentException("Se necesitan al menos dos campings para comparar.", nameof(campingIds));

        var campings = new List<Domain.Entities.Camping>();
        foreach (var id in ids) {
            var camping = await _mediator.SendQueryAsync<GetCampingByIdQuery, Domain.Entities.Camping>(new GetCampingByIdQuery(id));
            if (camping is not null)
                campings.Add(camping);
        }

        if (campings.Count < 2)
            throw new ArgumentException("No se encontraron suficientes campings válidos para comparar.", nameof(campingIds));

        var data = BuildCampingsData(campings);
        var userPrompt = PromptTemplates.BuildComparisonUser(data);
        var json = await _aiProvider.GenerateJsonAsync(PromptTemplates.ComparisonSystem, userPrompt, cancellationToken);

        return Deserialize(json);
    }

    CompareResponse Deserialize(string json) {
        try {
            return JsonSerializer.Deserialize<CompareResponse>(json, JsonOptions) ?? new CompareResponse();
        }
        catch (JsonException ex) {
            _logger.LogError(ex, "La IA devolvió un JSON de comparación inválido: {Json}", json);
            throw new InvalidOperationException("El asistente de IA devolvió una respuesta no válida.", ex);
        }
    }

    static string BuildCampingsData(IEnumerable<Domain.Entities.Camping> campings) {
        var sb = new StringBuilder();
        foreach (var c in campings) {
            sb.AppendLine($"{c.Id}:");
            sb.AppendLine($"  Nombre: {c.Name}");
            sb.AppendLine($"  Precio/noche: {c.PricePerNight.Value}€");
            sb.AppendLine($"  Descripción: {c.Description}");
            sb.AppendLine($"  Servicios: {c.FacilityIds.Count}");
        }
        return sb.ToString();
    }
}
