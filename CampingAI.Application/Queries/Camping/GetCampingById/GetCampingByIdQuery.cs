namespace CampingAI.Application.Queries.Camping.GetCampingById;
public record GetCampingByIdQuery(Guid CampingId) : Abstractions.Query.IQuery<Domain.Entities.Camping> {
}
