namespace CampingAI.WebApi.Controllers.api.Campings.DTO;
public record PagedCampingsResponse(IEnumerable<CampingResponse> Items,
                                    int TotalCount,
                                    int Page,
                                    int PageSize) {
}
