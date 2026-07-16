using FluentValidation;

namespace CampingAI.Application.Queries.Camping.SearchCampings;
public class SearchCampingsQueryValidator : AbstractValidator<SearchCampingsQuery> {
    public SearchCampingsQueryValidator() {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("La página debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");

        When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue, () => {
            RuleFor(x => x.MaxPrice!.Value)
                .GreaterThanOrEqualTo(x => x.MinPrice!.Value)
                .WithMessage("El precio máximo debe ser mayor o igual al precio mínimo.");
        });

        When(x => x.MinPrice.HasValue, () => {
            RuleFor(x => x.MinPrice!.Value)
                .GreaterThanOrEqualTo(0m).WithMessage("El precio mínimo no puede ser negativo.");
        });

        When(x => x.MaxPrice.HasValue, () => {
            RuleFor(x => x.MaxPrice!.Value)
                .GreaterThanOrEqualTo(0m).WithMessage("El precio máximo no puede ser negativo.");
        });

        When(x => x.MinLat.HasValue, () => {
            RuleFor(x => x.MinLat!.Value)
                .InclusiveBetween(-90m, 90m).WithMessage("La latitud mínima debe estar entre -90 y 90.");
        });

        When(x => x.MaxLat.HasValue, () => {
            RuleFor(x => x.MaxLat!.Value)
                .InclusiveBetween(-90m, 90m).WithMessage("La latitud máxima debe estar entre -90 y 90.");
        });

        When(x => x.MinLng.HasValue, () => {
            RuleFor(x => x.MinLng!.Value)
                .InclusiveBetween(-180m, 180m).WithMessage("La longitud mínima debe estar entre -180 y 180.");
        });

        When(x => x.MaxLng.HasValue, () => {
            RuleFor(x => x.MaxLng!.Value)
                .InclusiveBetween(-180m, 180m).WithMessage("La longitud máxima debe estar entre -180 y 180.");
        });

        When(x => x.MinLat.HasValue && x.MaxLat.HasValue, () => {
            RuleFor(x => x.MaxLat!.Value)
                .GreaterThanOrEqualTo(x => x.MinLat!.Value)
                .WithMessage("La latitud máxima debe ser mayor o igual a la latitud mínima.");
        });

        When(x => x.MinLng.HasValue && x.MaxLng.HasValue, () => {
            RuleFor(x => x.MaxLng!.Value)
                .GreaterThanOrEqualTo(x => x.MinLng!.Value)
                .WithMessage("La longitud máxima debe ser mayor o igual a la longitud mínima.");
        });
    }
}
