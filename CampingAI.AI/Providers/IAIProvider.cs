namespace CampingAI.AI.Providers;
/// <summary>
/// Abstracción del proveedor de IA. Independiente del proveedor concreto
/// (Gemini, OpenAI, Azure OpenAI, Ollama, Groq...).
/// </summary>
public interface IAIProvider {
    bool IsEnabled { get; }

    /// <summary>
    /// Envía un prompt y devuelve texto libre.
    /// </summary>
    Task<string> GenerateTextAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía un prompt esperando una salida JSON. Devuelve el JSON como string
    /// (ya saneado de posibles vallas markdown).
    /// </summary>
    Task<string> GenerateJsonAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);
}
