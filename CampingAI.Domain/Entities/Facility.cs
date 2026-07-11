namespace CampingAI.Domain.Entities;
public class Facility : Abstractions.Entities.Entity {

    public ValueObjects.CampingNameVO Name { get; private set; }

    public Facility(Guid idFacility, string name) : base(idFacility) {
        if (idFacility == Guid.Empty)
            throw new ArgumentException("The facility ID cannot be empty.", nameof(idFacility));

        Name = new ValueObjects.CampingNameVO(name);
    }

    public static Facility CreateNew(string name) {
        return new Facility(Guid.NewGuid(), name);
    }

    public void UpdateName(string name) {
        Name = new ValueObjects.CampingNameVO(name);
    }
}
