namespace CampingAI.WebApi.Controllers.api.RedArbor.Mappers;
public class EmployeeItemResponseDtoMapper : Domain.Abstractions.Mappers.SimpleMapper<Application.Shared.DTOs.GetEmployeeByIdItemDto, DTO.EmployeeItemResponseDto> {
    public override DTO.EmployeeItemResponseDto Map(Application.Shared.DTOs.GetEmployeeByIdItemDto src) {
        var result = new DTO.EmployeeItemResponseDto {
            Id = src.Id,
            Username = src.Username,
            Name = src.Name,
            Email = src.Email,
            Fax = src.Fax,
            CompanyId = src.CompanyId,
            PortalId = src.PortalId,
            RoleId = src.RoleId,
            StatusId = src.StatusId,
            Telephone = src.Telephone,
            CreatedOn = src.CreatedOn,
            UpdatedOn = src.UpdatedOn,
            LastLogin = src.LastLogin,
            DeletedOn = src.DeletedOn
        };
        return result;
    }


}