namespace CampingAI.Application.Commands.Camping.CreateCamping;
public record CreateCampingCommand(string Name,
                                   string Description,
                                   decimal Latitude,
                                   decimal Longitude,
                                   decimal PricePerNight,
                                   Guid OwnerId,
                                   int CategoryId,
                                   IEnumerable<Guid>? FacilityIds) : Abstractions.Command.ICommand {
}
