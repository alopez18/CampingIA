using FluentValidation;

namespace CampingAI.Application.Commands.Favorite.AddFavorite;
public class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand> {
    public AddFavoriteCommandValidator() {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("El UserId no puede estar vacío.");

        RuleFor(x => x.CampingId)
            .NotEqual(Guid.Empty).WithMessage("El CampingId no puede estar vacío.");
    }
}
