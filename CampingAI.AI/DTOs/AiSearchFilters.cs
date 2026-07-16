using System.Text.Json.Serialization;

namespace CampingAI.AI.DTOs;
/// <summary>
/// Filtros de búsqueda generados por la IA a partir de lenguaje natural.
/// Los GUIDs deben validarse contra el catálogo real antes de usarse.
/// </summary>
public class AiSearchFilters {
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("categoryIds")]
    public List<Guid> CategoryIds { get; set; } = [];

    [JsonPropertyName("facilityIds")]
    public List<Guid> FacilityIds { get; set; } = [];

    [JsonPropertyName("minPrice")]
    public decimal? MinPrice { get; set; }

    [JsonPropertyName("maxPrice")]
    public decimal? MaxPrice { get; set; }
}
