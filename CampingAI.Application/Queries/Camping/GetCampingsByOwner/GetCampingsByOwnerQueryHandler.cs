namespace CampingAI.Application.Queries.Camping.GetCampingsByOwner;
public class GetCampingsByOwnerQueryHandler : Abstractions.Query.IQueryHandler<GetCampingsByOwnerQuery, IEnumerable<Domain.Entities.Camping>> {

    #region Dependencias
    readonly Domain.Repositories.ICampingsReadRepository _campingsReadRepository;
    #endregion

    public GetCampingsByOwnerQueryHandler(Domain.Repositories.ICampingsReadRepository campingsReadRepository) {
        _campingsReadRepository = campingsReadRepository;
    }

    public async Task<IEnumerable<Domain.Entities.Camping>> HandleAsync(GetCampingsByOwnerQuery query) {
        return await _campingsReadRepository.GetByOwnerAsync(query.OwnerId);
    }
}
