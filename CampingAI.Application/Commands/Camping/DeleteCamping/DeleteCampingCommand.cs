namespace CampingAI.Application.Commands.Camping.DeleteCamping;
public record DeleteCampingCommand(Guid CampingId) : Abstractions.Command.ICommand {
}
