using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CampingAI.AI.Providers;
using CampingAI.AI.SemanticKernel;
using CampingAI.AI.Settings;

namespace CampingAI.AI.Configuration;
public static class DI_Manager {

    public static void Configure(IServiceCollection services, IConfiguration config) {
        var settings = config.GetSection(AISettings.SECTION).Get<AISettings>() ?? new AISettings();
        services.AddSingleton(settings);

        RegisterProvider(services, settings);
        RegisterAssistants(services);
    }

    private static void RegisterProvider(IServiceCollection services, AISettings settings) {
        if (!string.IsNullOrWhiteSpace(settings.ApiKey)) {
            services.AddSingleton(_ => KernelFactory.CreateGeminiKernel(settings.ApiKey!, settings.Model));

            if (settings.CacheEnabled) {
                services.AddMemoryCache();
                services.AddScoped<GeminiAIProvider>();
                services.AddScoped<IAIProvider>(sp => new CachingAIProvider(
                    sp.GetRequiredService<GeminiAIProvider>(),
                    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
                    sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CachingAIProvider>>(),
                    settings));
            }
            else {
                services.AddScoped<IAIProvider, GeminiAIProvider>();
            }
        }
        else {
            services.AddScoped<IAIProvider, NullAIProvider>();
        }
    }

    private static void RegisterAssistants(IServiceCollection services) {
        services.AddScoped<Search.CampingSearchAssistant>();
        services.AddScoped<Recommendations.RecommendationService>();
        services.AddScoped<Recommendations.CampingRecommendationAssistant>();
        services.AddScoped<Comparisons.CampingComparisonAssistant>();
    }
}
