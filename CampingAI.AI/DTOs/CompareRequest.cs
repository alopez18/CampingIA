namespace CampingAI.AI.DTOs;
/// <summary>
/// Petición para comparar campings por sus identificadores.
/// </summary>
public class CompareRequest {
    public IEnumerable<Guid> CampingIds { get; set; } = [];
}
