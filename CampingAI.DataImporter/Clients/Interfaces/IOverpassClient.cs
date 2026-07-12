namespace CampingAI.DataImporter.Clients.Interfaces;

public interface IOverpassClient {
    Task<DTOs.OverpassResponse> QueryCampingsByRegionAsync(string regionName, CancellationToken ct = default);
}
