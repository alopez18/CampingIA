namespace CampingAI.Domain.Entities;
public class Service : Abstractions.Entities.Entity {

    public ValueObjects.CampingNameVO Name { get; private set; }

    public Service(Guid idService, string name) : base(idService) {
        if (idService == Guid.Empty)
            throw new ArgumentException("The service ID cannot be empty.", nameof(idService));

        Name = new ValueObjects.CampingNameVO(name);
    }

    public static Service CreateNew(string name) {
        return new Service(Guid.NewGuid(), name);
    }

    public void UpdateName(string name) {
        Name = new ValueObjects.CampingNameVO(name);
    }
}
