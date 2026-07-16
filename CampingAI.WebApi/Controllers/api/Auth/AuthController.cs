using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api.Auth;
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase {

    #region Dependencias
    readonly ILogger<AuthController> _logger;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RegisterUser.RegisterUserCommand, Domain.Entities.User> _registerUserCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.LoginUser.LoginUserCommand, string> _loginUserCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.GoogleLoginUser.GoogleLoginUserCommand, string> _googleLoginUserCommandHandler;
    readonly Mappers.AuthResponseMapper _authResponseMapper;
    readonly IConfiguration _configuration;
    #endregion

    public AuthController(ILogger<AuthController> logger,
                          Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RegisterUser.RegisterUserCommand, Domain.Entities.User> registerUserCommandHandler,
                          Application.Abstractions.Command.ICommandHandler<Application.Commands.User.LoginUser.LoginUserCommand, string> loginUserCommandHandler,
                          Application.Abstractions.Command.ICommandHandler<Application.Commands.User.GoogleLoginUser.GoogleLoginUserCommand, string> googleLoginUserCommandHandler,
                          Mappers.AuthResponseMapper authResponseMapper,
                          IConfiguration configuration) {
        _logger = logger;
        _registerUserCommandHandler = registerUserCommandHandler;
        _loginUserCommandHandler = loginUserCommandHandler;
        _googleLoginUserCommandHandler = googleLoginUserCommandHandler;
        _authResponseMapper = authResponseMapper;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DTO.AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] DTO.RegisterRequest request) {
        var command = new Application.Commands.User.RegisterUser.RegisterUserCommand(request.Email,
                                                                                    request.Password,
                                                                                    request.Name);
        var user = await _registerUserCommandHandler.HandleAsync(command);

        var token = await _loginUserCommandHandler.HandleAsync(
            new Application.Commands.User.LoginUser.LoginUserCommand(request.Email, request.Password));

        return CreatedAtAction(nameof(Register), _authResponseMapper.Map(user, token));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DTO.AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] DTO.LoginRequest request) {
        var token = await _loginUserCommandHandler.HandleAsync(
            new Application.Commands.User.LoginUser.LoginUserCommand(request.Email, request.Password));

        return Ok(new DTO.AuthResponse { Token = token, Email = request.Email });
    }

    [HttpPost("google")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DTO.AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GoogleLogin([FromBody] DTO.GoogleLoginRequest request) {
        var clientId = _configuration["GoogleAuth:ClientId"];

        var validationSettings = new GoogleJsonWebSignature.ValidationSettings {
            Audience = clientId is not null ? [clientId] : null
        };

        GoogleJsonWebSignature.Payload payload;
        try {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
        }
        catch (InvalidJwtException ex) {
            _logger.LogWarning(ex, "Google ID token inválido");
            return Unauthorized(new Shared.ErrorResponse("Token de Google inválido."));
        }

        var token = await _googleLoginUserCommandHandler.HandleAsync(
            new Application.Commands.User.GoogleLoginUser.GoogleLoginUserCommand(payload.Email, payload.Name));

        return Ok(new DTO.AuthResponse { Token = token, Email = payload.Email, Name = payload.Name });
    }
}
