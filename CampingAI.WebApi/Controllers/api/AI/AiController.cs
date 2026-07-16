using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.api.AI;
[Route("api/ai")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AiController : ControllerBase {

    #region Dependencias
    readonly ILogger<AiController> _logger;
    readonly CampingAI.AI.Search.CampingSearchAssistant _searchAssistant;
    readonly CampingAI.AI.Recommendations.CampingRecommendationAssistant _recommendationAssistant;
    readonly CampingAI.AI.Comparisons.CampingComparisonAssistant _comparisonAssistant;
    readonly Campings.Mappers.CampingResponseMapper _campingResponseMapper;
    #endregion

    public AiController(ILogger<AiController> logger,
                        CampingAI.AI.Search.CampingSearchAssistant searchAssistant,
                        CampingAI.AI.Recommendations.CampingRecommendationAssistant recommendationAssistant,
                        CampingAI.AI.Comparisons.CampingComparisonAssistant comparisonAssistant,
                        Campings.Mappers.CampingResponseMapper campingResponseMapper) {
        _logger = logger;
        _searchAssistant = searchAssistant;
        _recommendationAssistant = recommendationAssistant;
        _comparisonAssistant = comparisonAssistant;
        _campingResponseMapper = campingResponseMapper;
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(Campings.DTO.PagedCampingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Search([FromBody] DTO.AiSearchRequest request, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new Shared.ErrorResponse("La consulta no puede estar vacía."));

        var result = await _searchAssistant.SearchAsync(request.Query, request.Page, request.PageSize, cancellationToken);

        var response = new Campings.DTO.PagedCampingsResponse(
            result.Items.Select(_campingResponseMapper.Map),
            result.TotalCount,
            request.Page,
            request.PageSize);

        return Ok(response);
    }

    [HttpGet("recommendations")]
    [ProducesResponseType(typeof(DTO.AiRecommendationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecommendations([FromQuery] int maxResults = 5, CancellationToken cancellationToken = default) {
        var userId = GetCurrentUserId();

        var (response, campings) = await _recommendationAssistant.RecommendAsync(userId, maxResults, cancellationToken);

        return Ok(new DTO.AiRecommendationResponse(
            campings.Select(_campingResponseMapper.Map),
            response.Reasoning));
    }

    [HttpPost("compare")]
    [ProducesResponseType(typeof(CampingAI.AI.DTOs.CompareResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Compare([FromBody] DTO.AiCompareRequest request, CancellationToken cancellationToken) {
        try {
            var result = await _comparisonAssistant.CompareAsync(request.CampingIds, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex) {
            return BadRequest(new Shared.ErrorResponse(ex.Message));
        }
    }

    private Guid GetCurrentUserId() {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new KeyNotFoundException("El token no contiene el identificador de usuario.");
        return Guid.Parse(claim);
    }
}
