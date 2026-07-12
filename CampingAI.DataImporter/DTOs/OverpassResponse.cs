using System.Text.Json.Serialization;

namespace CampingAI.DataImporter.DTOs;

public class OverpassResponse {
    [JsonPropertyName("elements")]
    public List<OverpassElement> Elements { get; set; } = [];
}
