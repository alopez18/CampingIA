using System.Text.Json.Serialization;

namespace CampingAI.DataImporter.DTOs;

public class OverpassTags {
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("addr:street")]
    public string? Street { get; set; }

    [JsonPropertyName("addr:housenumber")]
    public string? HouseNumber { get; set; }

    [JsonPropertyName("addr:postcode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("addr:city")]
    public string? City { get; set; }

    [JsonPropertyName("addr:province")]
    public string? Province { get; set; }

    [JsonPropertyName("addr:country")]
    public string? Country { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("contact:phone")]
    public string? ContactPhone { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("contact:email")]
    public string? ContactEmail { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("contact:website")]
    public string? ContactWebsite { get; set; }

    /// <summary>Teléfono resuelto priorizando el tag directo sobre el de contacto.</summary>
    public string? ResolvedPhone => Phone ?? ContactPhone;

    /// <summary>Email resuelto priorizando el tag directo sobre el de contacto.</summary>
    public string? ResolvedEmail => Email ?? ContactEmail;

    /// <summary>Website resuelto priorizando el tag directo sobre el de contacto.</summary>
    public string? ResolvedWebsite => Website ?? ContactWebsite;

    /// <summary>Dirección construida a partir de calle + número de portal.</summary>
    public string? ResolvedAddress =>
        (Street, HouseNumber) switch {
            (not null, not null) => $"{Street}, {HouseNumber}",
            (not null, null)     => Street,
            _                    => null
        };
}
