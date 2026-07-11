using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.api.Users;
[Route("api/users")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : ControllerBase {

    #region Dependencias
    readonly ILogger<UsersController> _logger;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetCurrentUser.GetCurrentUserQuery, Domain.Entities.User> _getCurrentUserQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.UpdateUser.UpdateUserCommand, Domain.Entities.User> _updateUserCommandHandler;
    readonly Mappers.UserResponseMapper _userResponseMapper;
    #endregion

    public UsersController(ILogger<UsersController> logger,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetCurrentUser.GetCurrentUserQuery, Domain.Entities.User> getCurrentUserQueryHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.UpdateUser.UpdateUserCommand, Domain.Entities.User> updateUserCommandHandler,
                           Mappers.UserResponseMapper userResponseMapper) {
        _logger = logger;
        _getCurrentUserQueryHandler = getCurrentUserQueryHandler;
        _updateUserCommandHandler = updateUserCommandHandler;
        _userResponseMapper = userResponseMapper;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(DTO.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMe() {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new KeyNotFoundException("El token no contiene el identificador de usuario.");

        var userId = Guid.Parse(userIdClaim);
        var user = await _getCurrentUserQueryHandler.HandleAsync(
            new Application.Queries.User.GetCurrentUser.GetCurrentUserQuery(userId));

        return Ok(_userResponseMapper.Map(user));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DTO.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] DTO.UpdateUserRequest request) {
        var command = new Application.Commands.User.UpdateUser.UpdateUserCommand(id,
                                                                                request.Name,
                                                                                request.Email,
                                                                                request.Password);
        var user = await _updateUserCommandHandler.HandleAsync(command);

        return Ok(_userResponseMapper.Map(user));
    }
}
