using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CampingAI.DataImporter.Services;

public class CategorySeederService : Interfaces.ICategorySeederService {
    #region Dependencies
    readonly string _connectionString;
    readonly ILogger<CategorySeederService> _logger;
    #endregion

    public CategorySeederService(IConfiguration config,
                                 ILogger<CategorySeederService> logger) {
        _connectionString = config.GetConnectionString("CAMPING_AI_SqlServer")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'CAMPING_AI_SqlServer'.");
        _logger = logger;
    }

    public async Task<int> SeedAsync(IReadOnlyList<Guid> campingIds, CancellationToken ct = default) {
        if (campingIds.Count == 0) return 0;

        int assigned = 0;

        try {
            using var connection = new SqlConnection(_connectionString);

            // Carga los datos de importación necesarios para inferir categorías.
            var imports = (await connection.QueryAsync<Models.T_CAMPINGS_IMPORT>(
                "SELECT CMI_IdCamping, CMI_Name, CMI_City, CMI_Province, CMI_Address " +
                "FROM T_CAMPINGS_IMPORT WHERE CMI_IdCamping IN @Ids",
                new { Ids = campingIds })).ToDictionary(r => r.CMI_IdCamping);

            foreach (var campingId in campingIds) {
                ct.ThrowIfCancellationRequested();

                if (!imports.TryGetValue(campingId, out var src)) continue;

                var allCategories = CategoryInferrer.InferAll(
                    campingId, src.CMI_Name, src.CMI_City, src.CMI_Province, src.CMI_Address);

                // La categoría principal (índice 0) ya está en CMP_CategoryId; solo procesamos las adicionales.
                var secondaryCategories = allCategories.Skip(1).ToList();
                if (secondaryCategories.Count == 0) continue;

                // Carga las categorías adicionales ya existentes para este camping.
                var existing = (await connection.QueryAsync<Guid>(
                    "SELECT CCA_CategoryId FROM T_CAMPING_CATEGORIES WHERE CCA_CampingId = @CampingId",
                    new { CampingId = campingId })).ToHashSet();

                foreach (var categoryId in secondaryCategories) {
                    if (existing.Contains(categoryId)) continue;

                    await connection.ExecuteAsync(
                        "INSERT INTO T_CAMPING_CATEGORIES (CCA_IdCampingCategory, CCA_CampingId, CCA_CategoryId) " +
                        "VALUES (@Id, @CampingId, @CategoryId)",
                        new { Id = Guid.NewGuid(), CampingId = campingId, CategoryId = categoryId });

                    assigned++;
                }
            }

            _logger.LogInformation("Sembrado de categorías adicionales completado — nuevas asignaciones: {Assigned}.", assigned);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error durante el sembrado de categorías adicionales.");
            throw;
        }

        return assigned;
    }
}
