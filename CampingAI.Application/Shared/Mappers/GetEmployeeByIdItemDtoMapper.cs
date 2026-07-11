using CampingAI.Application.Shared.DTOs;

namespace CampingAI.Application.Shared.Mappers;
public class GetEmployeeByIdItemDtoMapper : Domain.Abstractions.Mappers.SimpleMapper<Domain.Entities.Employee, GetEmployeeByIdItemDto> {
    public override GetEmployeeByIdItemDto Map(Domain.Entities.Employee src) {
        return new GetEmployeeByIdItemDto(src.Id,
                                              src.Username,
                                              src.Name,
                                              src.Email,
                                              src.Fax,
                                              src.CompanyId,
                                              src.PortalId,
                                              src.RoleId,
                                              src.StatusId,
                                              src.Telephone,
                                              src.LastLogin != null ? src.LastLogin.Value : null,
                                              src.CreatedOn.Value,
                                              src.UpdatedOn.Value,
                                              src.DeletedOn);
    }





}