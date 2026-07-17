using FluentValidation;

namespace CampingAI.Application.Commands.Favorite.RemoveFavorite;
public class RemoveFavoriteCommandHandler : Abstractions.Command.ICommandHandler<RemoveFavoriteCommand> {

    #region Dependencias
    readonly Domain.Repositories.IFavoritesWriteRepository _favoritesWriteRepository;
    readonly IValidator<RemoveFavoriteCommand> _validator;
    #endregion

    public RemoveFavoriteCommandHandler(Domain.Repositories.IFavoritesWriteRepository favoritesWriteRepository,
                                        IValidator<RemoveFavoriteCommand> validator) {
        _favoritesWriteRepository = favoritesWriteRepository;
        _validator = validator;
    }

    public async Task HandleAsync(RemoveFavoriteCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        await _favoritesWriteRepository.DeleteAsync(command.UserId, command.CampingId);
    }
}
