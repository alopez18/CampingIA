namespace CampingAI.AI.Settings;

public class AISettings
{
    public const string SECTION = "AISettings";

    public string Provider { get; set; } = "Gemini";
    public string Model { get; set; } = "gemini-3.1-flash-lite";
    public string? ApiKey { get; set; }

    /// <summary>
    /// Indica si se debe cachear la respuesta de las llamadas al proveedor de IA
    /// para evitar invocar la API cuando la misma entrada ya fue procesada.
    /// </summary>
    public bool CacheEnabled { get; set; } = true;

    /// <summary>
    /// Tiempo de vida (en minutos) de cada entrada en la caché del proveedor de IA.
    /// </summary>
    public int CacheMinutes { get; set; } = 60;
}
