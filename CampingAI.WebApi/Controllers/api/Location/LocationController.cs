using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api.Location;

[Route("api/location")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LocationController : ControllerBase {

    #region Dependencias
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetCountries.GetCountriesQuery, Application.Queries.Location.GetCountries.GetCountriesResult> _getCountriesQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetProvinces.GetProvincesQuery, Application.Queries.Location.GetProvinces.GetProvincesResult> _getProvincesQueryHandler;
    readonly Mappers.CountryResponseMapper _countryResponseMapper;
    readonly Mappers.ProvinceResponseMapper _provinceResponseMapper;
    #endregion

    public LocationController(
        Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetCountries.GetCountriesQuery, Application.Queries.Location.GetCountries.GetCountriesResult> getCountriesQueryHandler,
        Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetProvinces.GetProvincesQuery, Application.Queries.Location.GetProvinces.GetProvincesResult> getProvincesQueryHandler,
        Mappers.CountryResponseMapper countryResponseMapper,
        Mappers.ProvinceResponseMapper provinceResponseMapper) {
        _getCountriesQueryHandler = getCountriesQueryHandler;
        _getProvincesQueryHandler = getProvincesQueryHandler;
        _countryResponseMapper = countryResponseMapper;
        _provinceResponseMapper = provinceResponseMapper;
    }

    [HttpGet("countries")]
    [ProducesResponseType(typeof(IEnumerable<DTO.CountryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountries() {
        var result = await _getCountriesQueryHandler.HandleAsync(
            new Application.Queries.Location.GetCountries.GetCountriesQuery());

        return Ok(result.Items.Select(_countryResponseMapper.Map));
    }

    [HttpGet("countries/{countryId:guid}/provinces")]
    [ProducesResponseType(typeof(IEnumerable<DTO.ProvinceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProvincesByCountry([FromRoute] Guid countryId) {
        var result = await _getProvincesQueryHandler.HandleAsync(
            new Application.Queries.Location.GetProvinces.GetProvincesQuery(countryId));

        return Ok(result.Items.Select(_provinceResponseMapper.Map));
    }

    [HttpGet("provinces")]
    [ProducesResponseType(typeof(IEnumerable<DTO.ProvinceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProvinces() {
        var result = await _getProvincesQueryHandler.HandleAsync(
            new Application.Queries.Location.GetProvinces.GetProvincesQuery(null));

        return Ok(result.Items.Select(_provinceResponseMapper.Map));
    }
}
