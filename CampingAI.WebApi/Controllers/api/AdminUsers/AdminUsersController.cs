using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api.AdminUsers;

[Route("api/admin/users")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class AdminUsersController : ControllerBase {

    #region Dependencias
    readonly ILogger<AdminUsersController> _logger;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetAllUsers.GetAllUsersQuery, Application.Queries.User.GetAllUsers.GetAllUsersResult> _getAllUsersQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetUserById.GetUserByIdQuery, Domain.Entities.User> _getUserByIdQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.AdminUpdateUser.AdminUpdateUserCommand, Domain.Entities.User> _adminUpdateUserCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.DeleteUser.DeleteUserCommand> _deleteUserCommandHandler;
    readonly Controllers.api.Users.Mappers.UserResponseMapper _userResponseMapper;
    #endregion

    public AdminUsersController(ILogger<AdminUsersController> logger,
                                Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetAllUsers.GetAllUsersQuery, Application.Queries.User.GetAllUsers.GetAllUsersResult> getAllUsersQueryHandler,
                                Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetUserById.GetUserByIdQuery, Domain.Entities.User> getUserByIdQueryHandler,
                                Application.Abstractions.Command.ICommandHandler<Application.Commands.User.AdminUpdateUser.AdminUpdateUserCommand, Domain.Entities.User> adminUpdateUserCommandHandler,
                                Application.Abstractions.Command.ICommandHandler<Application.Commands.User.DeleteUser.DeleteUserCommand> deleteUserCommandHandler,
                                Controllers.api.Users.Mappers.UserResponseMapper userResponseMapper) {
        _logger = logger;
        _getAllUsersQueryHandler = getAllUsersQueryHandler;
        _getUserByIdQueryHandler = getUserByIdQueryHandler;
        _adminUpdateUserCommandHandler = adminUpdateUserCommandHandler;
        _deleteUserCommandHandler = deleteUserCommandHandler;
        _userResponseMapper = userResponseMapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DTO.PagedUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null) {
        var result = await _getAllUsersQueryHandler.HandleAsync(
            new Application.Queries.User.GetAllUsers.GetAllUsersQuery(page, pageSize, search));

        var response = new DTO.PagedUsersResponse(
            result.Items.Select(_userResponseMapper.Map),
            result.TotalCount,
            page,
            pageSize);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Controllers.api.Users.DTO.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id) {
        var user = await _getUserByIdQueryHandler.HandleAsync(new Application.Queries.User.GetUserById.GetUserByIdQuery(id));
        return Ok(_userResponseMapper.Map(user));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Controllers.api.Users.DTO.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] DTO.AdminUpdateUserRequest request) {
        Domain.Enums.UserRole? role = request.RoleId.HasValue
            ? (Domain.Enums.UserRole)request.RoleId.Value
            : null;

        var command = new Application.Commands.User.AdminUpdateUser.AdminUpdateUserCommand(id, request.Name, request.Email, role);
        var user = await _adminUpdateUserCommandHandler.HandleAsync(command);
        return Ok(_userResponseMapper.Map(user));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid id) {
        await _deleteUserCommandHandler.HandleAsync(new Application.Commands.User.DeleteUser.DeleteUserCommand(id));
        return NoContent();
    }
}
