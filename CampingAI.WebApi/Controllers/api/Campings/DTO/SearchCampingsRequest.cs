using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api.Campings.DTO;
public record SearchCampingsRequest(
    [FromQuery] string? Name,
    [FromQuery] Guid? ProvinciaId,
    [FromQuery] string? ProvinciaCode,
    [FromQuery] IEnumerable<Guid>? CategoryIds,
    [FromQuery] decimal? MinPrice,
    [FromQuery] decimal? MaxPrice,
    [FromQuery] IEnumerable<Guid>? FacilityIds,
    [FromQuery] decimal? MinLat,
    [FromQuery] decimal? MaxLat,
    [FromQuery] decimal? MinLng,
    [FromQuery] decimal? MaxLng,
    [FromQuery] int Page = 1,
    [FromQuery] int PageSize = 10);
