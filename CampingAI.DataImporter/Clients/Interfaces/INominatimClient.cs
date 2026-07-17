namespace CampingAI.DataImporter.Clients.Interfaces;

public interface INominatimClient {
    // Llama a Nominatim /reverse y devuelve el objeto address, o null si falla.
    Task<DTOs.NominatimReverseResponse?> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken ct = default);
}
