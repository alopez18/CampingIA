namespace CampingAI.Application.Queries.Camping.GetCampingsByOwner;
public record GetCampingsByOwnerQuery(Guid OwnerId) : Abstractions.Query.IQuery<IEnumerable<Domain.Entities.Camping>> {
}
