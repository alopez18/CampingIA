using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CampingAI.DataImporter.Services;

public class FacilitySeederService : Interfaces.IFacilitySeederService {
    #region Dependencies
    readonly string _connectionString;
    readonly ILogger<FacilitySeederService> _logger;
    #endregion

    // Número de facilities aleatorias a asignar por camping.
    private const int MinFacilities = 4;
    private const int MaxFacilities = 9;

    public FacilitySeederService(IConfiguration config,
                                 ILogger<FacilitySeederService> logger) {
        _connectionString = config.GetConnectionString("CAMPING_AI_SqlServer")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'CAMPING_AI_SqlServer'.");
        _logger = logger;
    }

    public async Task<int> SeedAsync(IReadOnlyList<Guid> campingIds, CancellationToken ct = default) {
        if (campingIds.Count == 0) return 0;

        int assigned = 0;

        try {
            using var connection = new SqlConnection(_connectionString);

            var facilityIds = (await connection.QueryAsync<Guid>(
                "SELECT FAC_IdFacility FROM T_FACILITIES")).ToList();

            if (facilityIds.Count == 0) {
                _logger.LogWarning("El catálogo T_FACILITIES está vacío. No se asignarán instalaciones.");
                return 0;
            }

            foreach (var campingId in campingIds) {
                ct.ThrowIfCancellationRequested();

                var selected = PickFacilities(campingId, facilityIds);

                var existing = (await connection.QueryAsync<Guid>(
                    "SELECT CFE_FacilityId FROM T_CAMPING_FACILITIES WHERE CFE_CampingId = @CampingId",
                    new { CampingId = campingId })).ToHashSet();

                foreach (var facilityId in selected) {
                    if (existing.Contains(facilityId)) continue;

                    await connection.ExecuteAsync(
                        "INSERT INTO T_CAMPING_FACILITIES (CFE_IdCampingFacility, CFE_CampingId, CFE_FacilityId) " +
                        "VALUES (@Id, @CampingId, @FacilityId)",
                        new { Id = Guid.NewGuid(), CampingId = campingId, FacilityId = facilityId });

                    assigned++;
                }
            }

            _logger.LogInformation("Sembrado de facilities completado — nuevas asignaciones: {Assigned}.", assigned);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error durante el sembrado de facilities.");
            throw;
        }

        return assigned;
    }

    private static List<Guid> PickFacilities(Guid campingId, List<Guid> facilityIds) {
        // Semilla determinista por camping: re-ejecuciones = mismo subconjunto.
        var seed = BitConverter.ToInt32(campingId.ToByteArray(), 0);
        var rnd  = new Random(seed);

        var count = rnd.Next(MinFacilities, Math.Min(MaxFacilities, facilityIds.Count) + 1);

        return facilityIds
            .OrderBy(_ => rnd.Next())
            .Take(count)
            .ToList();
    }
}
