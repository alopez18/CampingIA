namespace CampingAI.Domain.Entities;
public class Favorite : Abstractions.Entities.Entity {

    public Guid UserId { get; private set; }
    public Guid CampingId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Favorite(Guid idFavorite,
                    Guid userId,
                    Guid campingId,
                    DateTime createdAt) : base(idFavorite) {
        if (idFavorite == Guid.Empty)
            throw new ArgumentException("The favorite ID cannot be empty.", nameof(idFavorite));

        UserId = userId;
        CampingId = campingId;
        CreatedAt = createdAt;
    }

    public static Favorite CreateNew(Guid userId, Guid campingId) {
        return new Favorite(Guid.NewGuid(), userId, campingId, DateTime.UtcNow);
    }
}
