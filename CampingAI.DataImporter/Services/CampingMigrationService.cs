using Dapper;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace CampingAI.DataImporter.Services;

public class CampingMigrationService : Interfaces.ICampingMigrationService
{
    #region Dependencies
    readonly string _connectionString;
    readonly ILogger<CampingMigrationService> _logger;
    readonly Interfaces.IProvinceGeoResolver _provinceGeoResolver;
    readonly Clients.Interfaces.INominatimClient _nominatimClient;
    #endregion

    // Usuario Sistema con GUID fijo y controlable (ver 9.SeedSystemUser.sql).
    static readonly Guid SystemUserId = new("10000000-0000-0000-0000-000000000001");

    // Rango de precio aleatorio por noche.
    private const decimal MinPrice = 25m;
    private const decimal MaxPrice = 180m;

    public CampingMigrationService(IConfiguration config,
                                   ILogger<CampingMigrationService> logger,
                                   Interfaces.IProvinceGeoResolver provinceGeoResolver,
                                   Clients.Interfaces.INominatimClient nominatimClient)
    {
        _connectionString = config.GetConnectionString("CAMPING_AI_SqlServer")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'CAMPING_AI_SqlServer'.");
        _logger = logger;
        _provinceGeoResolver = provinceGeoResolver;
        _nominatimClient = nominatimClient;
    }

    public async Task<(int inserted, int updated, int skipped, IReadOnlyList<Guid> campingIds)> MigrateAsync(CancellationToken ct = default)
    {
        int inserted = 0, updated = 0, skipped = 0;
        var campingIds = new List<Guid>();

        try
        {
            using var connection = new SqlConnection(_connectionString);

            var provinces = await LoadProvincesAsync(connection);
            var sources = await LoadImportsAsync(connection);

            _logger.LogInformation("Registros a migrar desde T_CAMPINGS_IMPORT: {Count}.", sources.Count);

            foreach (var src in sources)
            {
                ct.ThrowIfCancellationRequested();

                if (!src.CMI_Latitude.HasValue || !src.CMI_Longitude.HasValue)
                {
                    skipped++;
                    _logger.LogWarning("Camping '{Name}' ({ExternalId}) omitido: sin coordenadas.",
                        src.CMI_Name, src.CMI_ExternalId);
                    continue;
                }

                var provinciaId = await ResolveProvinceIdAsync(
                    src.CMI_Latitude.Value, src.CMI_Longitude.Value, src.CMI_Province, provinces, ct);

                var model = new Models.T_CAMPINGS
                {
                    CMP_IdCamping = src.CMI_IdCamping,
                    CMP_Name = Truncate(src.CMI_Name, 255),
                    CMP_Description = BuildDescription(src),
                    CMP_Latitude = src.CMI_Latitude.Value,
                    CMP_Longitude = src.CMI_Longitude.Value,
                    CMP_PricePerNight = BuildPrice(src.CMI_IdCamping),
                    CMP_OwnerId = SystemUserId,
                    CMP_CategoryId = CategoryInferrer.InferAll(src.CMI_IdCamping, src.CMI_Name, src.CMI_City, src.CMI_Province, src.CMI_Address)[0],
                    CMP_ProvinciaId = provinciaId,
                    CMP_UpdatedOn = DateTime.UtcNow
                };

                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM T_CAMPINGS WHERE CMP_IdCamping = @Id",
                    new { Id = model.CMP_IdCamping });

                if (exists > 0)
                {
                    await UpdateAsync(connection, model);
                    updated++;
                }
                else
                {
                    model.CMP_CreatedOn = DateTime.UtcNow;
                    await InsertAsync(connection, model);
                    inserted++;
                }

                campingIds.Add(model.CMP_IdCamping);
            }

            _logger.LogInformation("Migración completada — insertados: {Ins}, actualizados: {Upd}, omitidos: {Skip}.",
                inserted, updated, skipped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la migración de T_CAMPINGS_IMPORT a T_CAMPINGS.");
            throw;
        }

        return (inserted, updated, skipped, campingIds);
    }

    // ── Carga de datos ──────────────────────────────────────────────────────

