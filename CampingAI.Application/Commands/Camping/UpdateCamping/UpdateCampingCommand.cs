namespace CampingAI.Application.Commands.Camping.UpdateCamping;
public record UpdateCampingCommand(Guid CampingId,
                                   string Name,
                                   string Description,
                                   decimal Latitude,
                                   decimal Longitude,
                                   decimal PricePerNight,
                                   int CategoryId,
                                   Guid? ProvinciaId,
                                   IEnumerable<Guid>? FacilityIds) : Abstractions.Command.ICommand {
}
