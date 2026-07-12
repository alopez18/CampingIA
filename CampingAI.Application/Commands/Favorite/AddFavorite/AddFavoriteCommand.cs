namespace CampingAI.Application.Commands.Favorite.AddFavorite;
public record AddFavoriteCommand(Guid UserId, Guid CampingId) : Abstractions.Command.ICommand;
