namespace CampingAI.Application.Queries.Category.GetCategories;
public class GetCategoriesQueryHandler : Abstractions.Query.IQueryHandler<GetCategoriesQuery, GetCategoriesResult> {

    #region Dependencies
    readonly Domain.Repositories.ICategoriesReadRepository _categoriesReadRepository;
    #endregion

    public GetCategoriesQueryHandler(Domain.Repositories.ICategoriesReadRepository categoriesReadRepository) {
        _categoriesReadRepository = categoriesReadRepository;
    }

    public async Task<GetCategoriesResult> HandleAsync(GetCategoriesQuery query) {
        var items = await _categoriesReadRepository.GetAllAsync();
        return new GetCategoriesResult(items);
    }
}
