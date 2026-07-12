using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.api.Reservations;
[Route("api/reservations")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ReservationsController : ControllerBase {

    #region Dependencias
    readonly ILogger<ReservationsController> _logger;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Reservation.CreateReservation.CreateReservationCommand, Domain.Entities.Reservation> _createReservationCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Reservation.CancelReservation.CancelReservationCommand> _cancelReservationCommandHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Reservation.GetUserReservations.GetUserReservationsQuery, IEnumerable<Domain.Entities.Reservation>> _getUserReservationsQueryHandler;
    readonly Mappers.ReservationResponseMapper _reservationResponseMapper;
    #endregion

    public ReservationsController(ILogger<ReservationsController> logger,
                                   Application.Abstractions.Command.ICommandHandler<Application.Commands.Reservation.CreateReservation.CreateReservationCommand, Domain.Entities.Reservation> createReservationCommandHandler,
                                   Application.Abstractions.Command.ICommandHandler<Application.Commands.Reservation.CancelReservation.CancelReservationCommand> cancelReservationCommandHandler,
                                   Application.Abstractions.Query.IQueryHandler<Application.Queries.Reservation.GetUserReservations.GetUserReservationsQuery, IEnumerable<Domain.Entities.Reservation>> getUserReservationsQueryHandler,
                                   Mappers.ReservationResponseMapper reservationResponseMapper) {
        _logger = logger;
        _createReservationCommandHandler = createReservationCommandHandler;
        _cancelReservationCommandHandler = cancelReservationCommandHandler;
        _getUserReservationsQueryHandler = getUserReservationsQueryHandler;
        _reservationResponseMapper = reservationResponseMapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(DTO.ReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReservation([FromBody] DTO.CreateReservationRequest request) {
        var userId = GetCurrentUserId();

        var reservation = await _createReservationCommandHandler.HandleAsync(
            new Application.Commands.Reservation.CreateReservation.CreateReservationCommand(
                userId, request.CampingId, request.CheckIn, request.CheckOut, request.TotalPrice));

        return CreatedAtAction(nameof(GetReservations), _reservationResponseMapper.Map(reservation));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelReservation([FromRoute] Guid id) {
        var userId = GetCurrentUserId();

        await _cancelReservationCommandHandler.HandleAsync(
            new Application.Commands.Reservation.CancelReservation.CancelReservationCommand(id, userId));

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DTO.ReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReservations() {
        var userId = GetCurrentUserId();

        var reservations = await _getUserReservationsQueryHandler.HandleAsync(
            new Application.Queries.Reservation.GetUserReservations.GetUserReservationsQuery(userId));

        return Ok(reservations.Select(_reservationResponseMapper.Map));
    }

    private Guid GetCurrentUserId() {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new KeyNotFoundException("El token no contiene el identificador de usuario.");
        return Guid.Parse(claim);
    }
}
