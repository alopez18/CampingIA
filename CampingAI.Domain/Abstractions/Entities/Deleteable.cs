namespace CampingAI.Domain.Abstractions.Entities;
public abstract class Deleteable : Entity {
    public DateTime? DeletedOn { get; private set; }

    public Deleteable(Guid id, DateTime? deletedOn) : base(id) {
        DeletedOn = deletedOn;
    }


    public void SetDeleted() {
        DeletedOn = DateTime.Now;
    }

    public void SetUndeleted() {
        DeletedOn = null;
    }

}