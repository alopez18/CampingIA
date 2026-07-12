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
    }
}
