namespace CampingAI.DataImporter.Importers.Interfaces;

public interface ICampingsImporter {
    Task RunAsync(CancellationToken ct = default);
}
