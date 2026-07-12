namespace CampingAI.DataImporter;

public class Worker : BackgroundService {
    #region Dependencies
    readonly Importers.Interfaces.ICampingsImporter _importer;
    readonly IHostApplicationLifetime _lifetime;
    readonly ILogger<Worker> _logger;
    #endregion

    public Worker(Importers.Interfaces.ICampingsImporter importer,
                  IHostApplicationLifetime lifetime,
                  ILogger<Worker> logger) {
        _importer = importer;
        _lifetime = lifetime;
        _logger   = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        try {
            await _importer.RunAsync(stoppingToken);
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, "Error fatal durante la importación. El proceso se detendrá.");
        }
        finally {
            _lifetime.StopApplication();
        }
    }
}
