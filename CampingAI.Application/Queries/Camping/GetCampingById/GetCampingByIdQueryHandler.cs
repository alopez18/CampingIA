namespace CampingAI.Application.Queries.Camping.GetCampingById;
public class GetCampingByIdQueryHandler : Abstractions.Query.IQueryHandler<GetCampingByIdQuery, Domain.Entities.Camping> {

    #region Dependencias
    readonly Domain.Repositories.ICampingsReadRepository _campingsReadRepository;
    #endregion

    public GetCampingByIdQueryHandler(Domain.Repositories.ICampingsReadRepository campingsReadRepository) {
        _campingsReadRepository = campingsReadRepository;
    }

    public async Task<Domain.Entities.Camping> HandleAsync(GetCampingByIdQuery query) {
        var camping = await _campingsReadRepository.GetByIdAsync(query.CampingId)
            ?? throw new KeyNotFoundException($"No existe ningún camping con el id '{query.CampingId}'.");

        return camping;
    }
}
