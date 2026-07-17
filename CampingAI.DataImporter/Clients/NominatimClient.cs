using System.Text.Json;
using CampingAI.DataImporter.Clients.Interfaces;

namespace CampingAI.DataImporter.Clients;

// Cliente tipado para la API de geocodificación inversa de Nominatim (OpenStreetMap).
// Política de uso: máximo 1 petición/segundo; User-Agent identificable obligatorio.
// Docs: https://nominatim.org/release-docs/develop/api/Reverse/
public class NominatimClient : INominatimClient {
    #region Dependencies
    readonly HttpClient _httpClient;
    readonly ILogger<NominatimClient> _logger;
    #endregion

    private static readonly JsonSerializerOptions _jsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public NominatimClient(HttpClient httpClient, ILogger<NominatimClient> logger) {
        _httpClient = httpClient;
        _logger     = logger;
    }

    public async Task<DTOs.NominatimReverseResponse?> ReverseGeocodeAsync(
        double latitude, double longitude, CancellationToken ct = default) {
        var url = $"reverse?format=json&lat={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                  $"&lon={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                  "&addressdetails=1&accept-language=es&zoom=8";
        try {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode) {
                _logger.LogWarning("Nominatim respondió {Status} para ({Lat}, {Lon}).",
                    (int)response.StatusCode, latitude, longitude);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<DTOs.NominatimReverseResponse>(json, _jsonOptions);
        }
        catch (Exception ex) when (ex is not OperationCanceledException) {
            _logger.LogError(ex, "Error al llamar a Nominatim para ({Lat}, {Lon}).", latitude, longitude);
            return null;
        }
    }
}
