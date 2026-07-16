namespace CampingAI.DataImporter.Orchestration.Interfaces;

public interface IImportOrchestrator {
    Task<Importers.ImportResult> RunAllAsync(CancellationToken ct = default);
}
