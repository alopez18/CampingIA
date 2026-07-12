namespace CampingAI.Application.Queries.Location.GetProvinces;
public class GetProvincesQueryHandler : Abstractions.Query.IQueryHandler<GetProvincesQuery, GetProvincesResult> {

    #region Dependencies
    readonly Domain.Repositories.IProvincesReadRepository _provincesReadRepository;
    #endregion

    public GetProvincesQueryHandler(Domain.Repositories.IProvincesReadRepository provincesReadRepository) {
        _provincesReadRepository = provincesReadRepository;
    }

    public async Task<GetProvincesResult> HandleAsync(GetProvincesQuery query) {
        var items = query.CountryId.HasValue
            ? await _provincesReadRepository.GetByCountryIdAsync(query.CountryId.Value)
            : await _provincesReadRepository.GetAllAsync();

        return new GetProvincesResult(items);
    }
}
