using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api.Facilities;

[Route("api/facilities")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FacilitiesController : ControllerBase {

    #region Dependencias
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Facility.GetFacilities.GetFacilitiesQuery, Application.Queries.Facility.GetFacilities.GetFacilitiesResult> _getFacilitiesQueryHandler;
    readonly Mappers.FacilityResponseMapper _facilityResponseMapper;
    #endregion

    public FacilitiesController(
        Application.Abstractions.Query.IQueryHandler<Application.Queries.Facility.GetFacilities.GetFacilitiesQuery, Application.Queries.Facility.GetFacilities.GetFacilitiesResult> getFacilitiesQueryHandler,
        Mappers.FacilityResponseMapper facilityResponseMapper) {
        _getFacilitiesQueryHandler = getFacilitiesQueryHandler;
        _facilityResponseMapper = facilityResponseMapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DTO.FacilityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFacilities() {
        var result = await _getFacilitiesQueryHandler.HandleAsync(
            new Application.Queries.Facility.GetFacilities.GetFacilitiesQuery());

        return Ok(result.Items.Select(_facilityResponseMapper.Map));
    }
}
