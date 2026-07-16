using System.Text.Json.Serialization;

namespace CampingAI.AI.DTOs;
/// <summary>
/// Respuesta de la IA para la comparación de campings.
/// </summary>
public class CompareResponse {
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("bestForBudget")]
    public Guid? BestForBudget { get; set; }

    [JsonPropertyName("bestOverall")]
    public Guid? BestOverall { get; set; }
}
