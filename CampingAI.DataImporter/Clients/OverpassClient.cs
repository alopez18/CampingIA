using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace CampingAI.DataImporter.Clients;

public class OverpassClient : Interfaces.IOverpassClient {
    #region Dependencies
    readonly HttpClient _httpClient;
    readonly Configuration.AppSettings _settings;
    readonly ILogger<OverpassClient> _logger;
    #endregion

    private static readonly JsonSerializerOptions _jsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public OverpassClient(HttpClient httpClient,
                          IOptions<Configuration.AppSettings> settings,
                          ILogger<OverpassClient> logger) {
        _httpClient = httpClient;
        _settings   = settings.Value;
        _logger     = logger;
    }

    public async Task<DTOs.OverpassResponse> QueryCampingsByRegionAsync(string regionName, CancellationToken ct = default) {
        var query = BuildQuery(regionName);
        _logger.LogInformation("Consultando Overpass API para la región: {Region}", regionName);

        var attempt = 0;
        while (true) {
            attempt++;
            try {
                var content  = new StringContent($"data={Uri.EscapeDataString(query)}", Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await _httpClient.PostAsync(string.Empty, content, ct);

                if (!response.IsSuccessStatusCode) {
                    var errorBody = await response.Content.ReadAsStringAsync(ct);
                    _logger.LogWarning("Overpass respondió {StatusCode} ({Reason}) para '{Region}'. Cuerpo: {Body}",
                        (int)response.StatusCode, response.ReasonPhrase, regionName, errorBody);
                    response.EnsureSuccessStatusCode();
                }

                var json   = await response.Content.ReadAsStringAsync(ct);
                var result = JsonSerializer.Deserialize<DTOs.OverpassResponse>(json, _jsonOptions)
                             ?? new DTOs.OverpassResponse();

                _logger.LogInformation("Overpass devolvió {Count} elementos para '{Region}'.", result.Elements.Count, regionName);
                return result;
            }
            catch (Exception ex) when (attempt <= _settings.Overpass.MaxRetries && !ct.IsCancellationRequested) {
                _logger.LogWarning(ex, "Intento {Attempt}/{Max} fallido para '{Region}'. Reintentando en {Delay} ms...",
                    attempt, _settings.Overpass.MaxRetries, _settings.Overpass.RetryDelayMs);
                await Task.Delay(_settings.Overpass.RetryDelayMs, ct);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error irrecuperable consultando Overpass API para la región '{Region}'.", regionName);
                throw;
            }
        }
    }

    private static string BuildQuery(string regionName) =>
        $"""
         [out:json][timeout:120];
         area["name"="{regionName}"]["boundary"="administrative"]->.searchArea;
         (
           node["tourism"="camp_site"](area.searchArea);
           way["tourism"="camp_site"](area.searchArea);
           relation["tourism"="camp_site"](area.searchArea);
         );
         out center tags;
         """;
}
