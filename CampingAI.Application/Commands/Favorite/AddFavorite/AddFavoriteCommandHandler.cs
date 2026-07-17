using FluentValidation;

namespace CampingAI.Application.Commands.Favorite.AddFavorite;
public class AddFavoriteCommandHandler : Abstractions.Command.ICommandHandler<AddFavoriteCommand, Domain.Entities.Favorite> {

    #region Dependencias
    readonly Domain.Repositories.IFavoritesReadRepository _favoritesReadRepository;
    readonly Domain.Repositories.IFavoritesWriteRepository _favoritesWriteRepository;
    readonly IValidator<AddFavoriteCommand> _validator;
    #endregion

    public AddFavoriteCommandHandler(Domain.Repositories.IFavoritesReadRepository favoritesReadRepository,
                                     Domain.Repositories.IFavoritesWriteRepository favoritesWriteRepository,
                                     IValidator<AddFavoriteCommand> validator) {
        _favoritesReadRepository = favoritesReadRepository;
        _favoritesWriteRepository = favoritesWriteRepository;
        _validator = validator;
    }

    public async Task<Domain.Entities.Favorite> HandleAsync(AddFavoriteCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var exists = await _favoritesReadRepository.ExistsAsync(command.UserId, command.CampingId);
        if (exists)
            throw new Domain.Exceptions.DomainException("El camping ya está en favoritos del usuario.");

        var favorite = Domain.Entities.Favorite.CreateNew(command.UserId, command.CampingId);

        await _favoritesWriteRepository.AddAsync(favorite);

        return favorite;
    }
}
