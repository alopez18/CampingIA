namespace CampingAI.DataImporter.Services.Interfaces;

public interface IFacilitySeederService {
    /// <summary>
    /// Asigna de forma idempotente un subconjunto aleatorio (pero determinista
    /// por camping) de facilities del catálogo a cada camping indicado.
    /// Solo inserta las combinaciones (camping, facility) que no existan aún.
    /// </summary>
    Task<int> SeedAsync(IReadOnlyList<Guid> campingIds, CancellationToken ct = default);
}
