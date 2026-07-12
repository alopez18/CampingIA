namespace CampingAI.DataImporter.Configuration;

public class AppSettings {
    public const string SECTION = "DataImporter";

    public OverpassSettings Overpass { get; set; } = new();
    public string[]         Regions  { get; set; } = [];
}

public class OverpassSettings {
    public string BaseUrl        { get; set; } = "https://overpass-api.de/api/interpreter";
    public int    TimeoutSeconds { get; set; } = 120;
    public int    MaxRetries     { get; set; } = 3;
    public int    RetryDelayMs   { get; set; } = 5000;
}