    private static async Task<List<Models.T_PROVINCES>> LoadProvincesAsync(SqlConnection connection)
    {
        var rows = await connection.QueryAsync<Models.T_PROVINCES>(
            "SELECT PRV_IdProvince, PRV_Code, PRV_Name, PRV_CountryId FROM T_PROVINCES");
        return rows.ToList();
    }

    private static async Task<List<Models.T_CAMPINGS_IMPORT>> LoadImportsAsync(SqlConnection connection)
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT CMI_IdCamping, CMI_ExternalId, CMI_Source, CMI_Name,");
        sql.AppendLine("       CMI_Latitude, CMI_Longitude, CMI_Address, CMI_PostalCode,");
        sql.AppendLine("       CMI_City, CMI_Province, CMI_Country, CMI_Phone, CMI_Email,");
        sql.AppendLine("       CMI_Website, CMI_CreatedOn, CMI_UpdatedOn");
        sql.AppendLine("FROM   T_CAMPINGS_IMPORT");

        var rows = await connection.QueryAsync<Models.T_CAMPINGS_IMPORT>(sql.ToString());
        return rows.ToList();
    }

    // ── Persistencia ────────────────────────────────────────────────────────

    private static async Task InsertAsync(SqlConnection connection, Models.T_CAMPINGS m)
    {
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO T_CAMPINGS (");
        sql.AppendLine("    CMP_IdCamping, CMP_Name, CMP_Description, CMP_Latitude, CMP_Longitude,");
        sql.AppendLine("    CMP_PricePerNight, CMP_OwnerId, CMP_CategoryId, CMP_ProvinciaId,");
        sql.AppendLine("    CMP_CreatedOn, CMP_UpdatedOn");
        sql.AppendLine(") VALUES (");
        sql.AppendLine("    @CMP_IdCamping, @CMP_Name, @CMP_Description, @CMP_Latitude, @CMP_Longitude,");
        sql.AppendLine("    @CMP_PricePerNight, @CMP_OwnerId, @CMP_CategoryId, @CMP_ProvinciaId,");
        sql.AppendLine("    @CMP_CreatedOn, @CMP_UpdatedOn");
        sql.AppendLine(")");

        try
        {
            await connection.ExecuteAsync(sql.ToString(), m);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private static async Task UpdateAsync(SqlConnection connection, Models.T_CAMPINGS m)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE T_CAMPINGS SET");
        sql.AppendLine("    CMP_Name          = @CMP_Name,");
        sql.AppendLine("    CMP_Description    = @CMP_Description,");
        sql.AppendLine("    CMP_Latitude       = @CMP_Latitude,");
        sql.AppendLine("    CMP_Longitude      = @CMP_Longitude,");
        sql.AppendLine("    CMP_PricePerNight  = @CMP_PricePerNight,");
        sql.AppendLine("    CMP_OwnerId        = @CMP_OwnerId,");
        sql.AppendLine("    CMP_CategoryId     = @CMP_CategoryId,");
        sql.AppendLine("    CMP_ProvinciaId    = @CMP_ProvinciaId,");
        sql.AppendLine("    CMP_UpdatedOn      = @CMP_UpdatedOn");
        sql.AppendLine("WHERE CMP_IdCamping    = @CMP_IdCamping");

        await connection.ExecuteAsync(sql.ToString(), m);
    }

    // ── Lógica de derivación ────────────────────────────────────────────────

    private async Task<Guid?> ResolveProvinceIdAsync(
        decimal latitude, decimal longitude, string? provinceName,
        List<Models.T_PROVINCES> provinces, CancellationToken ct)
    {
        // 1) Derivación geográfica offline (point-in-polygon sobre GeoJSON embebido).
        var provinceCode = _provinceGeoResolver.ResolveProvinceCode((double)latitude, (double)longitude);
        if (!string.IsNullOrEmpty(provinceCode))
        {
            var geoMatch = provinces.FirstOrDefault(p =>
                string.Equals(p.PRV_Code, provinceCode, StringComparison.OrdinalIgnoreCase));
            if (geoMatch is not null)
                return geoMatch.PRV_IdProvince;

            _logger.LogWarning("Código de provincia '{Code}' derivado de coordenadas ({Lat}, {Lon}) no existe en T_PROVINCES.",
                provinceCode, latitude, longitude);
        }

        // 2) Fallback online: geocodificación inversa con Nominatim (OpenStreetMap).
        //    Nominatim devuelve state_district = provincia española; state = comunidad autónoma
        //    (útil para comunidades uniprovinciales). Respeta el límite de 1 req/s.
        await Task.Delay(1_000, ct);
        var nominatim = await _nominatimClient.ReverseGeocodeAsync((double)latitude, (double)longitude, ct);
        if (nominatim?.Address is { } addr)
        {
            // Candidatos en orden de prioridad para España.
            var candidates = new[] { addr.StateDistrict, addr.State, addr.County };
            foreach (var candidate in candidates)
            {
                if (string.IsNullOrWhiteSpace(candidate)) continue;
                var normalized = Normalize(candidate);
                var nominatimMatch = provinces.FirstOrDefault(p => Normalize(p.PRV_Name) == normalized);
                if (nominatimMatch is not null)
                {
                    _logger.LogDebug(
                        "Provincia resuelta por Nominatim '{Field}' para coordenadas ({Lat}, {Lon}).",
                        candidate, latitude, longitude);
                    return nominatimMatch.PRV_IdProvince;
                }
            }
            _logger.LogWarning(
                "Nominatim respondió (state_district='{SD}', state='{St}') pero no coincide con ninguna provincia conocida.",
                addr.StateDistrict, addr.State);
        }

        // 3) Último recurso: match por nombre del campo CMI_Province.
        if (!string.IsNullOrWhiteSpace(provinceName))
        {
            var normalized = Normalize(provinceName);
            var nameMatch = provinces.FirstOrDefault(p => Normalize(p.PRV_Name) == normalized);
            if (nameMatch is not null)
            {
                _logger.LogDebug("Provincia resuelta por nombre CMI_Province '{Name}' para coordenadas ({Lat}, {Lon}).",
                    provinceName, latitude, longitude);
                return nameMatch.PRV_IdProvince;
            }
        }

        _logger.LogWarning("No se pudo resolver la provincia para coordenadas ({Lat}, {Lon}) ni por nombre '{Name}'.",
            latitude, longitude, provinceName);
        return null;
    }

    private static string BuildDescription(Models.T_CAMPINGS_IMPORT src)
    {
        var location = new List<string>();
        if (!string.IsNullOrWhiteSpace(src.CMI_City)) location.Add(src.CMI_City!.Trim());
        if (!string.IsNullOrWhiteSpace(src.CMI_Province)) location.Add(src.CMI_Province!.Trim());
        if (!string.IsNullOrWhiteSpace(src.CMI_Country)) location.Add(src.CMI_Country!.Trim());

        var sb = new StringBuilder();
        sb.Append(src.CMI_Name.Trim());

        if (location.Count > 0)
            sb.Append(" es un camping situado en ").Append(string.Join(", ", location));
        else
            sb.Append(" es un camping");

        if (!string.IsNullOrWhiteSpace(src.CMI_Address))
            sb.Append(" (").Append(src.CMI_Address!.Trim()).Append(')');

        sb.Append(". Un espacio ideal para disfrutar de la naturaleza y el aire libre en un entorno tranquilo y bien comunicado.");

        return Truncate(sb.ToString(), 2000);
    }

    private static decimal BuildPrice(Guid campingId)
    {
        // Semilla determinista a partir del GUID: mismas re-ejecuciones = mismo precio.
        var seed = BitConverter.ToInt32(campingId.ToByteArray(), 0);
        var rnd = new Random(seed);
        var value = MinPrice + (decimal)rnd.NextDouble() * (MaxPrice - MinPrice);
        return Math.Round(value, 2);
    }

    // ── Utilidades ────────────────────────────────────────────────────────────

    private static string Normalize(string value) => CategoryInferrer.Normalize(value);

    private static string Truncate(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..maxLength];
}
