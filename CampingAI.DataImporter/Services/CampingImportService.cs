using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.DataImporter.Services;

public class CampingImportService : Interfaces.ICampingImportService {
    #region Dependencies
    readonly string _connectionString;
    readonly ILogger<CampingImportService> _logger;
    #endregion

    private const string TABLE = "T_CAMPINGS_IMPORT";

    public CampingImportService(IConfiguration config,
                                ILogger<CampingImportService> logger) {
        _connectionString = config.GetConnectionString("CAMPING_AI_SqlServer")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'CAMPING_AI_SqlServer'.");
        _logger = logger;
    }

    public async Task<(int inserted, int updated)> UpsertAsync(IEnumerable<Models.T_CAMPINGS_IMPORT> campings,
                                                                CancellationToken ct = default) {
        int inserted = 0, updated = 0;

        try {
            using var connection = new SqlConnection(_connectionString);

            foreach (var camping in campings) {
                ct.ThrowIfCancellationRequested();

                var checkSql = new StringBuilder();
                checkSql.AppendLine($"SELECT {nameof(Models.T_CAMPINGS_IMPORT.CMI_IdCamping)}");
                checkSql.AppendLine($"FROM   {TABLE}");
                checkSql.AppendLine($"WHERE  {nameof(Models.T_CAMPINGS_IMPORT.CMI_ExternalId)} = @ExternalId");

                var existingId = await connection.QuerySingleOrDefaultAsync<Guid?>(
                    checkSql.ToString(),
                    new { ExternalId = camping.CMI_ExternalId });

                if (existingId.HasValue) {
                    await UpdateAsync(connection, camping, existingId.Value);
                    updated++;
                }
                else {
                    await InsertAsync(connection, camping);
                    inserted++;
                }
            }

            _logger.LogInformation("Upsert completado — insertados: {Inserted}, actualizados: {Updated}.", inserted, updated);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error durante el upsert en {Table}.", TABLE);
            throw;
        }

        return (inserted, updated);
    }

    public async Task<int> CountAsync(CancellationToken ct = default) {
        try {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {TABLE}");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al contar registros en {Table}.", TABLE);
            throw;
        }
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private static async Task InsertAsync(SqlConnection connection, Models.T_CAMPINGS_IMPORT m) {
        var sql = new StringBuilder();
        sql.AppendLine($"INSERT INTO {TABLE} (");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_IdCamping)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_ExternalId)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Source)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Name)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Latitude)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Longitude)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Address)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_PostalCode)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_City)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Province)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Country)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Phone)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Email)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Website)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_CreatedOn)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_UpdatedOn)}");
        sql.AppendLine(") VALUES (");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_IdCamping)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_ExternalId)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Source)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Name)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Latitude)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Longitude)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Address)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_PostalCode)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_City)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Province)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Country)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Phone)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Email)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Website)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_CreatedOn)},");
        sql.AppendLine($"    @{nameof(Models.T_CAMPINGS_IMPORT.CMI_UpdatedOn)}");
        sql.AppendLine(")");

        await connection.ExecuteAsync(sql.ToString(), m);
    }

    private static async Task UpdateAsync(SqlConnection connection, Models.T_CAMPINGS_IMPORT m, Guid existingId) {
        var sql = new StringBuilder();
        sql.AppendLine($"UPDATE {TABLE} SET");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Name)}       = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Name)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Latitude)}   = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Latitude)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Longitude)}  = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Longitude)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Address)}    = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Address)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_PostalCode)} = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_PostalCode)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_City)}       = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_City)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Province)}   = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Province)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Country)}    = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Country)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Phone)}      = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Phone)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Email)}      = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Email)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_Website)}    = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_Website)},");
        sql.AppendLine($"    {nameof(Models.T_CAMPINGS_IMPORT.CMI_UpdatedOn)}  = @{nameof(Models.T_CAMPINGS_IMPORT.CMI_UpdatedOn)}");
        sql.AppendLine($"WHERE {nameof(Models.T_CAMPINGS_IMPORT.CMI_IdCamping)} = @ExistingId");

        await connection.ExecuteAsync(sql.ToString(), new {
            m.CMI_Name, m.CMI_Latitude, m.CMI_Longitude, m.CMI_Address,
            m.CMI_PostalCode, m.CMI_City, m.CMI_Province, m.CMI_Country,
            m.CMI_Phone, m.CMI_Email, m.CMI_Website, m.CMI_UpdatedOn,
            ExistingId = existingId
        });
    }
}
