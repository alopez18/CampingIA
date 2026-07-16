namespace CampingAI.DataImporter.Importers.Interfaces;

/// <summary>
/// Contrato común para toda fuente de datos (OpenStreetMap/Overpass y portales
/// Open Data por Comunidad Autónoma). Permite orquestar todas las fuentes de
/// forma homogénea. Las fuentes de mayor <see cref="Priority"/> se ejecutan antes.
/// </summary>
public interface IDataSourceImporter {
    /// <summary>Identificador de la fuente, p. ej. "OpenStreetMap", "Catalonia".</summary>
    string SourceName { get; }

    /// <summary>Prioridad de ejecución (mayor valor = mayor prioridad).</summary>
    int Priority { get; }

    /// <summary>Indica si la fuente está habilitada para ejecutarse.</summary>
    bool Enabled { get; }

    Task<ImportResult> RunAsync(CancellationToken ct = default);
}
