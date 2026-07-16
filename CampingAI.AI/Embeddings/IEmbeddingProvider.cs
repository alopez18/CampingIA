namespace CampingAI.AI.Embeddings;
/// <summary>
/// Contrato para la generación de embeddings (preparado para RAG en Fase 12).
/// Desacoplado del proveedor concreto.
/// </summary>
public interface IEmbeddingProvider {
    Task<IReadOnlyList<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}
