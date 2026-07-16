using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api.Campings;
[Route("api/campings")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CampingsController : ControllerBase {

    #region Dependencias
    readonly ILogger<CampingsController> _logger;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampings.GetCampingsQuery, Application.Queries.Camping.GetCampings.GetCampingsResult> _getCampingsQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping> _getCampingByIdQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.SearchCampings.SearchCampingsQuery, Application.Queries.Camping.SearchCampings.SearchCampingsResult> _searchCampingsQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping> _createCampingCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping> _updateCampingCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.DeleteCamping.DeleteCampingCommand> _deleteCampingCommandHandler;
    readonly Mappers.CampingResponseMapper _campingResponseMapper;
    #endregion

    public CampingsController(ILogger<CampingsController> logger,
                               Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampings.GetCampingsQuery, Application.Queries.Camping.GetCampings.GetCampingsResult> getCampingsQueryHandler,
                               Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping> getCampingByIdQueryHandler,
                               Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.SearchCampings.SearchCampingsQuery, Application.Queries.Camping.SearchCampings.SearchCampingsResult> searchCampingsQueryHandler,
                               Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping> createCampingCommandHandler,
                               Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping> updateCampingCommandHandler,
                               Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.DeleteCamping.DeleteCampingCommand> deleteCampingCommandHandler,
                               Mappers.CampingResponseMapper campingResponseMapper) {
        _logger = logger;
        _getCampingsQueryHandler = getCampingsQueryHandler;
        _getCampingByIdQueryHandler = getCampingByIdQueryHandler;
        _searchCampingsQueryHandler = searchCampingsQueryHandler;
        _createCampingCommandHandler = createCampingCommandHandler;
        _updateCampingCommandHandler = updateCampingCommandHandler;
        _deleteCampingCommandHandler = deleteCampingCommandHandler;
        _campingResponseMapper = campingResponseMapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DTO.PagedCampingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCampings([FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
        var result = await _getCampingsQueryHandler.HandleAsync(
            new Application.Queries.Camping.GetCampings.GetCampingsQuery(page, pageSize));

        var response = new DTO.PagedCampingsResponse(
            result.Items.Select(_campingResponseMapper.Map),
            result.TotalCount,
            page,
            pageSize);

        return Ok(response);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(DTO.PagedCampingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchCampings([FromQuery] DTO.SearchCampingsRequest request) {
        var query = new Application.Queries.Camping.SearchCampings.SearchCampingsQuery(
            request.Name,
            request.ProvinciaId,
            request.ProvinciaCode,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.FacilityIds,
            request.MinLat,
            request.MaxLat,
            request.MinLng,
            request.MaxLng,
            request.Page,
            request.PageSize);

        var result = await _searchCampingsQueryHandler.HandleAsync(query);

        var response = new DTO.PagedCampingsResponse(
            result.Items.Select(_campingResponseMapper.Map),
            result.TotalCount,
            request.Page,
            request.PageSize);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DTO.CampingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCampingById([FromRoute] Guid id) {
        var camping = await _getCampingByIdQueryHandler.HandleAsync(
            new Application.Queries.Camping.GetCampingById.GetCampingByIdQuery(id));

        return Ok(_campingResponseMapper.Map(camping));
    }

    [HttpPost]
    [ProducesResponseType(typeof(DTO.CampingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCamping([FromBody] DTO.CreateCampingRequest request) {
        var command = new Application.Commands.Camping.CreateCamping.CreateCampingCommand(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.PricePerNight,
            request.OwnerId,
            request.CategoryId,
            request.ProvinciaId,
            request.FacilityIds);

        var camping = await _createCampingCommandHandler.HandleAsync(command);

        return CreatedAtAction(nameof(GetCampingById),
                               new { id = camping.Id },
                               _campingResponseMapper.Map(camping));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DTO.CampingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCamping([FromRoute] Guid id, [FromBody] DTO.UpdateCampingRequest request) {
        var command = new Application.Commands.Camping.UpdateCamping.UpdateCampingCommand(
            id,
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.PricePerNight,
            request.CategoryId,
            request.ProvinciaId,
            request.FacilityIds);

        var camping = await _updateCampingCommandHandler.HandleAsync(command);

        return Ok(_campingResponseMapper.Map(camping));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCamping([FromRoute] Guid id) {
        await _deleteCampingCommandHandler.HandleAsync(
            new Application.Commands.Camping.DeleteCamping.DeleteCampingCommand(id));

        return NoContent();
    }
}
