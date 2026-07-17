namespace CampingAI.Application.Queries.Camping.GetCampings;
public class GetCampingsQueryHandler : Abstractions.Query.IQueryHandler<GetCampingsQuery, GetCampingsResult> {

    #region Dependencias
    readonly Domain.Repositories.ICampingsReadRepository _campingsReadRepository;
    #endregion

    public GetCampingsQueryHandler(Domain.Repositories.ICampingsReadRepository campingsReadRepository) {
        _campingsReadRepository = campingsReadRepository;
    }

    public async Task<GetCampingsResult> HandleAsync(GetCampingsQuery query) {
        var (items, totalCount) = await _campingsReadRepository.GetPagedAsync(query.Page, query.PageSize, query.Search);
        return new GetCampingsResult(items, totalCount);
    }
}
