namespace CampingAI.Application.Queries.User.GetPendingManagers;
public class GetPendingManagersQueryHandler : Abstractions.Query.IQueryHandler<GetPendingManagersQuery, IEnumerable<Domain.Entities.User>> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    #endregion

    public GetPendingManagersQueryHandler(Domain.Repositories.IUsersReadRepository usersReadRepository) {
        _usersReadRepository = usersReadRepository;
    }

    public async Task<IEnumerable<Domain.Entities.User>> HandleAsync(GetPendingManagersQuery query) {
        return await _usersReadRepository.GetPendingManagersAsync();
    }
}
