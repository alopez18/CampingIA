
namespace CampingAI.Application.Queries.Employee.GetEmployeeById;
public class GetEmployeeByIdQueryHandler : Abstractions.Query.IQueryHandler<GetEmployeeByIdQuery, Shared.DTOs.GetEmployeeByIdItemDto> {
    #region Dependencias
    readonly Domain.Repositories.Employees.IEmpoyeesReadRepository _empoyeesReadRepository;
    readonly Shared.Mappers.GetEmployeeByIdItemDtoMapper _empoyeesMapper;
    #endregion

    public GetEmployeeByIdQueryHandler(Domain.Repositories.Employees.IEmpoyeesReadRepository empoyeesReadRepository,
                                       Shared.Mappers.GetEmployeeByIdItemDtoMapper empoyeesMapper) {
        _empoyeesReadRepository = empoyeesReadRepository;
        _empoyeesMapper = empoyeesMapper;
    }


    public async Task<Shared.DTOs.GetEmployeeByIdItemDto> HandleAsync(GetEmployeeByIdQuery query) {
        var employeeEntity = await _empoyeesReadRepository.Get(query.Id, true);
        if (employeeEntity == null) {
            throw new KeyNotFoundException($"Employee with id {query.Id} not found.");
        }

        return new Shared.DTOs.GetEmployeeByIdItemDto(
            employeeEntity.Id,
            employeeEntity.Username,
            employeeEntity.Name,
            employeeEntity.Email,
            employeeEntity.Fax,
            employeeEntity.CompanyId,
            employeeEntity.PortalId,
            employeeEntity.RoleId,
            employeeEntity.StatusId,
            employeeEntity.Telephone,
            employeeEntity.LastLogin != null ? employeeEntity.LastLogin.Value : null,
            employeeEntity.CreatedOn.Value,
            employeeEntity.UpdatedOn.Value,
            employeeEntity.DeletedOn);

    }
}