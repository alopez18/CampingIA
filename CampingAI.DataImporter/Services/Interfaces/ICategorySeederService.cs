namespace CampingAI.DataImporter.Services.Interfaces;

public interface ICategorySeederService {
    /// <summary>
    /// Infiere de forma determinista las categorías adicionales de cada camping
    /// (a partir de su nombre, ciudad, provincia y dirección en T_CAMPINGS_IMPORT)
    /// e inserta en T_CAMPING_CATEGORIES las que aún no existan.
    /// La categoría principal (CMP_CategoryId) no se duplica aquí.
    /// </summary>
    Task<int> SeedAsync(IReadOnlyList<Guid> campingIds, CancellationToken ct = default);
}
