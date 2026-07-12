using System.Text.Json.Serialization;

namespace CampingAI.DataImporter.DTOs;

public class OverpassCenter {
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}
