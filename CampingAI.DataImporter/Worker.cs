using Microsoft.Extensions.DependencyInjection;

namespace CampingAI.DataImporter;

public class Worker : BackgroundService {
    #region Dependencies
    readonly IServiceScopeFactory _scopeFactory;
    readonly IHostApplicationLifetime _lifetime;
    readonly ILogger<Worker> _logger;
    #endregion

    public Worker(IServiceScopeFactory scopeFactory,
                  IHostApplicationLifetime lifetime,
                  ILogger<Worker> logger) {
        _scopeFactory = scopeFactory;
        _lifetime     = lifetime;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        try {
            using var scope = _scopeFactory.CreateScope();
            var orchestrator = scope.ServiceProvider
                .GetRequiredService<Orchestration.Interfaces.IImportOrchestrator>();
            await orchestrator.RunAllAsync(stoppingToken);
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, "Error fatal durante la importación. El proceso se detendrá.");
        }
        finally {
            _lifetime.StopApplication();
        }
    }
}
