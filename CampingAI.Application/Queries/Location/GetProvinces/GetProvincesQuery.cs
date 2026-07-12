namespace CampingAI.Application.Queries.Location.GetProvinces;
public record GetProvincesQuery(Guid? CountryId) : Abstractions.Query.IQuery<GetProvincesResult> {
}
