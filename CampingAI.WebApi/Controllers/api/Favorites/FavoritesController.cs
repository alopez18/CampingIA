using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.api.Favorites;
[Route("api/favorites")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FavoritesController : ControllerBase {

    #region Dependencias
    readonly ILogger<FavoritesController> _logger;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Favorite.AddFavorite.AddFavoriteCommand, Domain.Entities.Favorite> _addFavoriteCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Favorite.RemoveFavorite.RemoveFavoriteCommand> _removeFavoriteCommandHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Favorite.GetFavorites.GetFavoritesQuery, IEnumerable<Domain.Entities.Favorite>> _getFavoritesQueryHandler;
    readonly Mappers.FavoriteResponseMapper _favoriteResponseMapper;
    #endregion

    public FavoritesController(ILogger<FavoritesController> logger,
                               Application.Abstractions.Command.ICommandHandler<Application.Commands.Favorite.AddFavorite.AddFavoriteCommand, Domain.Entities.Favorite> addFavoriteCommandHandler,
                               Application.Abstractions.Command.ICommandHandler<Application.Commands.Favorite.RemoveFavorite.RemoveFavoriteCommand> removeFavoriteCommandHandler,
                               Application.Abstractions.Query.IQueryHandler<Application.Queries.Favorite.GetFavorites.GetFavoritesQuery, IEnumerable<Domain.Entities.Favorite>> getFavoritesQueryHandler,
                               Mappers.FavoriteResponseMapper favoriteResponseMapper) {
        _logger = logger;
        _addFavoriteCommandHandler = addFavoriteCommandHandler;
        _removeFavoriteCommandHandler = removeFavoriteCommandHandler;
        _getFavoritesQueryHandler = getFavoritesQueryHandler;
        _favoriteResponseMapper = favoriteResponseMapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(DTO.FavoriteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddFavorite([FromBody] DTO.AddFavoriteRequest request) {
        var userId = GetCurrentUserId();

        var favorite = await _addFavoriteCommandHandler.HandleAsync(
            new Application.Commands.Favorite.AddFavorite.AddFavoriteCommand(userId, request.CampingId));

        return CreatedAtAction(nameof(GetFavorites), _favoriteResponseMapper.Map(favorite));
    }

    [HttpDelete("{campingId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveFavorite([FromRoute] Guid campingId) {
        var userId = GetCurrentUserId();

        await _removeFavoriteCommandHandler.HandleAsync(
            new Application.Commands.Favorite.RemoveFavorite.RemoveFavoriteCommand(userId, campingId));

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DTO.FavoriteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Shared.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFavorites() {
        var userId = GetCurrentUserId();

        var favorites = await _getFavoritesQueryHandler.HandleAsync(
            new Application.Queries.Favorite.GetFavorites.GetFavoritesQuery(userId));

        return Ok(favorites.Select(_favoriteResponseMapper.Map));
    }

    private Guid GetCurrentUserId() {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new KeyNotFoundException("El token no contiene el identificador de usuario.");
        return Guid.Parse(claim);
    }
}
