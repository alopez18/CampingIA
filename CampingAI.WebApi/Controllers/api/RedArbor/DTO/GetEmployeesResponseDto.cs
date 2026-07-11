namespace CampingAI.WebApi.Controllers.api.RedArbor.DTO;
public class GetEmployeesResponseDto : Shared.ResponseWrapper<IEnumerable<DTO.EmployeeItemResponseDto>> {


    public GetEmployeesResponseDto(IEnumerable<DTO.EmployeeItemResponseDto> data) : base(data) {

    }
}