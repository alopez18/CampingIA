using System.Text.Json.Serialization;

namespace CampingAI.DataImporter.Models.Geo;

// Modelos mínimos para deserializar un FeatureCollection GeoJSON de límites
// provinciales (geometrías Polygon y MultiPolygon). Las coordenadas siguen el
// orden GeoJSON [longitud, latitud].

public class GeoJsonFeatureCollection {
    [JsonPropertyName("features")]
    public List<GeoJsonFeature> Features { get; set; } = new();
}

public class GeoJsonFeature {
    [JsonPropertyName("properties")]
    public GeoJsonProperties Properties { get; set; } = new();

    [JsonPropertyName("geometry")]
    public GeoJsonGeometry Geometry { get; set; } = new();
}

public class GeoJsonProperties {
    // Código INE de provincia de 2 dígitos (p. ej. "08" = Barcelona).
    [JsonPropertyName("cod_prov")]
    public string? CodProv { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class GeoJsonGeometry {
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    // Se deserializa como JsonElement para soportar tanto Polygon como
    // MultiPolygon sin acoplarnos a una forma concreta del array.
    [JsonPropertyName("coordinates")]
    public System.Text.Json.JsonElement Coordinates { get; set; }
}
