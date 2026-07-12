namespace CampingAI.Domain.Entities;
public class Province : Abstractions.Entities.Entity {

    public string Code { get; private set; }
    public ValueObjects.CampingNameVO Name { get; private set; }
    public Guid CountryId { get; private set; }

    public Province(Guid idProvince, string code, string name, Guid countryId) : base(idProvince) {
        if (idProvince == Guid.Empty)
            throw new ArgumentException("The province ID cannot be empty.", nameof(idProvince));
        if (string.IsNullOrWhiteSpace(code))
            throw new Exceptions.DomainException("El código de provincia no puede estar vacío.");
        if (countryId == Guid.Empty)
            throw new Exceptions.DomainException("El CountryId no puede estar vacío.");

        Code = code.ToUpperInvariant();
        Name = new ValueObjects.CampingNameVO(name);
        CountryId = countryId;
    }

    public static Province CreateNew(string code, string name, Guid countryId) {
        return new Province(Guid.NewGuid(), code, name, countryId);
    }

    public void UpdateName(string name) {
        Name = new ValueObjects.CampingNameVO(name);
    }
}
