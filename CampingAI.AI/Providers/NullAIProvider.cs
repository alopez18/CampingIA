using Microsoft.Extensions.Logging;

namespace CampingAI.AI.Providers;
/// <summary>
/// Proveedor de IA de reserva (fallback) que se usa cuando no hay una API Key configurada.
/// Permite que la solución compile y arranque, y que los tests no realicen llamadas de red.
/// </summary>
public class NullAIProvider : IAIProvider {

    readonly ILogger<NullAIProvider> _logger;

    public NullAIProvider(ILogger<NullAIProvider> logger) {
        _logger = logger;
    }

    public bool IsEnabled => false;

    public Task<string> GenerateTextAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default) {
        _logger.LogWarning("Se solicitó una operación de IA pero no hay proveedor configurado (falta AISettings:ApiKey).");
        throw new InvalidOperationException("El asistente de IA no está configurado. Configure 'AISettings:ApiKey' para habilitarlo.");
    }

    public Task<string> GenerateJsonAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default) {
        return GenerateTextAsync(systemPrompt, userPrompt, cancellationToken);
    }
}
