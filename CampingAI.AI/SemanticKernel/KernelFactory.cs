using Microsoft.SemanticKernel;

namespace CampingAI.AI.SemanticKernel;
/// <summary>
/// Construye el <see cref="Kernel"/> de Semantic Kernel configurado con Google Gemini.
/// Es la única dependencia directa del conector de Gemini.
/// </summary>
public static class KernelFactory {

    public static Kernel CreateGeminiKernel(string apiKey, string model) {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("La API Key de Gemini es obligatoria.", nameof(apiKey));

        var builder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0070
        builder.AddGoogleAIGeminiChatCompletion(model, apiKey);
#pragma warning restore SKEXP0070

        return builder.Build();
    }
}
