namespace CampingAI.Application.Queries.Camping.SearchCampings;
public class SearchCampingsQueryHandler : Abstractions.Query.IQueryHandler<SearchCampingsQuery, SearchCampingsResult> {

    #region Dependencies
    readonly Domain.Repositories.ICampingsReadRepository _campingsReadRepository;
    readonly Domain.Repositories.IProvincesReadRepository _provincesReadRepository;
    #endregion

    public SearchCampingsQueryHandler(Domain.Repositories.ICampingsReadRepository campingsReadRepository,
                                      Domain.Repositories.IProvincesReadRepository provincesReadRepository) {
        _campingsReadRepository = campingsReadRepository;
        _provincesReadRepository = provincesReadRepository;
    }

    public async Task<SearchCampingsResult> HandleAsync(SearchCampingsQuery query) {
        var provinciaId = query.ProvinciaId;

        if (provinciaId is null && !string.IsNullOrWhiteSpace(query.ProvinciaCode)) {
            var province = await _provincesReadRepository.GetByCodeAsync(query.ProvinciaCode);
            provinciaId = province?.Id;
        }

        var filters = new Domain.Repositories.CampingSearchFilters(
            query.Name,
            provinciaId,
            query.CategoryId,
            query.MinPrice,
            query.MaxPrice,
            query.FacilityIds,
            query.MinLat,
            query.MaxLat,
            query.MinLng,
            query.MaxLng,
            query.Page,
            query.PageSize);

        var (items, totalCount) = await _campingsReadRepository.SearchAsync(filters);
        return new SearchCampingsResult(items, totalCount);
    }
}
