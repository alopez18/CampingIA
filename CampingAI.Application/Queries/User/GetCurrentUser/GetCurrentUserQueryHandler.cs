namespace CampingAI.Application.Queries.User.GetCurrentUser;
public class GetCurrentUserQueryHandler : Abstractions.Query.IQueryHandler<GetCurrentUserQuery, Domain.Entities.User> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    #endregion

    public GetCurrentUserQueryHandler(Domain.Repositories.IUsersReadRepository usersReadRepository) {
        _usersReadRepository = usersReadRepository;
    }

    public async Task<Domain.Entities.User> HandleAsync(GetCurrentUserQuery query) {
        var user = await _usersReadRepository.GetByIdAsync(query.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{query.UserId}'.");

        return user;
    }
}
