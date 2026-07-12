namespace CampingAI.Application.Queries.Location.GetCountries;
public class GetCountriesQueryHandler : Abstractions.Query.IQueryHandler<GetCountriesQuery, GetCountriesResult> {

    #region Dependencies
    readonly Domain.Repositories.ICountriesReadRepository _countriesReadRepository;
    #endregion

    public GetCountriesQueryHandler(Domain.Repositories.ICountriesReadRepository countriesReadRepository) {
        _countriesReadRepository = countriesReadRepository;
    }

    public async Task<GetCountriesResult> HandleAsync(GetCountriesQuery query) {
        var items = await _countriesReadRepository.GetAllAsync();
        return new GetCountriesResult(items);
    }
}
