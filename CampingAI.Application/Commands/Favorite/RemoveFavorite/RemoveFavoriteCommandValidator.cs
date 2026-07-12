using FluentValidation;

namespace CampingAI.Application.Commands.Favorite.RemoveFavorite;
public class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand> {
    public RemoveFavoriteCommandValidator() {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("El UserId no puede estar vacío.");

        RuleFor(x => x.CampingId)
            .NotEqual(Guid.Empty).WithMessage("El CampingId no puede estar vacío.");
    }
}
