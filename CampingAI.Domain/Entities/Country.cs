namespace CampingAI.Domain.Entities;
public class Country : Abstractions.Entities.Entity {

    public string Code { get; private set; }
    public ValueObjects.CampingNameVO Name { get; private set; }

    public Country(Guid idCountry, string code, string name) : base(idCountry) {
        if (idCountry == Guid.Empty)
            throw new ArgumentException("The country ID cannot be empty.", nameof(idCountry));
        if (string.IsNullOrWhiteSpace(code))
            throw new Exceptions.DomainException("El código de país no puede estar vacío.");

        Code = code.ToUpperInvariant();
        Name = new ValueObjects.CampingNameVO(name);
    }

    public static Country CreateNew(string code, string name) {
        return new Country(Guid.NewGuid(), code, name);
    }

    public void UpdateName(string name) {
        Name = new ValueObjects.CampingNameVO(name);
    }
}
