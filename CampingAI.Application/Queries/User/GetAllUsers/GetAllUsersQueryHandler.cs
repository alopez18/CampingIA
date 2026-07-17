namespace CampingAI.Application.Queries.User.GetAllUsers;
public class GetAllUsersQueryHandler : Abstractions.Query.IQueryHandler<GetAllUsersQuery, GetAllUsersResult> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    #endregion

    public GetAllUsersQueryHandler(Domain.Repositories.IUsersReadRepository usersReadRepository) {
        _usersReadRepository = usersReadRepository;
    }

    public async Task<GetAllUsersResult> HandleAsync(GetAllUsersQuery query) {
        var (items, totalCount) = await _usersReadRepository.GetPagedAsync(query.Page, query.PageSize, query.Search);
        return new GetAllUsersResult(items, totalCount);
    }
}
