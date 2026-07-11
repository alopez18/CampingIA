namespace CampingAI.Application.Queries.User.GetUserById;
public class GetUserByIdQueryHandler : Abstractions.Query.IQueryHandler<GetUserByIdQuery, Domain.Entities.User> {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    #endregion

    public GetUserByIdQueryHandler(Domain.Repositories.IUsersReadRepository usersReadRepository) {
        _usersReadRepository = usersReadRepository;
    }

    public async Task<Domain.Entities.User> HandleAsync(GetUserByIdQuery query) {
        var user = await _usersReadRepository.GetByIdAsync(query.UserId)
            ?? throw new KeyNotFoundException($"No existe ningún usuario con el id '{query.UserId}'.");

        return user;
    }
}
