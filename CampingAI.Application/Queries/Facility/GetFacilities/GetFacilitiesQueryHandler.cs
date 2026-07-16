namespace CampingAI.Application.Queries.Facility.GetFacilities;
public class GetFacilitiesQueryHandler : Abstractions.Query.IQueryHandler<GetFacilitiesQuery, GetFacilitiesResult> {

    #region Dependencies
    readonly Domain.Repositories.IFacilitiesReadRepository _facilitiesReadRepository;
    #endregion

    public GetFacilitiesQueryHandler(Domain.Repositories.IFacilitiesReadRepository facilitiesReadRepository) {
        _facilitiesReadRepository = facilitiesReadRepository;
    }

    public async Task<GetFacilitiesResult> HandleAsync(GetFacilitiesQuery query) {
        var items = await _facilitiesReadRepository.GetAllAsync();
        return new GetFacilitiesResult(items);
    }
}
