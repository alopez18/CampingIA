using Microsoft.Extensions.Logging;

namespace CampingAI.DataImporter.Orchestration;

public class ImportOrchestrator : Interfaces.IImportOrchestrator {
    #region Dependencies
    readonly IEnumerable<Importers.Interfaces.IDataSourceImporter> _importers;
    readonly ILogger<ImportOrchestrator> _logger;
    #endregion

    public ImportOrchestrator(IEnumerable<Importers.Interfaces.IDataSourceImporter> importers,
                              ILogger<ImportOrchestrator> logger) {
        _importers = importers;
        _logger    = logger;
    }

    public async Task<Importers.ImportResult> RunAllAsync(CancellationToken ct = default) {
        var enabled = _importers
            .Where(i => i.Enabled)
            .OrderByDescending(i => i.Priority)
            .ToList();

        _logger.LogInformation("Fuentes de datos habilitadas: {Count} de {Total}.",
            enabled.Count, _importers.Count());

        var aggregate = Importers.ImportResult.ForSource("ALL");

        foreach (var importer in enabled) {
            if (ct.IsCancellationRequested) break;

            _logger.LogInformation("▶ Ejecutando fuente '{Source}' (prioridad {Priority}).",
                importer.SourceName, importer.Priority);

            try {
                var result = await importer.RunAsync(ct);
                aggregate.Merge(result);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested) {
                aggregate.Failed++;
                aggregate.Succeeded = false;
                _logger.LogError(ex, "Fuente '{Source}' falló. Continuando con la siguiente.", importer.SourceName);
            }
        }

        _logger.LogInformation("Orquestación finalizada — insertados: {Ins}, actualizados: {Upd}, fallidos: {Fail}.",
            aggregate.Inserted, aggregate.Updated, aggregate.Failed);

        return aggregate;
    }
}
