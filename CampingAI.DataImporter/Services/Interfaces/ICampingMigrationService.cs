namespace CampingAI.DataImporter.Services.Interfaces;

public interface ICampingMigrationService {
    /// <summary>
    /// Traspasa los registros de T_CAMPINGS_IMPORT a T_CAMPINGS de forma
    /// idempotente (inserta los nuevos y actualiza los existentes) y devuelve
    /// los ids de los campings procesados para el sembrado de facilities.
    /// </summary>
    Task<(int inserted, int updated, int skipped, IReadOnlyList<Guid> campingIds)> MigrateAsync(CancellationToken ct = default);
}
