namespace CampingAI.DataImporter.Services.Interfaces;

public interface ICampingImportService {
    Task<(int inserted, int updated)> UpsertAsync(IEnumerable<Models.T_CAMPINGS_IMPORT> campings, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}
