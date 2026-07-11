using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.api.RedArbor;
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class RedArborController : ControllerBase {

    #region Dependencias
    readonly ILogger<RedArborController> _logger;

    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Employee.CreateEmployee.CreateEmployeeCommand, Domain.Entities.Employee> _createEmployeeCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Employee.UpdateEmployee.UpdateEmployeeCommand, Domain.Entities.Employee> _updateEmployeeCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Employee.DeleteEmployee.DeleteEmployeeCommand> _deleteEmployeeCommandHandler;

    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Employee.GetEmployeeById.GetEmployeeByIdQuery, Application.Shared.DTOs.GetEmployeeByIdItemDto> _getEmployeeByIdQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Employees.GetEmployees.GetEmployeesQuery, IEnumerable<Application.Shared.DTOs.GetEmployeeByIdItemDto>> _getEmployeesQueryHandler;

    readonly Mappers.EmployeeItemResponseDtoMapper _employeeItemResponseDtoMapper;
    #endregion

    public RedArborController(ILogger<RedArborController> logger,
                              Application.Abstractions.Command.ICommandHandler<Application.Commands.Employee.CreateEmployee.CreateEmployeeCommand, Domain.Entities.Employee> createEmployeeCommandHandler,
                              Application.Abstractions.Command.ICommandHandler<Application.Commands.Employee.UpdateEmployee.UpdateEmployeeCommand, Domain.Entities.Employee> updateEmployeeCommandHandler,
                              Application.Abstractions.Command.ICommandHandler<Application.Commands.Employee.DeleteEmployee.DeleteEmployeeCommand> deleteEmployeeCommandHandler,
                              Application.Abstractions.Query.IQueryHandler<Application.Queries.Employee.GetEmployeeById.GetEmployeeByIdQuery, Application.Shared.DTOs.GetEmployeeByIdItemDto> getEmployeeByIdQueryHandler,
                              Application.Abstractions.Query.IQueryHandler<Application.Queries.Employees.GetEmployees.GetEmployeesQuery, IEnumerable<Application.Shared.DTOs.GetEmployeeByIdItemDto>> getEmployeesQueryHandler,
                              Mappers.EmployeeItemResponseDtoMapper employeeItemResponseDtoMapper) {
        _logger = logger;
        _createEmployeeCommandHandler = createEmployeeCommandHandler;
        _updateEmployeeCommandHandler = updateEmployeeCommandHandler;
        _deleteEmployeeCommandHandler = deleteEmployeeCommandHandler;
        _getEmployeeByIdQueryHandler = getEmployeeByIdQueryHandler;
        _getEmployeesQueryHandler = getEmployeesQueryHandler;
        _employeeItemResponseDtoMapper = employeeItemResponseDtoMapper;


    }



    [HttpGet("{request.Id}")]
    [ProducesResponseType(typeof(DTO.GetEmployeeByIdResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmployeeById([FromRoute] DTO.GetEmployeeByIdRequestDto request) {
        Application.Shared.DTOs.GetEmployeeByIdItemDto dtoItem = await _getEmployeeByIdQueryHandler.HandleAsync(new Application.Queries.Employee.GetEmployeeById.GetEmployeeByIdQuery(request.Id));

        var result = new DTO.EmployeeItemResponseDto {
            Id = dtoItem.Id,
            Username = dtoItem.Username,
            Name = dtoItem.Name,
            Email = dtoItem.Email,
            Fax = dtoItem.Fax,
            CompanyId = dtoItem.CompanyId,
            PortalId = dtoItem.PortalId,
            RoleId = dtoItem.RoleId,
            StatusId = dtoItem.StatusId,
            Telephone = dtoItem.Telephone,
            CreatedOn = dtoItem.CreatedOn,
            UpdatedOn = dtoItem.UpdatedOn,
            LastLogin = dtoItem.LastLogin,
            DeletedOn = dtoItem.DeletedOn
        };
        return Ok(new DTO.GetEmployeeByIdResponseDto(result));
    }


    [HttpGet("")]
    [ProducesResponseType(typeof(DTO.GetEmployeesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmployees() {
        IEnumerable<Application.Shared.DTOs.GetEmployeeByIdItemDto> dtos = await _getEmployeesQueryHandler.HandleAsync(new Application.Queries.Employees.GetEmployees.GetEmployeesQuery());
        var result = _employeeItemResponseDtoMapper.Map(dtos);


        return Ok(new DTO.GetEmployeesResponseDto(result));
    }


    [HttpPost()]
    [ProducesResponseType(typeof(DTO.PostEmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostEmployee([FromBody] DTO.PostEmployeeRequestDto request) {

        //Soy consciente de que perdemos datos de auditoria que vienen en la request, pero es que no tiene sentido que el cliente las informe.
        Application.Commands.Employee.CreateEmployee.CreateEmployeeCommand command =
            new Application.Commands.Employee.CreateEmployee.CreateEmployeeCommand(
                request.Username,
                request.Name,
                request.Email,
                request.Fax,
                request.Password,
                request.CompanyId,
                request.PortalId,
                request.RoleId,
                request.StatusId,
                request.Telephone
            );
        Domain.Entities.Employee newEmployee = await _createEmployeeCommandHandler.HandleAsync(command);

        var result = new DTO.EmployeeItemResponseDto {
            Id = newEmployee.Id,
            UpdatedOn = newEmployee.UpdatedOn.Value,
            CompanyId = newEmployee.CompanyId,
            CreatedOn = newEmployee.CreatedOn.Value,
            DeletedOn = newEmployee.DeletedOn,
            Email = newEmployee.Email,
            Fax = newEmployee.Fax,
            LastLogin = newEmployee.LastLogin != null ? newEmployee.LastLogin.Value : null,
            Name = newEmployee.Name,
            PortalId = newEmployee.PortalId,
            RoleId = newEmployee.RoleId,
            StatusId = newEmployee.StatusId,
            Telephone = newEmployee.Telephone,
            Username = newEmployee.Username
        };

        return Ok(new DTO.GetEmployeeByIdResponseDto(result));
    }


    [HttpPut("{Id}")]
    [ProducesResponseType(typeof(DTO.PostEmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutEmployee([FromRoute] Guid Id, [FromBody] DTO.PostEmployeeRequestDto request) {

        //Soy consciente de que perdemos datos de auditoria que vienen en la request, pero es que no tiene sentido que el cliente las informe.
        Application.Commands.Employee.UpdateEmployee.UpdateEmployeeCommand command =
            new Application.Commands.Employee.UpdateEmployee.UpdateEmployeeCommand(Id,
                                                                                   request.Username,
                                                                                   request.Name,
                                                                                   request.Email,
                                                                                   request.Fax,
                                                                                   request.Password,
                                                                                   request.CompanyId,
                                                                                   request.PortalId,
                                                                                   request.RoleId,
                                                                                   request.StatusId,
                                                                                   request.Telephone
                                                                                );
        Domain.Entities.Employee newEmployee = await _updateEmployeeCommandHandler.HandleAsync(command);

        var result = new DTO.EmployeeItemResponseDto {
            Id = newEmployee.Id,
            UpdatedOn = newEmployee.UpdatedOn.Value,
            CompanyId = newEmployee.CompanyId,
            CreatedOn = newEmployee.CreatedOn.Value,
            DeletedOn = newEmployee.DeletedOn,
            Email = newEmployee.Email,
            Fax = newEmployee.Fax,
            LastLogin = newEmployee.LastLogin != null ? newEmployee.LastLogin.Value : null,
            Name = newEmployee.Name,
            PortalId = newEmployee.PortalId,
            RoleId = newEmployee.RoleId,
            StatusId = newEmployee.StatusId,
            Telephone = newEmployee.Telephone,
            Username = newEmployee.Username
        };

        return Ok(new DTO.GetEmployeeByIdResponseDto(result));
    }


    [HttpDelete("{request.Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEmployeeById([FromRoute] DTO.GetEmployeeByIdRequestDto request) {
        await _deleteEmployeeCommandHandler.HandleAsync(new Application.Commands.Employee.DeleteEmployee.DeleteEmployeeCommand(request.Id));
        return Ok();
    }

    [HttpGet("profile")]
    public IActionResult GetProfile() {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var googleId = User.FindFirst("google_id")?.Value;

        return Ok(new {
            Id = userId,
            Email = email,
            Name = name,
            GoogleId = googleId
        });
    }

}