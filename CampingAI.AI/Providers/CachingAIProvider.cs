using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CampingAI.AI.Settings;

namespace CampingAI.AI.Providers;

/// <summary>
/// Decorador de <see cref="IAIProvider"/> que cachea las respuestas del proveedor real.
/// Si la misma entrada (prompt de sistema + prompt de usuario) ya fue procesada,
/// devuelve el resultado cacheado sin volver a llamar a la API.
/// </summary>
public class CachingAIProvider : IAIProvider {

    #region Dependencias
    readonly IAIProvider _inner;
    readonly IMemoryCache _cache;
    readonly ILogger<CachingAIProvider> _logger;
    readonly AISettings _settings;
    #endregion

    public CachingAIProvider(IAIProvider inner, IMemoryCache cache, ILogger<CachingAIProvider> logger, AISettings settings) {
        _inner = inner;
        _cache = cache;
        _logger = logger;
        _settings = settings;
    }

    public bool IsEnabled => _inner.IsEnabled;

    public Task<string> GenerateTextAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default) {
        return GetOrCreateAsync("text", systemPrompt, userPrompt,
            () => _inner.GenerateTextAsync(systemPrompt, userPrompt, cancellationToken));
    }

    public Task<string> GenerateJsonAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default) {
        return GetOrCreateAsync("json", systemPrompt, userPrompt,
            () => _inner.GenerateJsonAsync(systemPrompt, userPrompt, cancellationToken));
    }

    async Task<string> GetOrCreateAsync(string kind, string systemPrompt, string userPrompt, Func<Task<string>> factory) {
        var key = BuildKey(kind, systemPrompt, userPrompt);

        if (_cache.TryGetValue(key, out string? cached) && !string.IsNullOrWhiteSpace(cached)) {
            _logger.LogDebug("Cache HIT del proveedor de IA ({Kind}). Clave: {Key}", kind, key);
            return cached!;
        }

        _logger.LogDebug("Cache MISS del proveedor de IA ({Kind}). Clave: {Key}", kind, key);
        var result = await factory();

        if (!string.IsNullOrWhiteSpace(result)) {
            _cache.Set(key, result, new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheMinutes)
            });
        }

        return result;
    }

    static string BuildKey(string kind, string systemPrompt, string userPrompt) {
        var raw = $"{kind}|{systemPrompt}|{userPrompt}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return $"ai:{kind}:{Convert.ToHexString(hash)}";
    }
}
