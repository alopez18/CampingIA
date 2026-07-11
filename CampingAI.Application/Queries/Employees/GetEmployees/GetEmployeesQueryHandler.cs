namespace CampingAI.Application.Queries.Employees.GetEmployees;
public class GetEmployeesQueryHandler : Abstractions.Query.IQueryHandler<GetEmployeesQuery, IEnumerable<Shared.DTOs.GetEmployeeByIdItemDto>> {
    #region Dependencias
    readonly Domain.Repositories.Employees.IEmpoyeesReadRepository _empoyeesReadRepository;
    readonly Shared.Mappers.GetEmployeeByIdItemDtoMapper _empoyeesMapper;
    #endregion


    public GetEmployeesQueryHandler(Domain.Repositories.Employees.IEmpoyeesReadRepository empoyeesReadRepository,
                                    Shared.Mappers.GetEmployeeByIdItemDtoMapper empoyeesMapper) {
        _empoyeesReadRepository = empoyeesReadRepository;
        _empoyeesMapper = empoyeesMapper;
    }

    public async Task<IEnumerable<Shared.DTOs.GetEmployeeByIdItemDto>> HandleAsync(GetEmployeesQuery query) {
        var entities = await _empoyeesReadRepository.GetAllAsync(true);
        var resultado = _empoyeesMapper.Map(entities);
        return resultado;
    }
}