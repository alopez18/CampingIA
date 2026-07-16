namespace CampingAI.WebApi.Controllers.api.AI.DTO;
public record AiSearchRequest(string Query, int Page = 1, int PageSize = 20) {
}
