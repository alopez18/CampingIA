namespace CampingAI.Application.Commands.Camping.UpdateCamping;
public record UpdateCampingCommand(Guid CampingId,
                                   string Name,
                                   string Description,
                                   decimal Latitude,
                                   decimal Longitude,
                                   decimal PricePerNight,
                                   Guid CategoryId,
                                   Guid? ProvinciaId,
                                   IEnumerable<Guid>? FacilityIds,
                                   IEnumerable<Guid>? AdditionalCategoryIds) : Abstractions.Command.ICommand {
}
