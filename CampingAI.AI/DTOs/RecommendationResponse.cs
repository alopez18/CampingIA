using System.Text.Json.Serialization;

namespace CampingAI.AI.DTOs;
/// <summary>
/// Respuesta de la IA para recomendaciones personalizadas.
/// </summary>
public class RecommendationResponse {
    [JsonPropertyName("recommendedCampingIds")]
    public List<Guid> RecommendedCampingIds { get; set; } = [];

    [JsonPropertyName("reasoning")]
    public string Reasoning { get; set; } = string.Empty;
}
