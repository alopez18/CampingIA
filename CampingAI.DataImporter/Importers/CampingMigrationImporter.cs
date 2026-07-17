using Microsoft.Extensions.Logging;

namespace CampingAI.DataImporter.Importers;

public class CampingMigrationImporter : Interfaces.ICampingMigrationImporter {
    #region Dependencies
    readonly Services.Interfaces.ICampingMigrationService _migrationService;
    readonly Services.Interfaces.IFacilitySeederService _facilitySeeder;
    readonly Services.Interfaces.ICategorySeederService _categorySeeder;
    readonly ILogger<CampingMigrationImporter> _logger;
    #endregion

    public CampingMigrationImporter(Services.Interfaces.ICampingMigrationService migrationService,
                                    Services.Interfaces.IFacilitySeederService facilitySeeder,
                                    Services.Interfaces.ICategorySeederService categorySeeder,
                                    ILogger<CampingMigrationImporter> logger) {
        _migrationService = migrationService;
        _facilitySeeder   = facilitySeeder;
        _categorySeeder   = categorySeeder;
        _logger           = logger;
    }

    // Se ejecuta después de la importación desde OpenStreetMap (prioridad 100).
    public string SourceName => "CampingsMigration";
    public int    Priority   => 50;
    public bool   Enabled    => true;

    public async Task<ImportResult> RunAsync(CancellationToken ct = default) {
        _logger.LogInformation("──── Migración T_CAMPINGS_IMPORT → T_CAMPINGS ────");

        var result = ImportResult.ForSource(SourceName);

        var (inserted, updated, skipped, campingIds) = await _migrationService.MigrateAsync(ct);

        result.Inserted = inserted;
        result.Updated  = updated;
        result.Skipped  = skipped;

        var assigned = await _facilitySeeder.SeedAsync(campingIds, ct);
        var categoriesAssigned = await _categorySeeder.SeedAsync(campingIds, ct);

        _logger.LogInformation("Migración finalizada — insertados: {Ins}, actualizados: {Upd}, omitidos: {Skip}, facilities asignadas: {Fac}, categorías adicionales: {Cat}.",
            inserted, updated, skipped, assigned, categoriesAssigned);

        return result;
    }
}
