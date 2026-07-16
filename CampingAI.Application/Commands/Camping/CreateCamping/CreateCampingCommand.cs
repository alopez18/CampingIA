namespace CampingAI.Application.Commands.Camping.CreateCamping;
public record CreateCampingCommand(string Name,
                                   string Description,
                                   decimal Latitude,
                                   decimal Longitude,
                                   decimal PricePerNight,
                                   Guid OwnerId,
                                   Guid CategoryId,
                                   Guid? ProvinciaId,
                                   IEnumerable<Guid>? FacilityIds,
                                   IEnumerable<Guid>? AdditionalCategoryIds) : Abstractions.Command.ICommand {
}
