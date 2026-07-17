using System.Text.Json.Serialization;

namespace CampingAI.DataImporter.DTOs;

// Subconjunto de la respuesta de Nominatim /reverse que necesitamos.
// Docs: https://nominatim.org/release-docs/develop/api/Reverse/
public sealed class NominatimReverseResponse {
    [JsonPropertyName("address")]
    public NominatimAddress? Address { get; init; }
}

public sealed class NominatimAddress {
    // En España: comarca (p. ej. "Barcelonès") — demasiado granular para provincia.
    [JsonPropertyName("county")]
    public string? County { get; init; }

    // En España: provincia (p. ej. "Barcelona", "Girona", "Madrid") — campo clave.
    [JsonPropertyName("state_district")]
    public string? StateDistrict { get; init; }

    // En España: comunidad autónoma (p. ej. "Cataluña", "Comunidad de Madrid").
    // Útil para comunidades uniprovinciales (Asturias, Cantabria, La Rioja…).
    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; }
}
