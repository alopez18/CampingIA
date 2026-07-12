namespace CampingAI.Application.Commands.Favorite.RemoveFavorite;
public record RemoveFavoriteCommand(Guid UserId, Guid CampingId) : Abstractions.Command.ICommand;
