namespace CampingAI.AI.Embeddings;
/// <summary>
/// Contrato para un almacén vectorial (preparado para RAG en Fase 12).
/// Desacoplado de la implementación concreta (Qdrant, Azure AI Search, etc.).
/// </summary>
public interface IVectorStore {
    Task UpsertAsync(Guid id, IReadOnlyList<float> vector, IDictionary<string, string> metadata, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(Guid Id, float Score)>> SearchAsync(IReadOnlyList<float> queryVector, int topK, CancellationToken cancellationToken = default);
}
