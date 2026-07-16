using CampingAI.AI.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.RegularExpressions;

namespace CampingAI.AI.Providers;
/// <summary>
/// Implementación real del proveedor de IA usando Google Gemini a través de Semantic Kernel.
/// </summary>
public class GeminiAIProvider : IAIProvider
{

    #region Dependencias
    readonly Kernel _kernel;
    readonly IChatCompletionService _chatCompletion;
    readonly ILogger<GeminiAIProvider> _logger;
    readonly AISettings _settings;
    #endregion

    public GeminiAIProvider(Kernel kernel, ILogger<GeminiAIProvider> logger, AISettings settings)
    {
        _kernel = kernel;
        _logger = logger;
        _settings = settings;
        _chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public bool IsEnabled => true;

    public Task<string> GenerateTextAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        return InvokeAsync(systemPrompt, userPrompt, cancellationToken);
    }

    public async Task<string> GenerateJsonAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        var raw = await InvokeAsync(systemPrompt, userPrompt, cancellationToken);
        return SanitizeJson(raw);
    }

    async Task<string> InvokeAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;
        var attempt = 0;

        while (true)
        {
            attempt++;
            try
            {
                var history = new ChatHistory();
                if (!string.IsNullOrWhiteSpace(systemPrompt))
                    history.AddSystemMessage(systemPrompt);
                history.AddUserMessage(userPrompt);

                var response = await _chatCompletion.GetChatMessageContentAsync(
                    history,
                    kernel: _kernel,
                    cancellationToken: cancellationToken);

                var content = response?.Content;
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning("Gemini devolvió una respuesta vacía (intento {Attempt}).", attempt);
                    throw new InvalidOperationException("El proveedor de IA devolvió una respuesta vacía.");
                }

                return content;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (attempt < maxAttempts && IsTransient(ex))
            {
                _logger.LogWarning(ex, "Error transitorio llamando a Gemini (intento {Attempt}/{Max}). Reintentando...", attempt, maxAttempts);
                await Task.Delay(TimeSpan.FromMilliseconds(400 * attempt), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error llamando al proveedor de IA (Gemini).");
                throw new InvalidOperationException("No se pudo completar la petición al asistente de IA. Inténtalo de nuevo más tarde.", ex);
            }
        }
    }

    static bool IsTransient(Exception ex)
    {
        var message = ex.Message?.ToLowerInvariant() ?? string.Empty;
        return message.Contains("timeout")
            || message.Contains("temporarily")
            || message.Contains("rate")
            || message.Contains("429")
            || message.Contains("503")
            || ex is HttpRequestException
            || ex is TaskCanceledException;
    }

    static string SanitizeJson(string raw)
    {
        var trimmed = raw.Trim();

        var fenceMatch = Regex.Match(trimmed, "```(?:json)?\\s*(.*?)```", RegexOptions.Singleline);
        if (fenceMatch.Success)
            trimmed = fenceMatch.Groups[1].Value.Trim();

        return trimmed;
    }
}


//"Your prepayment credits are depleted. Please go to AI Studio at https://ai.studio/projects to manage your project and billing. Learn more at https://ai.google.dev/gemini-api/docs/billing#prepay. "
//"This model models/gemini-2.0-flash-lite is no longer available. Please update your code to use a newer model for the latest features and improvements. We recommend you to use the Interactions API (https://ai.google.dev/gemini-api/docs/migrate-to-interactions)."