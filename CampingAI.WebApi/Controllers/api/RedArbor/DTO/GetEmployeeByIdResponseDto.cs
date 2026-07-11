namespace CampingAI.WebApi.Controllers.api.RedArbor.DTO;
public class GetEmployeeByIdResponseDto : Shared.ResponseWrapper<EmployeeItemResponseDto> {
    public GetEmployeeByIdResponseDto(EmployeeItemResponseDto data) : base(data) {
    }
}



