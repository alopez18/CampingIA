namespace CampingAI.WebApi.Controllers.api.AI.DTO;
public record AiRecommendationResponse(IEnumerable<Campings.DTO.CampingResponse> Items,
                                       string Reasoning) {
}
