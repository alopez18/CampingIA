namespace CampingAI.DataImporter.Mappers;

public class CampingImportMapper {
    public Models.T_CAMPINGS_IMPORT Map(DTOs.OverpassElement element) {
        var tags = element.Tags;

        return new Models.T_CAMPINGS_IMPORT {
            CMI_IdCamping  = Guid.NewGuid(),
            CMI_ExternalId = $"{element.Type}/{element.Id}",
            CMI_Source     = "OpenStreetMap",
            CMI_Name       = tags?.Name ?? $"Camping OSM {element.Id}",
            CMI_Latitude   = element.ResolvedLat.HasValue ? (decimal)element.ResolvedLat.Value : null,
            CMI_Longitude  = element.ResolvedLon.HasValue ? (decimal)element.ResolvedLon.Value : null,
            CMI_Address    = tags?.ResolvedAddress,
            CMI_PostalCode = tags?.PostalCode,
            CMI_City       = tags?.City,
            CMI_Province   = tags?.Province,
            CMI_Country    = tags?.Country,
            CMI_Phone      = tags?.ResolvedPhone,
            CMI_Email      = tags?.ResolvedEmail,
            CMI_Website    = tags?.ResolvedWebsite,
            CMI_CreatedOn  = DateTime.UtcNow,
            CMI_UpdatedOn  = DateTime.UtcNow,
        };
    }

    public IEnumerable<Models.T_CAMPINGS_IMPORT> Map(IEnumerable<DTOs.OverpassElement> elements) =>
        elements.Select(Map);
}
