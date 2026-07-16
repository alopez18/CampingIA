using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api.Categories;

[Route("api/categories")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CategoriesController : ControllerBase {

    #region Dependencias
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Category.GetCategories.GetCategoriesQuery, Application.Queries.Category.GetCategories.GetCategoriesResult> _getCategoriesQueryHandler;
    readonly Mappers.CategoryResponseMapper _categoryResponseMapper;
    #endregion

    public CategoriesController(
        Application.Abstractions.Query.IQueryHandler<Application.Queries.Category.GetCategories.GetCategoriesQuery, Application.Queries.Category.GetCategories.GetCategoriesResult> getCategoriesQueryHandler,
        Mappers.CategoryResponseMapper categoryResponseMapper) {
        _getCategoriesQueryHandler = getCategoriesQueryHandler;
        _categoryResponseMapper = categoryResponseMapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DTO.CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategories() {
        var result = await _getCategoriesQueryHandler.HandleAsync(
            new Application.Queries.Category.GetCategories.GetCategoriesQuery());

        return Ok(result.Items.Select(_categoryResponseMapper.Map));
    }
}
