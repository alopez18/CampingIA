namespace CampingAI.DataImporter.DTOs.Normalized;

/// <summary>
/// Representación normalizada de un camping, común a todas las fuentes
/// (OpenStreetMap y portales Open Data). Coordenadas siempre en WGS84.
/// </summary>
public class CampingNormalizedDto {
    public string  ExternalId     { get; set; } = string.Empty;
    public string  Source         { get; set; } = string.Empty;
    public string? SourceRecordId { get; set; }
    public string  Name           { get; set; } = string.Empty;
    public string? Description     { get; set; }
    public string? Category        { get; set; }
    public string? Address         { get; set; }
    public string? PostalCode      { get; set; }
    public string? City            { get; set; }
    public string? Province        { get; set; }
    public string? Region          { get; set; }
    public string? Country         { get; set; }
    public decimal? Latitude       { get; set; }
    public decimal? Longitude      { get; set; }
    public string? Phone           { get; set; }
    public string? Email           { get; set; }
    public string? Website         { get; set; }
}
