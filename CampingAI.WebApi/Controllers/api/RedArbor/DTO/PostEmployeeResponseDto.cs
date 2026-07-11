namespace CampingAI.WebApi.Controllers.api.RedArbor.DTO;
public class PostEmployeeResponseDto : Shared.ResponseWrapper<EmployeeItemResponseDto> {
    public PostEmployeeResponseDto(EmployeeItemResponseDto data) : base(data) {
    }
}
