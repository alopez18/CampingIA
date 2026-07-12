using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CampingAI.DataImporter.Importers;

public class CampingsImporter : Interfaces.ICampingsImporter {
    #region Dependencies
    readonly Clients.Interfaces.IOverpassClient _overpassClient;
    readonly Services.Interfaces.ICampingImportService _importService;
    readonly Mappers.CampingImportMapper _mapper;
    readonly Configuration.AppSettings _settings;
    readonly ILogger<CampingsImporter> _logger;
    #endregion

    public CampingsImporter(Clients.Interfaces.IOverpassClient overpassClient,
                            Services.Interfaces.ICampingImportService importService,
                            Mappers.CampingImportMapper mapper,
                            IOptions<Configuration.AppSettings> settings,
                            ILogger<CampingsImporter> logger) {
        _overpassClient = overpassClient;
        _importService  = importService;
        _mapper         = mapper;
        _settings       = settings.Value;
        _logger         = logger;
    }

    public async Task RunAsync(CancellationToken ct = default) {
        _logger.LogInformation("═══════════════════════════════════════════");
        _logger.LogInformation("  CampingAI DataImporter — Inicio");
        _logger.LogInformation("  Regiones configuradas: {Count}", _settings.Regions.Length);
        _logger.LogInformation("═══════════════════════════════════════════");

        int totalInserted = 0, totalUpdated = 0;

        foreach (var region in _settings.Regions) {
            if (ct.IsCancellationRequested) break;

            _logger.LogInformation("──── Procesando región: {Region} ────", region);

            try {
                var response = await _overpassClient.QueryCampingsByRegionAsync(region, ct);

                var valid = response.Elements
                    .Where(e => !string.IsNullOrWhiteSpace(e.Tags?.Name) || e.ResolvedLat.HasValue)
                    .ToList();

                _logger.LogInformation("{Total} elementos totales → {Valid} válidos para '{Region}'.",
                    response.Elements.Count, valid.Count, region);

                if (valid.Count == 0) {
                    _logger.LogWarning("No se encontraron campings válidos para '{Region}'.", region);
                    continue;
                }

                var models = _mapper.Map(valid);
                var (ins, upd) = await _importService.UpsertAsync(models, ct);

                totalInserted += ins;
                totalUpdated  += upd;

                _logger.LogInformation("Región '{Region}' completada — insertados: {Ins}, actualizados: {Upd}.",
                    region, ins, upd);

                // Pausa cortés entre regiones para no sobrecargar la API pública de Overpass
                if (_settings.Regions.Last() != region)
                    await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested) {
                _logger.LogError(ex, "Error procesando la región '{Region}'. Continuando con la siguiente.", region);
            }
        }

        var total = await _importService.CountAsync(ct);

        _logger.LogInformation("═══════════════════════════════════════════");
        _logger.LogInformation("  Importación finalizada.");
        _logger.LogInformation("  Nuevos insertados : {Inserted}", totalInserted);
        _logger.LogInformation("  Actualizados      : {Updated}",  totalUpdated);
        _logger.LogInformation("  Total en BD       : {Total}",    total);
        _logger.LogInformation("═══════════════════════════════════════════");
    }
}
