namespace CampingAI.DataImporter.Importers;

public class ImportResult {
    public string SourceName { get; init; } = string.Empty;
    public int    Inserted   { get; set; }
    public int    Updated    { get; set; }
    public int    Skipped    { get; set; }
    public int    Failed     { get; set; }
    public bool   Succeeded  { get; set; } = true;
    public string? Error     { get; set; }

    public int Total => Inserted + Updated + Skipped + Failed;

    public static ImportResult ForSource(string sourceName) => new() { SourceName = sourceName };

    public ImportResult Merge(ImportResult other) {
        Inserted += other.Inserted;
        Updated  += other.Updated;
        Skipped  += other.Skipped;
        Failed   += other.Failed;
        Succeeded = Succeeded && other.Succeeded;
        return this;
    }
}
