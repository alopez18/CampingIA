using System.Text.Json.Serialization;

namespace CampingAI.DataImporter.DTOs;

public class OverpassElement {
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>Coordenadas directas — disponibles solo en elementos tipo "node".</summary>
    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    [JsonPropertyName("lon")]
    public double? Lon { get; set; }

    /// <summary>Centroide calculado — disponible en "way" y "relation" con "out center".</summary>
    [JsonPropertyName("center")]
    public OverpassCenter? Center { get; set; }

    [JsonPropertyName("tags")]
    public OverpassTags? Tags { get; set; }

    /// <summary>Latitud resuelta independientemente del tipo de elemento OSM.</summary>
    public double? ResolvedLat => Lat ?? Center?.Lat;

    /// <summary>Longitud resuelta independientemente del tipo de elemento OSM.</summary>
    public double? ResolvedLon => Lon ?? Center?.Lon;
}
