namespace CampingAI.Domain.Abstractions.Entities;
public abstract class Entity {
    public Guid Id { get; set; }

    protected Entity(Guid id) {
        Id = id;
    }

    public bool IsNew() => Id == Guid.Empty;
}